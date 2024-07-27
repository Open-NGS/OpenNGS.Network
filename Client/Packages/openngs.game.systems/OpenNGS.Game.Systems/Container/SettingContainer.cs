namespace OpenNGS.Setting.Data
{ 
    public partial class SettingContainer
    {
        public void SetVertical(VerticalSynchronizationData vertical)
        {
            Vertical = vertical;
        }

        public void SetAudio(AudioSettingData audio)
        {
            if (Audio.Count!=0)
            {
                for (int i = 0; i < Audio.Count; i++)
                {
                    if (Audio[i].AudioName == audio.AudioName)
                    {
                        Audio[i] = audio;
                        return;
                    }
                }
                Audio.Add(audio);
            }
            else
            {
                Audio.Add(audio);
            }
        }

        public void SetKeyControl(KeyControlSettingData keyControl)
        {
            if (KeyControl.Count!=0)
            {
                for (int i = 0; i < KeyControl.Count; i++)
                {
                    if (KeyControl[i].KeyName == keyControl.KeyName)
                    {
                        KeyControl[i] = keyControl;
                        return;
                    }
                }
                KeyControl.Add(keyControl);
            }
            else
            {
                KeyControl.Add(keyControl);
            }
        }

        public void SetLanguage(LanguageData language)
        {
            Language = language;
        }

        public void SetResolutionRatios(ResolutionRatiosData ratios)
        {
            ResolutionRatios = ratios;
        }
    }
}
