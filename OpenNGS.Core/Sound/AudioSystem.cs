using System;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.Pool;
using UnityEngine;


namespace OpenNGS.Audio
{
    public class AudioSystem : Singleton<AudioSystem>
    {
        private bool m_Mute;
        private const int POSITION_GAMEOBJECT_POOL = 30;

        public bool Mute
        {
            get { return m_Mute; }
            private set
            {
                if (m_Mute != value)
                {
                    m_Mute = value;
                }
            }
        }

        private AudioDriver m_AudioEngine;

        public void Initialize(AudioDriver audioEngine)
        {
            m_AudioEngine = audioEngine;
        }

        public void Clear()
        {
            m_AudioEngine = null;
        }

        public void SoundMute(bool mute)
        {
            Mute = mute;
        }

        public uint Play(string name, GameObject obj = null)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return 0;
            }
            return m_AudioEngine.Play(name, obj);
        }

        public bool IsPlaying<T>(T audio, GameObject obj)
        {
            return m_AudioEngine.IsPlaying(audio, obj);
        }

        public void PlayAtPoint<T>(T audio, Vector3 pos,GameObject parent = null)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.PlayAt(audio, pos,parent);
        }

        public void SetParam(GameObject obj, string paramName, float paramValue)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.SetParam(obj, paramName,paramValue);
        }


        public void Play<T>(T audio, GameObject obj )
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.Play(audio,obj);
        }

        public void PostEvent(string evtName, GameObject obj)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.PostEvent(evtName,obj);
        }

        public void PostEvent<T>(T audio, string evtName, GameObject obj)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.PostEvent(audio,evtName,obj);
        }

        public void Stop(GameObject obj, string sound = null)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.Stop(obj,sound);
        }

        public void SetVolume(GameObject obj, float volume)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.SetVolume(obj,volume);
        }

        public void Pause(GameObject obj)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.Pause(obj);
        }
        
        public void Resume(GameObject obj)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.Resume(obj);
        }


        public virtual void PlayMusic(string strMusicName, GameObject gameObject)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.PlayMusic(strMusicName, gameObject);
        }
        public virtual void StopMusic(string strMusicName, GameObject gameObject)
        {
            if (m_AudioEngine == null)
            {
                Debug.LogWarning("Sound System Not Initialized.");
                return;
            }
            m_AudioEngine.StopMusic(strMusicName, gameObject);
        }

    }

}
