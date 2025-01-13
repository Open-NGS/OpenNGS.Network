using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IAudioEngine
{
    AudioSystem audioSystem { get; }

    void Initialize(GameObject root);
    void Start();

    void PlayBGM(string name, bool isLoop = true, bool fromFile = false);
    void StopBGM();

    void Mute(SoundType type, bool mute);
    void SetVolume(SoundType type, int value);

    void PlaySound(string name);
    void PlaySoundVO(string name);
    void PlaySoundNewbieVO(string name);

    void SetIsPlayer(GameObject obj, bool isPlayer);

    AudioSource PlaySound(IAudioSource aduioSource, string sourceName, SoundType type);
    AudioSource PlayVoiceOnGameObject(string sourceName, IAudioSource audio = null);

    void StopPlay(IAudioSource aduioSource, string sourceName);
    void StopPlayAll(IAudioSource aduioSource, SoundType type);

    void StopAllSound();

    void Update();

    /// <summary>
    /// 设置当前音效语言
    /// </summary>
    /// <param name="lang">对应语言</param>
    void SetLanguage(string lang);
}
