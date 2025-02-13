#if UNITY_PS5
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Sessions;
using Unity.PSN.PS5.Users;
using Unity.PSN.PS5.WebApi;
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
        public int playerId = 0;
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
            Debug.LogFormat("[PSUser]OnUserServiceEvent -> User UserId:0x{0:X} state changed :{1}", userid, eventtype);

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
            Debug.LogFormat("[PSUser] Initialize({0})", maxPlayer);

            Debug.LogFormat("Initial UserId:0x{0:X}  Primary UserId:0x{1:X}", UnityEngine.PS5.Utility.initialUserId, UnityEngine.PS5.Utility.primaryUserId);

            for (int i = 0; i < maxPlayer; i++)
            {
                int playerId = i;

                users[playerId] = new PSUser();
                users[playerId].playerId = playerId;
            }
        }
        public static void CheckRegistration()
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null)
                {
                    if (users[i].IsConnected == true)
                    {
                        if (users[i].loggedInUser.status == 1)
                        {
                            if (users[i].registerSequence == RegisterSequences.NotSet)
                            {
                                UserLoggedIn(users[i].loggedInUser.userId);
                            }
                            else if (users[i].registerSequence == RegisterSequences.UserAdded)
                            {
                                if (users[i].loggedInUser.onlineStatus == PlatformInput.OnlineStatus.SignedIn)
                                {
                                    SessionsManager.RegisterUserSessionEvent(users[i].loggedInUser.userId);
                                    Debug.Log("[PSUser]SessionsManager.RegisterUserSessionEvent");
                                    users[i].registerSequence = RegisterSequences.RegisteringUser;
                                }
                                else
                                {
                                    users[i].registerSequence = RegisterSequences.UserAddedButNoOnline;
                                }
                            }
                            else if (users[i].registerSequence == RegisterSequences.RegisteringUser)
                            {
                                WebApiPushEvent pushEvent = SessionsManager.GetUserSessionPushEvent(users[i].loggedInUser.userId);

                                // Check for registraction.
                                if (pushEvent != null)
                                {
                                    users[i].registerSequence = RegisterSequences.UserRegistered;
                                }
                            }
                            else if (users[i].registerSequence == RegisterSequences.UserAddedButNoOnline)
                            {
                                if (users[i].loggedInUser.onlineStatus == PlatformInput.OnlineStatus.SignedIn)
                                {
                                    SessionsManager.RegisterUserSessionEvent(users[i].loggedInUser.userId);
                                    Debug.Log("[PSUser]SessionsManager.RegisterUserSessionEvent");
                                    users[i].registerSequence = RegisterSequences.RegisteringUser;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (users[i].loggedInUser.status == 0)
                        {
                            if (users[i].registerSequence == RegisterSequences.NotSet)
                            {
                                UserLoggedOut(users[i].loggedInUser.userId);
                            }
                        }
                    }
                }
            }
        }

        public bool IsConnected
        {
#if UNITY_PS4 || UNITY_PS5
            get { return PlatformInput.PadIsConnected(playerId); }
#else
            get { return false; }
#endif
        }

        public static void UserLoggedIn(int userid)
        {
            PSUser user = FindUser((int)userid);
            Debug.LogFormat("[PSUser]UserLoggedIn:UserId:0x{0:X}, User:{1}", userid, user);
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

                    Debug.LogFormat("[PSUser]User UserId:0x{0:X} being added...", userid);
                }

                if (UnityEngine.PS5.Utility.initialUserId == userid)
                {
                    initialUser = user;
                    Debug.LogFormat("[PSUser]Initial User UserId:0x{0:X}  playerId:0x{1:X} userName:{2}", initialUser.loggedInUser.userId, initialUser.playerId, initialUser.loggedInUser.userName);
                }

                if (PS5SDK.InitialUserAlwaysLoggedIn)
                    activeUser = initialUser;
                Debug.LogFormat("[PSUser]Active User UserId:0x{0:X}, User:{1}", PSUser.activeUser.loggedInUser.userId, PSUser.activeUser.loggedInUser.userName);
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
                                    Debug.LogFormat("[PSUser]User UserId:0x{0:X}  Removed", userid);

                                    registeredUser.registerSequence = RegisterSequences.NotSet;
                                }
                            }
                        }
                    });

                    UserSystem.Schedule(requestOp);

                    Debug.LogFormat("[PSUser]User UserId:0x{0:X} being removed...", userid);
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

        bool hasSetupGamepad = false;
        public static PSUser activeUser;

        public static PSUser initialUser;

        public static void Update()
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i] != null)
                {
                    users[i].UpdateGamePad();
                }
            }
        }
        public void UpdateGamePad()
        {
#if UNITY_PS4 || UNITY_PS5
            if (PlatformInput.PadIsConnected(playerId))
            {
                if(!hasSetupGamepad)
                    ToggleGamePad(true);


                if (!PS5SDK.InitialUserAlwaysLoggedIn && activeUser == null)
                {
                    activeUser = this;
                    Debug.LogFormat("[PSUser]Update() activeUser UserId:0x{0:X}, playerId={1}", loggedInUser.userId, activeUser.playerId);
                }
            }
            else if (hasSetupGamepad)
                ToggleGamePad(false);
#endif
        }
        void ToggleGamePad(bool active)
        {
            Debug.LogFormat("[PSUser]ToggleGamePad({1}) activeUser UserId:0x{0:X}, playerId={1} active={2}", loggedInUser.userId, playerId, active);
            if (active)
            {
                // Set 3D Text to whoever's using the pad
#if UNITY_PS4 || UNITY_PS5
                loggedInUser = PlatformInput.RefreshUsersDetails(playerId);
#endif
                hasSetupGamepad = true;
            }
            else
            {
                hasSetupGamepad = false;
            }
        }

#endif
    }
}
#endif