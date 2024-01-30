namespace OpenNGS.Systems
{
    public interface ICharacterSystem 
    {
        public void CreateCharacter();
        public void RefreshCharacter();
        OpenNGS.Character.Common.CharacterInfo GetCharacterInfo(ulong uin);
    }

}