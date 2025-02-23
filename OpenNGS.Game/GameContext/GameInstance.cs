using System.Collections.Generic;
using Systems;
using Common;
using UnityEngine;
using OpenNGS.IO;
using OpenNGS.Logs.Appenders;
using OpenNGS.Localize;
using OpenNGSCommon;
using OpenNGS.Assets;
using StatusData = OpenNGSCommon.StatusData;
using System;

/// <summary>
/// GameInstance is a singleton manager for an instance of the running game.
/// </summary>
public class GameInstance : OpenNGS.Singleton<GameInstance>
{
    private GameMode _gamemode;
    private GameContext _currentGameContext;
    private Dictionary<int, GameContext> _gameContexts = new Dictionary<int, GameContext>();
    public GameMode Gamemode { get => _gamemode; private set => _gamemode = value; }
    public GameContext CurrentGameContext { get => _currentGameContext; private set => _currentGameContext = value; }

    public void Init()
    {
        Debug.Log("GameInstance:Initialize() - Start");

        OpenNGS.Globalization.Culture.SetUserDefaultCulture(System.Globalization.CultureInfo.InvariantCulture);

#if UNITY_EDITOR
        FileSystem.Init(null, new UnityPathProvider());

#elif UNITY_ANDROID
        FileSystem.Init(null, new UnityAndroidPathProvider());

#elif UNITY_IOS
        Debug.LogError("Game:Initialize() should Create UnitIOSPathProvider");
        FileSystem.Init(null, new UnityAndroidPathProvider());
        
#else

        FileSystem.Init(null, new UnityPathProvider());
#endif

        _gameContexts.Clear();
       //临时解决Config加载问题 屏蔽config加载保存
        if (GameConfig.Config == null)
        {
            if (!GameConfig.Read("Config.txt"))
            {
                GameConfig.Write("Config.txt");
            }
        }

        OpenNGS.Logs.LogSystem.Init(GameConfig.Config, new BaseAppender[] { new UnityAppender(), new FileAppender("NormalLog"), new FileAppender("BattleLogger"), new FileAppender("ProfilerLog") });

        OpenNGS.Profiling.ProfilerLog.Start("GameInstance.Init");

#if UNITY_EDITOR
        if (!AssetLoader.RawMode)
        {
            AssetBundleManager.Instance.Init();
        }
#else
        AssetBundleManager.Instance.Init();
#endif
        StatusSystem.Instance.Init(NetworkModule.Instance);

        //
        LocalizationSystem.Instance.Init();
        SetGameMode(0);

        Application.onBeforeRender += OnBeforeRender;

        OpenNGS.Profiling.ProfilerLog.End("GameInstance.Init");
    }

    private void OnBeforeRender()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif
        StatusSystem.Instance.Update();
    }

    public void Clear()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.Clear();
        }
    }

    public void OnPlayerLogin()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnPlayerLogin();
        }
    }

    public void OnWorldEnter()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnWorldEnter();
        }
    }
    public void OnWorldLeave()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnWorldLeave();
        }
    }

    public void OnPlayerLogout()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnPlayerLogout();
        }
    }

    public void OnSceneLoaded()
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnSceneLoaded();
        }
    }

    public void OnStatus(StatusData status)
    {
        foreach (var context in _gameContexts.Values)
        {
            context.OnStatus(status);
        }
    }

    public void SetGameMode(int severGameModeID)
    {
        Gamemode = new DefaultGameMode();
    }

    public GameMode GetGameMode()
    {
        return Gamemode;
    }

    public void InitGameContext(GameContextType contextID, GameContext context)
    {
        OpenNGS.Profiling.ProfilerLog.Start("GameInstance.InitGameContext:" + contextID);
        if (!_gameContexts.TryGetValue((int)contextID, out var gameContext))
        {
            gameContext = context;
            _gameContexts.Add((int)contextID, gameContext);
            gameContext.Init(Gamemode);
        }
        else
        {
            Debug.LogError($"GameContext {contextID} is already initialized.");
        }
        OpenNGS.Profiling.ProfilerLog.End("GameInstance.InitGameContext:" + contextID);
    }

    public void ChangeGameContext(GameContextType contextID)
    {
        if (_currentGameContext != null)
        {
            _currentGameContext.OnExit();
        }
        var gameContext = GetGameContext(contextID);
        if (gameContext != null)
        {
            gameContext.OnEnter();
        }
        CurrentGameContext = gameContext;
    }

    public GameContext GetGameContext(GameContextType contextID)
    {
        _gameContexts.TryGetValue((int)contextID, out var gameContext);
        return gameContext;
    }
}
