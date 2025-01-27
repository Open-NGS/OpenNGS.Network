using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform
{
    public interface IBaseProvider : IModuleProvider
    {
        bool IsDebug { get; set; }
        bool IsAppInstalled(string appStr);
        bool IsAppSubscribed(string appId);
    }
}
