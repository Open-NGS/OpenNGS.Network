using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Sessions;
using Unity.PSN.PS5.Users;
using UnityEngine;

#if UNITY_PS4
using PlatformInput = UnityEngine.PS4.PS4Input;
#elif UNITY_PS5
using PlatformInput = UnityEngine.PS5.PS5Input;
#endif

namespace OpenNGS.Platform.PS5
{
    class PSUser
    {
#if UNITY_PS5 || UNITY_PS4
        static PSUser()
        {
            PlatformInput.OnUserServiceEvent += OnUserServiceEvent;
        }
#if UNITY_PS5
        static void OnUserServiceEvent(PlatformInput.UserServiceEventType eventtype, uint userid)
#elif UNITY_PS4
        static void OnUserServiceEvent(uint eventtype, uint userid)
#endif
        {
            //User user = FindUser((int)userid);
            Debug.Log("OnUserServiceEvent -> User state changed : " + eventtype);

#if UNITY_PS5
            if (eventtype == PlatformInput.UserServiceEventType.Login)
#elif UNITY_PS4
            if (eventtype == 0) // SCE_USER_SERVICE_EVENT_TYPE_LOGIN
#endif
            {
                UserLoggedIn((int)userid);
            }
#if UNITY_PS5
            else if (eventtype == PlatformInput.UserServiceEventType.Logout)
#elif UNITY_PS4
            else if (eventtype == 1)
#endif
            {
                UserLoggedOut((int)userid);
            }
        }


        public static void Initialize(int maxPlayer)
        {
            for (int i = 0; i < maxPlayer; i++)
            {
                int playerId = i;

                users[playerId] = new PSUser();
            }
        }

        public static void UserLoggedIn(int userid)
        {
            PSUser user = FindUser((int)userid);

            if (user != null)
            {
                if (user.registerSequence == RegisterSequences.NotSet)
                {
                    user.registerSequence = RegisterSequences.AddingUser;

                    UserSystem.AddUserRequest request = new UserSystem.AddUserRequest() { UserId = (int)userid };

                    var requestOp = new AsyncRequest<UserSystem.AddUserRequest>(request).ContinueWith((antecedent) =>
                    {
                        if (antecedent != null && antecedent.Request != null)
                        {
                            user.registerSequence = RegisterSequences.UserAdded;
                        }
                    });

                    UserSystem.Schedule(requestOp);

                    Debug.Log("User being added...");
                }
            }

        }

        public static void UserLoggedOut(int userid)
        {
            PSUser user = FindUser((int)userid);

            if (user != null)
            {
                if (user.registerSequence != RegisterSequences.UserLoggingOut)
                {
                    user.registerSequence = RegisterSequences.UserLoggingOut;

                    SessionsManager.UnregisterUserSessionEventAsync((int)userid);

                    UserSystem.RemoveUserRequest request = new UserSystem.RemoveUserRequest() { UserId = userid };

                    var requestOp = new AsyncRequest<UserSystem.RemoveUserRequest>(request).ContinueWith((antecedent) =>
                    {
                        if (antecedent != null && antecedent.Request != null)
                        {
                            PSUser registeredUser = PSUser.FindUser(antecedent.Request.UserId);

                            if (registeredUser != null)
                            {
                                if (PS5SDK.CheckAysncRequestOK(antecedent))
                                {
                                    Debug.Log("User Removed");

                                    registeredUser.registerSequence = RegisterSequences.NotSet;
                                }
                            }
                        }
                    });

                    UserSystem.Schedule(requestOp);

                    Debug.Log("User being removed...");
                }
            }
        }




        public static PSUser[] users = new PSUser[4];

        public static PSUser FindUser(int userId)
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null)
                {
                    if (users[i].loggedInUser.userId != 0 && users[i].loggedInUser.userId == userId)
                    {
                        return users[i];
                    }
                }
            }
            return null;
        }
        public enum RegisterSequences
        {
            NotSet,
            AddingUser,
            UserAdded,
            RegisteringUser,
            UserRegistered,
            UserAddedButNoOnline,
            UserLoggingOut,
        }

        RegisterSequences registerSequence = RegisterSequences.NotSet;


#if UNITY_PS4 || UNITY_PS5
        public PlatformInput.LoggedInUser loggedInUser;
#endif

        public static PSUser activeUser;
        public void Update()
        {

        }
    }
#endif
}