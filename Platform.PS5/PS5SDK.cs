using System;
#if UNITY_PS5
using Unity.PSN.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Initialization;
using UnityEngine;

namespace OpenNGS.Platform.PS5
{
    internal class PS5SDK
    {

        static InitResult initResult;
        internal static bool InitialUserAlwaysLoggedIn;

        public static bool CheckAysncRequestOK<R>(AsyncRequest<R> asyncRequest) where R : Request
        {
            if (asyncRequest == null)
            {
                UnityEngine.Debug.LogError("AsyncRequest is null");
                return false;
            }

            return CheckRequestOK<R>(asyncRequest.Request);
        }

        public static bool CheckRequestOK<R>(R request) where R : Request
        {
            if (request == null)
            {
                UnityEngine.Debug.LogError("Request is null");
                return false;
            }

            if (request.Result.apiResult == APIResultTypes.Success)
            {
                return true;
            }

            OutputApiResult(request.Result);

            return false;
        }

        public static void OutputApiResult(APIResult result)
        {
            if (result.apiResult == APIResultTypes.Success)
            {
                return;
            }

            string output = result.ErrorMessage();

            //OnScreenLog.AddError($"\n{(sceCode)(long)(UInt32)result.sceErrorCode}\n");
            //Debug.LogError($"\n{(sceCode)(long)(UInt32)result.sceErrorCode}\n");

            if (result.apiResult == APIResultTypes.Error)
            {
                Debug.LogError(output);
            }
            else
            {
                Debug.LogWarning(output);
            }
        }

        internal static void Initialize()
        {
            try
            {
                Debug.Log("[PS5SDK]Initialize");
                initResult = Main.Initialize();

                // RequestCallback.OnRequestCompletion += OnCompleteion;

                if (initResult.Initialized == true)
                {
                    Debug.Log("[PS5SDK]PSN Initialized ");
                    Debug.Log("[PS5SDK]Plugin SDK Version : " + initResult.SceSDKVersion.ToString());
                }
                else
                {
                    Debug.Log("[PS5SDK]PSN not initialized ");
                }
            }
            catch (PSNException e)
            {
                Debug.LogError("[PS5SDK]Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                Debug.LogError("[PS5SDK]Missing DLL Expection : " + e.Message);
                Debug.LogError("[PS5SDK]The PlayStation SDK will not run in the editor.");
            }
#endif

            string[] args = System.Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                Debug.Log("Args:");

                for (int i = 0; i < args.Length; i++)
                {
                    Debug.Log("  " + args[i]);
                }
            }
            else
            {
                Debug.Log("No Args");
            }

            PSUser.Initialize(4);
        }

        internal static void Update()
        {
            PSUser.CheckRegistration();
            try
            {
                Main.Update();
            }
            catch (Exception e)
            {
                Debug.LogError("[PS5SDK]Main.Update Exception : " + e);
            }
            PSUser.Update();
            //Unity.PSN.PS5.Sessions.TestJSONParsing.TestJSONNotifciations();
            // Unity.PSN.PS5.Matchmaking.TestJSONParsing.TestJSONNotifciations();
        }

        internal static void Terminate()
        {
            Main.ShutDown();
            Debug.Log("[PS5SDK]Terminated");
        }
    }
}
#endif