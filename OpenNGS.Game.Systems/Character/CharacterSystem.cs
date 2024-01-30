using OpenNGS.Character.Common;
using OpenNGS.Rank.Data;
using OpenNGSCommon;
using System.Collections.Generic;
using UnityEngine;
namespace OpenNGS.Systems
{
    class CharacterSystem : EntitySystem, ICharacterSystem
    {
        public readonly Dictionary<ulong, Character> CharacterDic = new Dictionary<ulong, Character>();

        private readonly Dictionary<ulong, OpenNGS.Rank.Data.CharacterInfo> m_dicChar = new Dictionary<ulong, OpenNGS.Rank.Data.CharacterInfo>();
        public readonly List<string> cachedRandomNames = new List<string>();

        private ISaveSystem m_saveSystem;
        public override void InitSystem()
        {
            m_saveSystem = App.GetService<ISaveSystem>();
        }
        public Character GetCharacter(ulong uin)
        {
            CharacterDic.TryGetValue(uin, out var character);
            return character;
        }

        public void CreateCharacter()
        {
            ISaveInfo _saveInfo = m_saveSystem.GetFileData("CHARACTER");
            if(_saveInfo != null)
            {
                if(_saveInfo is CharacterSaveData)
                {
                    CharacterSaveData myInterface = (CharacterSaveData)_saveInfo;
                    myInterface.characterInfoArray.items.Add(new Rank.Data.CharacterInfo());
                }
            }
            m_saveSystem.SaveFile();
        }

        public void RefreshCharacter()
        {
            ISaveInfo _saveInfo = m_saveSystem.GetFileData("CHARACTER");
            if (_saveInfo != null)
            {
                if (_saveInfo is CharacterSaveData)
                {
                    CharacterSaveData _charData = (CharacterSaveData)_saveInfo;
                    foreach (Rank.Data.CharacterInfo _charInf in _charData.characterInfoArray.items)
                    {
                        if(m_dicChar.ContainsKey(_charInf.ID) == false)
                        {
                            m_dicChar.Add(_charInf.ID, _charInf);
                        }
                    }
                }
            }
                        
        }

        public OpenNGS.Rank.Data.CharacterInfo GetCharacterInfo(ulong uin)
        {
            if (m_dicChar.TryGetValue(uin, out var characterInfo))
            {
                return characterInfo;
            }
            return null;
        }
    }
}