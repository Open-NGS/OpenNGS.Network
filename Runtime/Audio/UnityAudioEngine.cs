using System.Collections;
using System.Collections.Generic;
using OpenNGS.Assets;
using OpenNGS.Audio;
using UnityEngine;

namespace Sound
{
    public class UnityAudioEngine : AudioDriver
    {
        public AudioListener AudioListener { get; set; }
        private Transform objParent;
        public UnityAudioEngine(Transform objParent):base()
        {
            this.objParent = objParent;
            UnityAudioPositionObject.InitPool(0);
        }

        public override uint Play(string sound, GameObject obj)
        {
            base.Play(sound, obj);
            AudioClip audioClip = AssetLoader.Load<AudioClip>(sound);
            if (audioClip == null)
            {
                Debug.LogError($"加载音频文件{sound}失败，无法播放");
                return 0;
            }

            return Play(audioClip, obj);
        }

        public override uint Play<T>(T audio, GameObject obj)
        {
            base.Play(audio, obj);
            if (!(audio is AudioClip))
            {
                return 0;
            }

            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource, true))
                return 0;
            audioSource.clip = audio as AudioClip;
            audioSource.Play();
            return (uint)audioSource.GetHashCode();
        }

        public override bool IsPlaying<T>(T audio, GameObject obj)
        {
            base.IsPlaying(audio, obj);

            AudioClip clipToPlay;
            if (!(audio is AudioClip))
            {
                return false;
            }
            else
            {
                clipToPlay = audio as AudioClip;
            }

            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource, false))
                return false;

            if (audioSource.isPlaying && audioSource.clip == clipToPlay)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void PostEvent<T>(T audio, string evtName, GameObject obj)
        {
            base.PostEvent(audio, evtName, obj);
            if (!(audio is AudioClip))
            {
                return;
            }

            switch (evtName)
            {
                case SoundEvent.LOOP_PLAY_EVT:
                    PlayLoop(obj, audio as AudioClip);
                    break;
                case SoundEvent.PLAY_ONE_SHOT:
                    PlayOneShot(obj, audio as AudioClip);
                    break;
            }
        }

        void PlayOneShot(GameObject obj, AudioClip audioClip)
        {
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource, true))
                return;
            audioSource.PlayOneShot(audioClip);
        }

        void PlayLoop(GameObject obj, AudioClip audioClip)
        {
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource, true))
                return;
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        public override void SetVolume(GameObject obj, float volume)
        {
            base.SetVolume(obj, volume);
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource))
                return;
            audioSource.volume = volume;
        }

        public override void Pause(GameObject obj, string sound = null)
        {
            base.Pause(obj, sound);
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource))
                return;
            audioSource.Pause();
        }

        public override void Resume(GameObject obj, string sound = null)
        {
            base.Resume(obj, sound);
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource))
                return;
            audioSource.UnPause();
        }

        public override void Stop(GameObject obj, string sound = null)
        {
            base.Stop(obj, sound);
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource))
                return;
            audioSource.Stop();
        }

        public override void SetParam(GameObject obj, string paramName, float paramValue)
        {
            base.SetParam(obj, paramName, paramValue);
            AudioSource audioSource;
            if (!TryGetAudioSource(obj, out audioSource))
                return;
            switch (paramName)
            {
                case SoundParam.Pitch:
                    SetPitch(audioSource,paramValue);
                    break;
                case SoundParam.Speed:
                    SetSpeed(audioSource, paramValue);

                    break;
            }
        }

        void SetPitch(AudioSource audioSource, float pitch)
        {
            if(audioSource.outputAudioMixerGroup == null || audioSource.outputAudioMixerGroup.audioMixer == null)
                return;
            if (!audioSource.outputAudioMixerGroup.audioMixer.SetFloat(SoundParam.Pitch, pitch))
            {
                Debug.LogError($"audioMixer {audioSource.outputAudioMixerGroup.audioMixer.name} Param {SoundParam.Pitch} ");
            }
        }

        void SetSpeed(AudioSource audioSource, float paramValue)
        {
            if(paramValue == 0)
                return;
            audioSource.pitch = paramValue;
            if(audioSource.outputAudioMixerGroup == null || audioSource.outputAudioMixerGroup.audioMixer == null)
                return;
            if (!audioSource.outputAudioMixerGroup.audioMixer.SetFloat(SoundParam.Pitch, 1 / paramValue))
            {
                Debug.LogError($"audioMixer {audioSource.outputAudioMixerGroup.audioMixer.name} Param {SoundParam.Pitch} ");
            }
        }


        bool TryGetAudioSource(GameObject obj, out AudioSource audioSource, bool autoAdd = false)
        {
            audioSource = null;
            if (obj == null)
                return false;
            audioSource = autoAdd ? obj.GetOrAddComponent<AudioSource>() : obj.GetComponent<AudioSource>();
            return audioSource != null;
        }
        
        public override void PlayAt<T>(T audio, Vector3 position,GameObject parent = null)
        {
            UnityAudioPositionObject obj =   UnityAudioPositionObject.New();
            Transform par = parent == null ? objParent : parent.transform;
            if(par!= null)
                obj.gameObject.transform.SetParent(par);
            obj.PlayAt(audio, position);

        }

        public override void PlayAt(string evtName, Vector3 position,GameObject parent = null)
        {
            UnityAudioPositionObject obj =   UnityAudioPositionObject.New();
            Transform par = parent == null ? objParent : parent.transform;
            if(par!= null)
                obj.gameObject.transform.SetParent(par);
            obj.PlayAt(evtName, position);
        }

        public void Clear()
        {
            objParent = null;
            UnityAudioPositionObject.DeleteAll();
        }

    }

}
