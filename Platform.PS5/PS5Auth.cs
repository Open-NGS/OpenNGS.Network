using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using System.Collections;
using System.Collections.Generic;
using Unity.PSN.PS5.Auth;
using Unity.PSN.PS5.Aysnc;
using UnityEngine;

public class PS5Auth : IModuleProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.LOGIN;

    public void Start()
    {

    }
    public void Stop()
    {
        
    }


    public void Update()
    {
    }

    public void GetAuthorizationCode()
    {
        Authentication.GetAuthorizationCodeRequest request = new Authentication.GetAuthorizationCodeRequest()
        {
            UserId = PSUser.activeUser.loggedInUser.userId,
#if UNITY_PS5
            ClientId = "686986a6-3b34-4a42-89d1-b4ba193bc80f",
#elif UNITY_PS4
                    ClientId = "c5806b90-16f4-4086-9b43-665b69654b05",
#endif
            Scope = "psn:s2s"
        };

        var requestOp = new AsyncRequest<Authentication.GetAuthorizationCodeRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("GetAuthorizationCodeRequest:");
                Debug.Log("  ClientId = " + antecedent.Request.ClientId);
                Debug.Log("  Scope = " + antecedent.Request.Scope);
                Debug.Log("  AuthCode = " + antecedent.Request.AuthCode);
                Debug.Log("  IssuerId = " + antecedent.Request.IssuerId);
            }
        });

        Authentication.Schedule(requestOp);
    }

}
