public class SoundManager:MonoSingleton<SoundManager>
{
    bool musicOn;
    public bool MusicOn
    {
        set
        {
            musicOn = value;

            this.MusicVolumes(musicOn);
        }
    }

    bool soundOn;
    public bool SoundOn
    {
        set
        {
            soundOn = value;
            this.SoundVolumes(soundOn);
        }
    }

    bool overallOn;
    public bool OverallOn
    {
        set
        {
            overallOn = value;
            this.MusicVolumes(overallOn);
            this.SoundVolumes(overallOn);
        }
    
   }
    bool voiceOn;
    public bool VoiceOn
    {
        set
        {
           voiceOn = value;
            
        }
    }



    int musicVolume;
    public int MusicVolume
    {
        set
        {
            musicVolume = value;
            this.SetMusicVolume(musicVolume);
        }
    }

    int soundVolume;
    public int SoundVolume
    {
        set
        {
            soundVolume = value;
            this.SetSoundVolume(soundVolume);
        }
    }

    //public AudioMixer AudioMixer;
    //public AudioSource MusicAduioSource;
    //public AudioSource SoundAduioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    public void PlayMusic(string name)
    {
       
    }
    public void PlaySound(string name)
    {

    }

    void SetMusicVolume(int value)
    {
        float volume = value * 0.5f - 50f;
        
    }

    void SetSoundVolume(int value)
    {
        float volume = value * 0.5f - 50f;
        
    }

    void MusicVolumes(bool value)
    {
        if (!value)
        {
            
        }
        else
        {
            
        }
    }
    void SoundVolumes(bool value)
    {
        if (!value)
        {
            //SoundAduioSource.Stop();
        }
        else
        {
            //SoundAduioSource.Play();
        }
    }
}
