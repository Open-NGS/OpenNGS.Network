namespace OpenNGS.Setting.Data
{ 
    public partial class SettingContainer
    {
        public void SetVertical(VerticalSynchronization vertical)
        {
            Vertical = vertical;
        }

        public void SetAudio(AudioSettingInfo audio)
        {
            Audio.Add(audio);
        }

        public void SetKeyControl(KeyControlSettingInfo keyControl)
        {
            KeyControl.Add(keyControl);
        }

        public void SetLanguage(Language language)
        {
            Language = language;
        }

        public void SetResolutionRatios(ResolutionRatios ratios)
        {
            ResolutionRatios = ratios;
        }
    }
}
