using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.UDS;
using UnityEngine;

public class PS5UDS : IUDSProvider
{
    public PLATFORM_MODULE Module =>  PLATFORM_MODULE.UDS;

   
    public void EventToString()
    {
        
    }

    public void GetMemoryStats()
    {
        
    }

    public void PostEvent()
    {
        
    }

    public void Start()
    {
        Debug.Log("[PS5UDS]Start");
        if (!UniversalDataSystem.IsInitialized)
        {
            UniversalDataSystem.StartSystemRequest request = new UniversalDataSystem.StartSystemRequest();
            request.PoolSize = 256 * 1024;
            var requestOp = new AsyncRequest<UniversalDataSystem.StartSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5SDK.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("[PS5UDS]UDS System Started");
                }
            });
            UniversalDataSystem.Schedule(requestOp);
        }
    }

    public void Stop()
    {
        Debug.Log("[PS5UDS]Stop");
        if (UniversalDataSystem.IsInitialized)
        {
            UniversalDataSystem.StopSystemRequest request = new UniversalDataSystem.StopSystemRequest();
            var requestOp = new AsyncRequest<UniversalDataSystem.StopSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5SDK.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("[PS5UDS]UDS System Stopped");
                }
            });
            UniversalDataSystem.Schedule(requestOp);
        }
    }


    public void Update()
    {
        
    }
}
