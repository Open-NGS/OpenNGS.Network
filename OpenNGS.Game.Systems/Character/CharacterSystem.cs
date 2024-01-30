using OpenNGSCommon;
using Rpc.Naming;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
namespace OpenNGS.Systems
{
    class CharacterSystem : EntitySystem, ICharacterSystem
    {
        public readonly Dictionary<ulong, Character> CharacterDic = new Dictionary<ulong, Character>();
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
                    int a = 0;
                }
            }
            m_saveSystem.SaveFile();
        }

        public void RefreshCharacter()
        {
            throw new System.NotImplementedException();
        }

        OpenNGS.Character.Common.CharacterInfo ICharacterSystem.GetCharacterInfo(ulong uin)
        {
            throw new System.NotImplementedException();
        }
    }
}