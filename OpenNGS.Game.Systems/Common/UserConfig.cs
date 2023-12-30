using OpenNGS.Configs;
using OpenNGS.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGSCommon;
using UnityEngine;

public class UserConfig : UserConfig<UserConfig>, IConfig, ILogConfig
{
    public bool LogEnable { get; set; }

    public List<AppenderConfig> LogAppenders { get; set; }

    public  Dictionary<uint, UserSettingValueItem> userSettings { get; set; }

    public static string UserSettingFileName = "UserConfig.txt";
    public void SetDefault()
    {

    }

    public static void Load()
    {
        if (Config == null)
        {
            if (Read(UserSettingFileName))
            {
                Write(UserSettingFileName);
            }
        }
    }

    public static void Save()
    {
        Write(UserSettingFileName);
    }
}
