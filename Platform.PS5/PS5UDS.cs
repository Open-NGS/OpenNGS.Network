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
        Debug.Log("[PS5UDS]ActivityStart");
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

    private void PostEvent(UniversalDataSystem.UDSEvent udsEvent)
    {
        UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

        request.UserId = PSUser.activeUser.loggedInUser.userId;
        request.CalculateEstimatedSize = true;
        request.EventData = udsEvent;

        var requestOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("Event sent - Estimated size = " + antecedent.Request.EstimatedSize);
            }
            else
            {
                Debug.LogError("Event send error");
            }
        });

        UniversalDataSystem.Schedule(requestOp);

        UniversalDataSystem.EventDebugStringRequest stringRequest = new UniversalDataSystem.EventDebugStringRequest();

        stringRequest.UserId = PSUser.activeUser.loggedInUser.userId;
        stringRequest.EventData = udsEvent;

        var secondRequestOp = new AsyncRequest<UniversalDataSystem.EventDebugStringRequest>(stringRequest).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log(antecedent.Request.Output);
            }
            else
            {
                Debug.LogError("Event string error");
            }
        });

        UniversalDataSystem.Schedule(secondRequestOp);
    }
}
