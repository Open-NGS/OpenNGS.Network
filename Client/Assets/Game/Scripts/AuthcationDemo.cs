using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Core.Initiallization;
using OpenNGS.SDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthcationDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test Auth ------------------------------- !");

        OpenNGSPlatformServices.Initialize(new InitializationOptions()
        {
            AppId = "iboN4V3anKRnsKwgudonW0ESxGwJLNUz2rhN",
            AppSecret = "YDwqSQ5Be0oGIpQJ6sPBlHLHveRTfC5p"
        }, new SDKLogger());

        AuthcationService.Instance.LoginCallback += (info) =>
        {
            Debug.Log($"ÒÑµÇÂ¼ {info.Nickname}");
        };

        AuthcationService.Instance.VerificationCodeCallback += () =>
        {
            int a = 0;
        };

        AuthcationService.Instance.AutoLoginCallback += (result) =>
        {
            if (result)
            {
                Debug.Log($"ÒÑµÇÂ¼ {AuthcationService.Instance.User.Nickname}");
            }
            else
            {
                Debug.Log("Î´µÇÂ¼£¬ÕýÔÚµÇÂ½....");
                AuthcationService.Instance.LoginByUsernamePassword("xxx", "xxx");
            }
        };

        AuthcationService.Instance.AutoLogin();

        Debug.Log("Test Auth End ---------------------------- !");
    }
}
