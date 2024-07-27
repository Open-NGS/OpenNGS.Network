using OpenNGS.Character.Common;

namespace OpenNGS.Systems
{
    public interface ICharacterSystem 
    {
        public void CreateCharacter(string strName);
        public void RefreshCharacter();
        public CharacterInfo GetCharacterInfo(ulong uin);
    }

}