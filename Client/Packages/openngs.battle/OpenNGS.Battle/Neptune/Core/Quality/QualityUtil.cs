using UnityEngine;

public class QualityUtil
{
    /*
     * Shader Keywords Description
     *                                                      高质量 中质量 低质量
     * _EFFECT_ON       效果开关     控制角色光影效果           ON    ON    OFF
     * _RIM_ON          Rim效果开关  控制角色 Rim 颜色效果     ON    OFF   OFF
     * _DETAIL_ON       流光效果开关  控制单位流光效果          ON    ON   OFF
     * _EMISSION_ON     自发光开关   控制角色受击闪亮效果       ON    ON    ON
     * 
     * 
     * 
    */

    private const string PerfKeyQuality = "Quality";
    private const string PerfKeyOutline = "Outline";

    private static bool _lockQualityLevel = false;

    public static bool LockQualityLevel
    {
        get { return _lockQualityLevel; }
        set { _lockQualityLevel = value; }
    }

    public static bool LimitMemoryAllocate = false;

    private static int _recommandQualityLevel = -1;

    private static QualityLevel _qualityLevel = QualityLevel.Medium;
    /// <summary>
    /// 获取或设置当前运行的画面质量级别
    /// </summary>
    public static QualityLevel QualityLevel
    {
        get
        {
            return _qualityLevel;
//            return (QualityLevel)QualitySettings.GetQualityLevel();
        }
    }

    public static bool SetQualityLevel(QualityLevel level)
    {
        if (LockQualityLevel && _recommandQualityLevel >= 0 && level > (QualityLevel)_recommandQualityLevel)
        {
            //PopupManager.Instance.ShowTextTips("KEY.34469");
            return false;
        }
        QualitySettings.SetQualityLevel((int)level, false);
        ConfigLevel = level;
        _qualityLevel = level;
        return true;
    }

    public static void SetRecommandQualityLevel(QualityLevel quality)
    {
        Debug.Log("Recommand Quality Level: " + quality);
        // 玩家没有手动设置过显示质量，则使用服务器返回的推荐质量；否则使用用户自己的配置
        if (PlayerPrefs.GetInt(PerfKeyQuality, -1) == -1)
        {
            SetQualityLevel(quality);
            Debug.Log("Quality Level Set To: " + QualityLevel);
        }
        else
        {
            Debug.Log("User Has Already Set Quality: " + QualityLevel);
        }
        LockQualityLevel = false;
    }

    public static void SetLockQualityLevel(QualityLevel quality)
    {
        SetQualityLevel(quality);
        _recommandQualityLevel = (int)quality;
        //if (quality == DeviceInfoReply.QUALITY_LEVEL.QUALITY_LEVEL_DEFAULT ||
        //    quality == DeviceInfoReply.QUALITY_LEVEL.QUALITY_LEVEL_HIGH)
        //{
        //    QualityLevel = QualityLevel.High;
        //    _recommandQualityLevel = (int) QualityLevel.High;
        //}
        //else if (quality == DeviceInfoReply.QUALITY_LEVEL.QUALITY_LEVEL_MIDIUM)
        //{
        //    QualityLevel = QualityLevel.Medium;
        //    _recommandQualityLevel = (int) QualityLevel.Medium;
        //}
        //else
        //{
        //    QualityLevel = QualityLevel.Low;
        //    _recommandQualityLevel = (int) QualityLevel.Low;
        //}
        Debug.Log("Already Set Lock Quality Level: " + QualityLevel);
        LockQualityLevel = true;
    }


    /// <summary>
    /// 获取或设置配置的画面质量级别
    /// </summary>
    public static QualityLevel ConfigLevel
    {
        get
        {
            return (QualityLevel)PlayerPrefs.GetInt(PerfKeyQuality, (int)QualityLevel);
        }
        set
        {
            PlayerPrefs.SetInt(PerfKeyQuality, (int)value);
        }
    }

    /// <summary>
    /// 获取或设置是否启用角色描边
    /// </summary>
    public static bool EnableOutline
    {
        get
        {
            return PlayerPrefs.GetInt(PerfKeyOutline, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(PerfKeyOutline, value ? 1 : 0);
            SetOutline(value);
        }
    }


    static void SetOutline(bool enable)
    {
        if (enable)
        {
            Shader.globalMaximumLOD = 800;
        }
        else
        {
            Shader.globalMaximumLOD = 700;
        }
    }

    /// <summary>
    /// 根据配置质量初始化当前显示质量
    /// </summary>
    public static void InitQuality()
    {
        if(ConfigLevel!=QualityLevel)
        {
            SetQualityLevel(ConfigLevel);
        }
    }

    public static string GetLODName()
    {
        string lod = "";
        if (QualityUtil.QualityLevel == QualityLevel.Low)
        {
            lod = "_LOD2";
        }
        return lod;
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.Save();
    }
}

