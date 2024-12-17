// See https://aka.ms/new-console-template for more information
using OpenNGS.SDK;
using OpenNGS.SDK.Auth;
using OpenNGS.SDK.Avatar;
using OpenNGS.SDK.Core;
using OpenNGS.SDK.Core.Initiallization;

#region Auth
Console.WriteLine("Test Auth ------------------------------- !");

OpenNGSPlatformServices.Initialize(new InitializationOptions()
{
    AppId = "9bd3835c-a035-4f13-944d-84cd992267d4",
    AppSecret = "123"
});

AuthcationService.Instance.LoginCallback += (info) =>
{
    Log.Info($"已登录 {info.Nickname}");
};

AuthcationService.Instance.AutoLoginCallback += (result) =>
{
    if (result)
    {
        Log.Info($"已登录 {AuthcationService.Instance.User.Nickname}");
    }
    else
    {
        Log.Info("未登录，正在登陆....");
        AuthcationService.Instance.LoginByUsernamePassword("noah", "zhouliang7410.");
    }
};

AuthcationService.Instance.AutoLogin();

//auth.SetAuthBaseUrl("http://192.168.10.115:5011");

//auth.Logout();

Console.WriteLine("Test Auth End ---------------------------- !");
#endregion

#region Avatar

//await AvatarSDK.GetUserAvatar();

#endregion

Console.ReadLine();
