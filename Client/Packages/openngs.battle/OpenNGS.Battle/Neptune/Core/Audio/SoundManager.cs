using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 播放音效 SoundManager.Instance.PlaySound ("COCO_attack");
/// 播放音乐 SoundManager.Instance.PlayBGM ("win");
/// </summary>
public class SoundManager : BehaviourSingleton<SoundManager>
{

#if WWISE_AUDIO
    public float distance = 16;
#else
	public float distance = 21;
#endif
    AudioSystem audioSystem;
    IAudioEngine AudioEngine = null;
    private bool isReady = false;
    private string currentBgm;

    public bool BGMMute { get { return this.setting.bgmMute; } }//获得音乐静音状态
    public bool SoundMute { get { return this.setting.soundMute; } }//获得音效静音状态

    private AudioSetting setting;


    /// <summary>
    /// 音乐音量
    /// </summary>
    public int MusicVolume
    {
        get { return this.setting.musicVolume; }
        set
        {
            this.setting.musicVolume = value;
            if (this.AudioEngine != null)
                this.AudioEngine.SetVolume(SoundType.BGM, value);
        }
    }
    /// <summary>
    /// 音效音量
    /// </summary>
    public int SoundVolume
    {
        get { return this.setting.soundVolume; }
        set
        {
            this.setting.soundVolume = value;
            if (this.AudioEngine != null)
                this.AudioEngine.SetVolume(SoundType.SFX, value);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize(IAudioEngine audioEngine, AudioSetting setting)
    {
        if (isReady)
        {
            return;
        }
        this.AudioEngine = audioEngine;
        this.audioSystem = AudioEngine.audioSystem;

        DontDestroyOnLoad(this.gameObject);

        this.setting = setting;

        if (this.AudioEngine != null)
        {
            this.AudioEngine.Initialize(this.gameObject);
            // set init volume
            this.AudioEngine.SetVolume(SoundType.BGM, setting.musicVolume);
            this.AudioEngine.SetVolume(SoundType.SFX, setting.soundVolume);

        }

        isReady = true;
        if (this.currentBgm != null)
            this.PlayBGM(currentBgm);
    }


    public void SetIsPlayer(GameObject obj, bool isPlayer)
    {
        if (this.AudioEngine != null)
            this.AudioEngine.SetIsPlayer(obj, isPlayer);
    }


    /// <summary>
    /// 播放UI音效
    /// </summary>
    /// <param name="clipName">音效名</param>
    public void PlayUISound(string clipName)
    {
        PlaySound("Sound/UI/" + clipName);
    }

    /// <summary>
    /// 播放音效2D
    /// </summary>
    /// <param name="name">Name.</param>
    public void PlaySound(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (this.setting.soundMute)
            return;

        if (this.AudioEngine != null)
            this.AudioEngine.PlaySound(name);
    }

    public void PlaySoundVO(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (this.setting.soundMute)
            return;

        if (this.AudioEngine != null)
            this.AudioEngine.PlaySoundVO(name);
    }
    public void PlaySoundNewbieVO(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (this.setting.soundMute)
            return;

        if (this.AudioEngine != null)
            this.AudioEngine.PlaySoundNewbieVO(name);
    }

    /// <summary>
    /// 在指定游戏对象上播放音效
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <summary>
    public AudioSource PlaySound(IAudioSource aduioSource, string sourceName, SoundType type)
    {
        if (this.setting.soundMute)
            return null;

        if (aduioSource != null && this.AudioEngine != null)
        {
            return this.AudioEngine.PlaySound(aduioSource, sourceName, type);
        }
        return null;
    }


    /// <summary>
    /// 在指定游戏对象上播放人物说话声音
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sourceName"></param>
    public AudioSource PlayVoiceOnGameObject(string sourceName, IAudioSource audio = null)
    {
        if (!this.isReady || this.AudioEngine == null)
            return null;

        if (this.setting.soundMute)
            return null;

        return this.AudioEngine.PlayVoiceOnGameObject(sourceName, audio);
    }

    /// <summary>
    /// 播放音乐2D
    /// </summary>
    /// <param name="name">Name.</param>
    public void PlayBGM(string name, bool isLoop = true, bool fromFile = false)
    {
        if (name.Equals("bgm_main_union"))
        {
            fromFile = true;
        }
        if (!this.isReady || (this.currentBgm == name && isLoop) || this.AudioEngine == null)
            return;

        this.currentBgm = name;
        this.AudioEngine.PlayBGM(name, isLoop, fromFile);
    }

    /// <summary>
    /// 停止播放当前背景音乐
    /// </summary>
    public void StopBGM()
    {
        this.AudioEngine.StopBGM();
    }

    /// <summary>
    /// UI类通知 SoundManager 需要切换音乐静音状态
    /// </summary>
    public void SwitchBGMMute(bool isBool)//切换音乐静音状态
    {
        this.setting.bgmMute = isBool;
        this.AudioEngine.Mute(SoundType.BGM, isBool);
    }

    /// <summary>
    /// UI类通知 SoundManager 需要切换音效静音状态
    /// </summary>
    public void SwitchSoundMute(bool isBool)//切换音乐静音状态
    {
        this.setting.soundMute = isBool;
        this.AudioEngine.Mute(SoundType.SFX, isBool);
    }

    /// <summary>
    /// 停止对象播放的指定音效
    /// </summary>
    /// <param name="aduioSource"></param>
    /// <param name="sourceName"></param>
    public void StopPlay(IAudioSource aduioSource, string sourceName)
    {
        if (this.setting.soundMute)
            return;

        if (aduioSource != null)
        {
            this.AudioEngine.StopPlay(aduioSource, sourceName);
        }
        return;
    }

    /// <summary>
    /// 停止对象播放的所有音效 
    /// </summary>
    /// <param name="aduioSource"></param>
    public void StopPlayAll(IAudioSource aduioSource, SoundType type)
    {
        if (this.setting.soundMute)
            return;

        if (aduioSource != null)
        {
            this.AudioEngine.StopPlayAll(aduioSource, type);
        }
        return;
    }

    /// <summary>
    /// 停止所有播放的2D音效
    /// </summary>
    public void StopAllSound()
    {
        this.AudioEngine.StopAllSound();
    }

    public void PauseSoundOnGameObject()
    {
        //SetAudioMixerDB(SoundType.SFX, -80);
    }

    /// <summary>
    /// 针对3D音效的继续播放，战斗继续时调用
    /// </summary>
    public void ResumeSoundOnGameObject()
    {

    }




    void Update()
    {
        if (Camera.main != null)
        {
            this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        }
    }

    void Start()
    {
        Debug.Log("SoundManager Start");
        if (this.AudioEngine != null)
        {
            this.AudioEngine.Start();
        }
    }

    void FixedUpdate()
    {
        if (this.AudioEngine != null && this.audioSystem == AudioSystem.UnityAudio)
        {
            this.AudioEngine.Update();
        }
    }

    public override void Destroy()
    {
        StopBGM();
        base.Destroy();
        isReady = false;
    }

    /// <summary>
    /// 设置音效语言
    /// </summary>
    /// <param name="lang"></param>
    public void SetLanguage(string lang)
    {
        this.AudioEngine.SetLanguage(lang);
    }
}
