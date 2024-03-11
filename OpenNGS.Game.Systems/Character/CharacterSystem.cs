using OpenNGS.Character.Common;
using OpenNGSCommon;
using System.Collections.Generic;
using Systems;
namespace OpenNGS.Systems
{
    public class CharacterSystem : GameSubSystem<CharacterSystem>, ICharacterSystem
    {
        public readonly Dictionary<ulong, Character> CharacterDic = new Dictionary<ulong, Character>();

        private readonly Dictionary<ulong, OpenNGS.Character.Common.CharacterInfo> m_dicChar = new Dictionary<ulong, OpenNGS.Character.Common.CharacterInfo>();
        public readonly List<string> cachedRandomNames = new List<string>();

        private ISaveSystem m_saveSystem;
        protected override void OnCreate()
        {
            m_saveSystem = App.GetService<ISaveSystem>();
            base.OnCreate();
        }
        public Character GetCharacter(ulong uin)
        {
            CharacterDic.TryGetValue(uin, out var character);
            return character;
        }

        public void CreateCharacter(string strCharName)
        {
            ISaveInfo _saveInfo = m_saveSystem.GetFileData("CHARACTER");
            if(_saveInfo != null)
            {
                if(_saveInfo is SaveFileData_Character)
                {
                    SaveFileData_Character myInterface = (SaveFileData_Character)_saveInfo;
                    OpenNGS.Character.Common.CharacterInfo _charInfo = new OpenNGS.Character.Common.CharacterInfo();
                    _charInfo.nickname = strCharName;
                    myInterface.characterInfoArray.items.Add(_charInfo);
                }
            }
            m_saveSystem.SaveFile();
        }

        public void RefreshCharacter()
        {
            ISaveInfo _saveInfo = m_saveSystem.GetFileData("CHARACTER");
            if (_saveInfo != null)
            {
                if (_saveInfo is SaveFileData_Character)
                {
                    SaveFileData_Character _charData = (SaveFileData_Character)_saveInfo;
                    foreach (OpenNGS.Character.Common.CharacterInfo _charInf in _charData.characterInfoArray.items)
                    {
                        if(m_dicChar.ContainsKey(_charInf.uin) == false)
                        {
                            m_dicChar.Add(_charInf.uin, _charInf);
                        }
                    }
                }
            }
                        
        }

        public OpenNGS.Character.Common.CharacterInfo GetCharacterInfo(ulong uin)
        {
            if (m_dicChar.TryGetValue(uin, out var characterInfo))
            {
                return characterInfo;
            }
            return null;
        }
    public override string GetSystemName()
    {
        return "com.openngs.system.CharacterSystem";
    }

    }
}