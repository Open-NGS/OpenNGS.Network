using OpenNGS.IO;
using OpenNGS.Platform;
using OpenNGS.Platform.EEGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private EEGamesSDKProvider m_sdkProvider;
    [ContextMenu("Login")]
    private void _OnClickLogin()
    {
        GameInstance.Instance.Init();
        m_sdkProvider = new EEGamesSDKProvider();
        Platform.Init(m_sdkProvider);
        PlatformLogin.Login("Channel");
    }
}
