using System;
using OpenNGSCommon;

namespace Systems
{


    public interface IGameSubSystem
    {
        GameMode GameMode { get; }
        GameContextType GetGameContextType();

        void Init();

        void OnPlayerLogin();

        void OnPlayerLogout();

        void OnSceneLoaded();

        void OnWorldEnter();
        void OnWorldLeave();

        void Clear();
        void OnStatus(OpenNGSCommon.StatusData status);

        string GetSystemName();
    }

    public abstract class GameSystemBase<T> : OpenNGS.Singleton<T>, IGameSubSystem where T : GameSystemBase<T>, new()
    {
        public GameSystemBase()
        {
            instance = this as T;
        }

        public GameMode GameMode { get; private set; }

        public abstract GameContextType GetGameContextType();

        public abstract string GetSystemName();

        public void Clear()
        {
            StatusSystem.Instance.UnRegisterSystem(GetSystemName());
            OnClear();
        }
        
        protected virtual void OnClear() { }
        
        public virtual void Init() {
            RegisterStatus();
            OnCreate();
        }

        public virtual void OnPlayerLogin()
        {
        }
        
        public virtual void OnSceneLoaded() { }
        public virtual void OnPlayerLogout() { }

        public virtual void OnWorldEnter() { }
        public virtual void OnWorldLeave() { }

        public static void Create()
        {
            //Debug.Log($"{(Common.SYSTEM_ID)systemId} Create");
            //Instance.SystemID = systemId;
      
            Instance.RegisterStatus();
            
            Instance.OnCreate();
        }

        private void RegisterStatus()
        {
            StatusSystem.Instance.Register(GetSystemName(), OnStatus);
        }

        public virtual void OnStatus(OpenNGSCommon.StatusData status)
        {
            
        }

        protected virtual void OnCreate()
        {
            var gameContext = GameInstance.Instance.GetGameContext(GetGameContextType());
            GameMode = gameContext.GameMode;
            gameContext.RegisterSystem(this);
        }
    }
}
