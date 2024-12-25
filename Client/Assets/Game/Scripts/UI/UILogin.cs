using OpenNGS.IO;
using OpenNGS.Platform;
using OpenNGS.Platform.EEGames;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static OpenNGS.Platform.PlatformReport;

public class UILogin : MonoBehaviour
{
    public class LoginInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string appId { get; set; }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!SteamAPI.Init())
        {
            Debug.LogError("SteamAPI_Init() failed.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private EEGamesSDKProvider m_sdkProvider;
    [SerializeField]
    private TMP_InputField TextUsername;
    [SerializeField]
    private TMP_InputField TextPassword;
    [ContextMenu("Login")]
    public void _OnClickLogin()
    {
        LoginInfo _val = new LoginInfo();
        _val.username = TextUsername.text;
        _val.password = TextPassword.text;
        _val.appId = "9bd3835c-a035-4f13-944d-84cd992267d4";
        fastJSON.JSONParameters jp = new fastJSON.JSONParameters();
        jp.EnableAnonymousTypes = true;
        string strResult = fastJSON.JSON.ToJSON(_val, jp);

        GameInstance.Instance.Init();
        m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.LoginRetEvent += OnLoginResult;
        PlatformLogin.Login("Channel", "", "", strResult);
    }
    public void OnLoginResult(PlatformLoginRet _ret)
    {

    }
    [ContextMenu("Report")]
    public void Report()
    {
        GameInstance.Instance.Init();
        m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        ExtraInfo _extraInf = new ExtraInfo();
        PlatformReport.Report("testevent", _extraInf);
    }
}
