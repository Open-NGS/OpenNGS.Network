using OpenNGS.Rank.Data;

namespace OpenNGS.Systems
{
    public interface ICharacterSystem 
    {
        public void CreateCharacter();
        public void RefreshCharacter();
        public CharacterInfo GetCharacterInfo(ulong uin);
    }

}