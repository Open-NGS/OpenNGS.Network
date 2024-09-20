using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IAudioSource
{
    AudioSource GetAudioSource(SoundType type);
    GameObject gameObject { get; }
}