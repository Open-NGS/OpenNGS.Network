using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS.Audio
{
    public interface IAudioPackage
    {
        void Initialize();
        void LoadPackage(string name);
        void LoadPackageAsync(string name);
        void UnLoadPackage(string name, Action onFinish);
        void UnLoadPackageAsync(string name, Action onFinish);
        T GetSound<T>(string sound);
    }

}
