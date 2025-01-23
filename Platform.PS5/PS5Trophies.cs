using OpenNGS.Platform;
using OpenNGS.Platform.PS5;
using System.Collections;
using System.Collections.Generic;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;
using UnityEngine;
using static PlasticPipe.Server.MonitorStats;

public class PS5Trophies : IAchievementProvider
{
    public PLATFORM_MODULE Module => PLATFORM_MODULE.ACHIEVEMENT;

    public void Start()
    {
        if (!TrophySystem.IsInitialized)
        {
            TrophySystem.StartSystemRequest request = new TrophySystem.StartSystemRequest();
            var requestOp = new AsyncRequest<TrophySystem.StartSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5SDK.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("TrophySystem Started");
                }
            });

            TrophySystem.Schedule(requestOp);
        }
    }
    public void Stop()
    {
        if (TrophySystem.IsInitialized)
        {
            TrophySystem.StopSystemRequest request = new TrophySystem.StopSystemRequest();

            var requestOp = new AsyncRequest<TrophySystem.StopSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5SDK.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("TrophySystem Stopped");
                }
            });

            TrophySystem.Schedule(requestOp);
        }
    }
    public void GetGameInfo(string accountId)
    {
        if (TrophySystem.IsInitialized)
        {
            TrophySystem.TrophyGameDetails gameDetails = new TrophySystem.TrophyGameDetails();
            TrophySystem.TrophyGameData gameData = new TrophySystem.TrophyGameData();

            TrophySystem.GetGameInfoRequest request = new TrophySystem.GetGameInfoRequest()
            {
                UserId = PSUser.activeUser.loggedInUser.userId,
                GameDetails = gameDetails,
                GameData = gameData
            };

            var requestOp = new AsyncRequest<TrophySystem.GetGameInfoRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5SDK.CheckAysncRequestOK(antecedent))
                {
                    //OnScreenLog.Add("GetGameInfoRequest completed");

                    //OutputTrophyGameDetails(antecedent.Request.GameDetails);
                    //OutputTrophyGameData(antecedent.Request.GameData);
                }
            });

            TrophySystem.Schedule(requestOp);
        }
    }

    public void Unlock(int id)
    {
        UniversalDataSystem.UnlockTrophyRequest request = new UniversalDataSystem.UnlockTrophyRequest();

        request.TrophyId = id;
        request.UserId = PSUser.activeUser.loggedInUser.userId;

        var getTrophyOp = new AsyncRequest<UniversalDataSystem.UnlockTrophyRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("Trophy Unlock Request finished = " + antecedent.Request.TrophyId);
            }
        });

        UniversalDataSystem.Schedule(getTrophyOp);

        Debug.Log("Trophy Unlocking");
    }

    public void UnlockProgress(int id, long value)
    {
        UniversalDataSystem.UpdateTrophyProgressRequest request = new UniversalDataSystem.UpdateTrophyProgressRequest();

        request.TrophyId = id;
        request.UserId = PSUser.activeUser.loggedInUser.userId;
        request.Progress = value;

        var getTrophyOp = new AsyncRequest<UniversalDataSystem.UpdateTrophyProgressRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                Debug.Log("Progress Trophy Update Request finished = " + antecedent.Request.TrophyId + " : Progress = " + antecedent.Request.Progress);
                GetTrophyInfo(id);
            }
        });

        UniversalDataSystem.Schedule(getTrophyOp);

        Debug.Log("Progress Trophy Updating");
    }

    enum SampleTrophies
    {
        Platinum = 0,
        BasicGold = 1,
        BasicSilver = 2,
        BasicBronze = 3,
        Hidden = 4,
        ProggreeStatThree = 5,
        ProgressStatTwenty = 6,
        BasicProgress = 7,
        Reward = 8,

        LastIndex = Reward,
        TrophyCount,
    }
    int numTrophiesReturned = 0;
    TrophySystem.TrophyDetails[] currentDetails;
    TrophySystem.TrophyData[] currentData;
    public void GetAllTrophyState()
    {
        currentDetails = new TrophySystem.TrophyDetails[(int)SampleTrophies.TrophyCount];
        currentData = new TrophySystem.TrophyData[(int)SampleTrophies.TrophyCount];

        numTrophiesReturned = 0;

        for (int i = 0; i < (int)SampleTrophies.TrophyCount; i++)
        {
            GetTrophyInfo(i);
        }
    }

    public void GetTrophyInfo(int trophyId)
    {
        Debug.Log("Getting info for trophy " + trophyId);

        TrophySystem.GetTrophyInfoRequest request = new TrophySystem.GetTrophyInfoRequest();

        request.UserId = PSUser.activeUser.loggedInUser.userId;
        request.TrophyId = trophyId;
        request.TrophyDetails = new TrophySystem.TrophyDetails();
        request.TrophyData = new TrophySystem.TrophyData();

        var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyInfoRequest>(request).ContinueWith((antecedent) =>
        {
            if (PS5SDK.CheckAysncRequestOK(antecedent))
            {
                //OutputTrophyDetails(antecedent.Request.TrophyDetails);
                //OutputTrophyData(antecedent.Request.TrophyData);
                int id = antecedent.Request.TrophyId;

                if (currentDetails[id] == null)
                {
                    numTrophiesReturned++;
                }

                currentDetails[id] = antecedent.Request.TrophyDetails;
                currentData[id] = antecedent.Request.TrophyData;
            }
        });

        UniversalDataSystem.Schedule(getTrophyOp);
    }

    public void Unlock(string key)
    {
        throw new System.NotSupportedException();
    }

    public void UnlockProgress(string key, long value)
    {
        throw new System.NotSupportedException();
    }

    public void ResetAllAchievements()
    {
        
    }
}
