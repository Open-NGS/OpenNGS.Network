using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ILoadingProcessor
{
    void OnUnitySceneLoaded(Action makeSceneActive);
    void OnUnitySceneActive();
    void OnUnitySceneLoadingProcess(float process);
}
