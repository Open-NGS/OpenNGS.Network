using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// AudioSystem Type
/// </summary>
public enum AudioSystem
{
    /// <summary>
    /// Unity Audio System
    /// </summary>
    UnityAudio = 0,
    /// <summary>
    /// WWise Audio System
    /// </summary>
    WWiseAudio
}

/// <summary>
/// Audio Setting
/// </summary>
public struct AudioSetting
{
    /// <summary>
    /// BGM Mute
    /// </summary>
    public bool bgmMute;

    /// <summary>
    /// Sound Mute
    /// </summary>
    public bool soundMute;

    /// <summary>
    /// Music Volume (0 - 100)
    /// </summary>
    public int musicVolume;

    /// <summary>
    /// Sound Volume (0 - 100)
    /// </summary>
    public int soundVolume;
}