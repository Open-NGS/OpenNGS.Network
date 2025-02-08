using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using System;
using Unity.PSN.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;
using UnityEngine;

public class PS5Activity : IActivityProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.ACTIVITY;

    public void Start()
    {
        Debug.Log("[PS5Activity]Start");
        
    }
    public void Stop()
    {
        Debug.Log("[PS5Activity]Stop");

    }

    public void ResetAllActivities()
    {
        
    }

    public void ActivityStart(string actitityId)
    {
        UniversalDataSystem.UDSEvent myEvent = new UniversalDataSystem.UDSEvent();
        myEvent.Create("activityStart");
        myEvent.Properties.Set("activityId", actitityId);

        UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

        request.UserId = PSUser.activeUser.loggedInUser.userId;
        request.EventData = myEvent;

        var requestOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("activityStart Request finished = " + antecedent.Request.Result);
            }
        });

        UniversalDataSystem.Schedule(requestOp);

        Debug.Log("[PS5Activity]Progress Trophy Updating");
    }

    public void ActivityEnd(string actitityId, string outcome)
    {
        UniversalDataSystem.UDSEvent myEvent = new UniversalDataSystem.UDSEvent();
        myEvent.Create("activityEnd");
        myEvent.Properties.Set("activityId", actitityId);
        myEvent.Properties.Set("outcome", outcome);

        UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

        request.UserId = PSUser.activeUser.loggedInUser.userId;
        request.EventData = myEvent;
        var getTrophyOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("Progress Trophy Update Request finished = " + antecedent.IsCompleted + " : Progress = " + antecedent);
            }
        });
        UniversalDataSystem.Schedule(getTrophyOp);
        Debug.Log("[PS5Activity]activityEnd");
    }

    public void Update()
    {
        
    }
}
