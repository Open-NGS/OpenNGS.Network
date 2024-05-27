using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS.Audio
{
    public abstract class AudioDriver
    {

        public virtual void Play<T>(T audio, GameObject obj)
        {
            
        }

        public virtual void Play(string sound, GameObject obj)
        {

        }

        public virtual bool IsPlaying<T>(T audio, GameObject obj)
        {
            return false;
        }

        public virtual void Stop( GameObject obj,string sound = null)
        {
            
        }

        public virtual void Pause( GameObject obj, string sound =null)
        {
            
        }

        public virtual void Resume(GameObject obj, string sound = null )
        {
            
        }

        public virtual void SetVolume(GameObject obj, float volume)
        {
            
        }

        public virtual void LoadPackage(string packageName)
        {
            
        }


        public virtual void UnloadPackage(string packageName)
        {

        }

        public virtual void SetParam(GameObject obj, string paramName, float paramValue)
        {
            
        }

        public virtual void PostEvent(string evtName, GameObject obj)
        {
            
        }

        public virtual void PostEvent<T>(T audio, string evtName, GameObject obj)
        {
            
        }
        
        public virtual void PlayAt<T>(T audio, Vector3 position,GameObject parent = null)
        {
           
        }

        public virtual void PlayAt(string evtName, Vector3 position,GameObject parent = null)
        {
           
        }

        public virtual void PlayMusic(string strMusicName, GameObject gameObject)
        {

        }
        public virtual void StopMusic(string strMusicName, GameObject gameObject)
        {

        }

    }

}
