using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShareProvider : IModuleProvider
{
    void Initialize();

    void ShowSharePlatformList(PlatformShareInfo platformShareInfo);

    void SendMessage(PlatformShareInfo platformShareInfo, string channel = "");

    void Share(PlatformShareInfo platformShareInfo, string channel = "");
}
