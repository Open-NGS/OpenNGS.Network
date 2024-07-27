using Microsoft.Extensions.DependencyInjection;
using OpenNGSCommon;
using OpenNGS.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Systems;
using UnityEngine;
using OpenNGS;

/// <summary>
/// GameContext: dealing with different Levels in different game stage.
/// </summary>
/// 
public enum  GameContextType
{
    BootStrap,
    Login,
    World,
}

public abstract class GameContext : IApplicationContext
{
    public GameMode GameMode { get; private set; }

    SystemBuilder builder = new SystemBuilder();
    private Dictionary<string, IGameSubSystem> _systems = new Dictionary<string, IGameSubSystem>();

    public void Init(GameMode gameMode)
    {
        Debug.Log($"GameContext Init : {_systems.Count}");

        GameMode = gameMode;
        OnInit();

        builder.InitApplication(this);


        int nCounts = builder.container.Count;
        for( int i = 0; i < nCounts; i++ )
        {
            if( builder.container[i].ImplementationInstance is IGameSubSystem)
            {
                IGameSubSystem _subSys = (IGameSubSystem)(builder.container[i].ImplementationInstance);
                _subSys.Init();
            }
        }
    }

    protected virtual void OnInit()
    {

    }

    public void RegisterSystem(IGameSubSystem system)
    {
        if (system.GetSystemName() == "") return;
        if (this._systems.TryGetValue(system.GetSystemName(), out var sys))
        {
            Debug.LogError(string.Format("System {0}[{1}] ID repetition with {2}", system.ToString(), system.GetSystemName(), sys.ToString()));
            return;
        }
        this._systems.Add(system.GetSystemName(), system);
    }
    public void Clear()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.Clear();
        }
    }
    public void OnPlayerLogin()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.OnPlayerLogin();
        }
    }
    public void OnPlayerLogout()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.OnPlayerLogout();
        }
    }

    public void OnSceneLoaded()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.OnSceneLoaded();
        }
    }

    public void OnEnter()
    {

    }
    public void OnExit()
    {

    }

    public void OnStatus(OpenNGSCommon.StatusData status)
    {
        IGameSubSystem system;
        if (this._systems.TryGetValue(status.SystemName, out system))
        {
            system.OnStatus(status);
        }
        else
        {
            Debug.LogErrorFormat("SystemName:{0} not found for OnStatus", status.SystemName);
        }
    }

    internal void OnWorldEnter()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.OnWorldEnter();
        }
    }

    internal void OnWorldLeave()
    {
        foreach (var sys in this._systems)
        {
            sys.Value.OnWorldLeave();
        }
    }

    public abstract void ConfigureServices(IServiceCollection services);
    public abstract void Configure(IApplicationBuilder app);
}
