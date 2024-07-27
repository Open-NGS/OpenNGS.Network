using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OpenNGS.Audio;
using OpenNGS.Pool;
using UnityEngine;

namespace Sound
{
    public class UnityAudioPositionObject : PoolObject<UnityAudioPositionObject>
    {
        public AudioSource audioSource;
        public GameObject gameObject = new GameObject();

        public override void Create()
        {
            base.Create();
            audioSource = gameObject.GetOrAddComponent<AudioSource>();
        }

        public  void PlayAt<T>(T audio, Vector3 position)
        {
            if (!(audio is AudioClip))
            {
                Debug.LogError($"{typeof(T)} 不是AudioClip类型");
                return;
            }
            gameObject.transform.position = position;
            audioSource.clip = audio as AudioClip;
            audioSource.Play();
            float time = GetAudioClipLength(audioSource.clip);
            OpenNGS.Timer.Timer.Start(time, Delete, true);
        }

        float GetAudioClipLength(AudioClip audioClip)
        {
           return audioClip.length * ((double) Time.timeScale < 0.00999999977648258 ? 0.01f : Time.timeScale);
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}

