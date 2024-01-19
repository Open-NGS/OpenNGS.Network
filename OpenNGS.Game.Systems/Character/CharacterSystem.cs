using OpenNGSCommon;
using Rpc.Naming;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
namespace OpenNGS.Systems
{
    class CharacterSystem : EntitySystem
    {
        public readonly Dictionary<ulong, Character> CharacterDic = new Dictionary<ulong, Character>();
        public readonly List<string> cachedRandomNames = new List<string>();

        public Character GetCharacter(ulong uin)
        {
            CharacterDic.TryGetValue(uin, out var character);
            return character;
        }

        public async Task<OpenNGS.Character.Common.GetCharacterRsp> GetCharacterInfo(ulong uin)
        {
            //todolist
            var rsp = new OpenNGS.Character.Common.GetCharacterRsp();
            OnGetCharacterRsp(uin, rsp);
            return rsp;
        }

        private void RefreshCharacter(ulong uin, OpenNGS.Character.Common.CharacterInfo info)
        {
            if (CharacterDic.TryGetValue(uin, out var c))
            {
                c.RefreshCharacter(info);
            }
            else
            {
                CharacterDic.Add(uin, new Character(info));
            }
        }

        #region S2C
        private void OnGetCharacterRsp(ulong uin, OpenNGS.Character.Common.GetCharacterRsp rsp)
        {
            if (rsp?.info == null)
            {
                return;
            }

            RefreshCharacter(uin, rsp.info);
        }
        #endregion
    }
}