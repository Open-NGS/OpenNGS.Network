using OpenNGS;
using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Achievement.Service;
using OpenNGS.ERPC;
using OpenNGS.ERPC.Configuration;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;


public class AchievementAPIController : Singleton<AchievementAPIController>, INiAchievementService
{
    private List<AchievementState> achievementStates = new  List<AchievementState>();

    IRPCLite m_rpcLite;
    RPCService m_service;
    private RPCClient m_cli;
    INgAchievementSystem m_NgAchievementSystem;

    public AchievementAPIController()
    {
        m_NgAchievementSystem = App.GetService<INgAchievementSystem>();
    }

    public void Init(uint playerID)
    {
        GetOrCreatePlayerAchievements(playerID);

        m_cli = new RPCClient();
        m_rpcLite = new IRPCLite();
        m_rpcLite.Init(MessageSender);
        m_rpcLite.InitClient(m_cli);
        System.Type _insType = typeof(INiAchievementService);
        ServiceAttribute sa = _insType.GetCustomAttribute<ServiceAttribute>(true);
        m_service = new RPCService(sa.Name);
        m_rpcLite.InitService(m_service);

        // 注册服务方法
        m_service.AddMethod<GetAchievementsReq, GetAchievementsRsp>(sa.Name + "/" + "GetAchievements", ActionGetAchievementsReq);
        m_service.AddMethod<UpdateAchievementReq, UpdateAchievementRsp>(sa.Name + "/" + "UpdateAchievement", ActionUpdateAchievementReq);
        m_service.AddMethod<GetAchievementRewardReq, GetAchievementRewardRsp>(sa.Name + "/" + "GetAchievementReward", ActionGetAchievementRewardReq);
    }
    public bool MessageSender(IRPCMessage msg)
    {
        m_rpcLite.OnMessage(msg);
        return true;
    }
    // 初始化玩家成就数据
    private void GetOrCreatePlayerAchievements(uint playerID)
    {
        var playerAchievements = achievementStates.Where(a => a.PlayerID == playerID).ToList();
        //数据库
        m_NgAchievementSystem.AddAchievementStates(playerAchievements);
    }

    // 获取成就列表
    private async Task<GetAchievementsRsp> ActionGetAchievementsReq(ServerContext context, GetAchievementsReq req)
    {
        await Task.Delay(100);
        var rsp = new GetAchievementsRsp();
        try
        {
            rsp = await m_NgAchievementSystem.GetAchievements(req);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching achievements: {ex.Message}");
        }
        return rsp;
    }

    // 更新成就进度
    private async Task<UpdateAchievementRsp> ActionUpdateAchievementReq(ServerContext context, UpdateAchievementReq req)
    {
        await Task.Delay(100);
        var rsp = new UpdateAchievementRsp();
        try
        {
            rsp = await m_NgAchievementSystem.UpdateAchievement(req);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating achievement: {ex.Message}");
            rsp.result = Achievement_Result.AchievementResult_Fail_NotExist;
        }
        return rsp;
    }

    // 领取成就奖励
    private async Task<GetAchievementRewardRsp> ActionGetAchievementRewardReq(ServerContext context, GetAchievementRewardReq req)
    {
        await Task.Delay(100);
        var rsp = new GetAchievementRewardRsp();
        try
        {
            rsp = await m_NgAchievementSystem.GetAchievementReward(req);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting reward: {ex.Message}");
            rsp.result = Achievement_Result.AchievementResult_Fail_NotExist;
        }
        return rsp;
    }

    public Task<GetAchievementRewardRsp> GetAchievementReward(GetAchievementRewardReq req, ClientContext context = null)
    {
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiAchievementService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }

        try
        {
            //return m_NgAchievementSystem.GetAchievementReward(req);
            // 调用 RPC 方法
            return m_cli.UnaryInvoke<GetAchievementRewardReq, GetAchievementRewardRsp>(context, req);
        }
        catch (Exception ex)
        {
            Debug.LogError($"AddCollection failed: {ex.Message}");
            return null;
        }
    }
    
    // 获取成就列表
    public Task<GetAchievementsRsp> GetAchievements(GetAchievementsReq req, ClientContext context = null)
    {
        m_NgAchievementSystem.GetAchievements(req, context);
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiAchievementService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }

        try
        {
            //return m_NgAchievementSystem.GetAchievements(req);
            // 调用 RPC 方法获取数据
            return m_cli.UnaryInvoke<GetAchievementsReq, GetAchievementsRsp>(context, req);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetCollections failed: {ex.Message}");
            return null;
        }
    }
    // 更新成就进度
    public Task<UpdateAchievementRsp> UpdateAchievement(UpdateAchievementReq value, ClientContext context = null)
    {
        if (context != null)
        {
            ServiceAttribute sa = typeof(INiAchievementService).GetCustomAttribute<ServiceAttribute>(true);
            context.FuncName = sa.Name + "/" + MethodBase.GetCurrentMethod().Name;
        }

        try
        {
            //return m_NgAchievementSystem.UpdateAchievement(value);
            // 调用 RPC 方法
            return m_cli.UnaryInvoke<UpdateAchievementReq, UpdateAchievementRsp>(context, value);
        }
        catch (Exception ex)
        {
            Debug.LogError($"AddCollection failed: {ex.Message}");
            return null;
        }
    }
}
