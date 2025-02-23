using OpenNGS.Configs;
using OpenNGS.Logs;
using OpenNGSCommon;
using System.Collections.Generic;


class GameConfig : AppConfig<GameConfig>, IConfig, ILogConfig
{
    public bool LogEnable { get; set; }

    public List<AppenderConfig> LogAppenders { get; set; }

    private Dictionary<uint, UserSettingValueItem> userSettings = new Dictionary<uint, UserSettingValueItem>();
    
    public void SetDefault()
    {
        this.LogEnable = true;
        this.LogAppenders = new List<AppenderConfig>();
        this.LogAppenders.Add(
            new AppenderConfig()
            {
                Name = "Unity",
                Enable = true,
                Type = "Unity",
                FilterType = LogFilter.FilterType.All,
                FilterTags = LogSystem.Tag
            }
        );

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID
        this.LogAppenders.Add(
            new AppenderConfig()
            {
                Name = "NormalLog",
                Enable = true,
                LogFile = "Log.txt",
                Type = "File",
                Roll = false,
                FilterType = LogFilter.FilterType.All,
                FilterTags = ""
            }
        );
#endif
    }
}