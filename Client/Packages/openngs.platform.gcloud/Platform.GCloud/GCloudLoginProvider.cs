//using GCloud.MSDK;
using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCloudLoginProvider : ILoginProvider
{
    public Platform_MODULE Module => Platform_MODULE.LOGIN;

    public GCloudLoginProvider()
    {

    }

    public void AutoLogin()
    {
        MSDKLogin.AutoLogin();
    }

    public PlatformLoginRet GetLoginRet()
    {
        return new PlatformLoginRet(MSDKLogin.GetLoginRet().ToJsonString());
    }

    public void Login(string channel, string permissions = "", string subChannel = "", string extraJson = "")
    {
        MSDKLogin.Login(channel, permissions, subChannel, extraJson);
    }

    public void Logout(string channel = "")
    {
        MSDKLogin.Logout(channel);
    }

    public void SwitchUser(bool useLaunchUser)
    {
        MSDKLogin.SwitchUser(useLaunchUser);
    }
}
