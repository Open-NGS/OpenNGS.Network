using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neptune.Datas;
//using System.Linq;

namespace Neptune
{
    public class RoleSkin
    {



        public int roleSkinID = 0;

        private RoleSkinData roleSkinData = null;

        public RoleSkinData RoleSkinData
        {
            get { return roleSkinData; }
        }

        private string modelName = null;

        public RoleSkin(int ID)
        {
            roleSkinID = ID;
            if (roleSkinID > 0)
            {
                roleSkinData = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, this.GetKey(roleSkinID, (int)RoleSkinKeyType.RoleSkin));
                if (roleSkinData == null)
                {
                    Logger.LogError("Can not find roleSkinData by ID: " + roleSkinID);
                }

            }
        }

        public string GetKey(int id, int keytype)
        {
            string key = id + "_" + keytype;
            return key;
        }

        public bool NeedChangeAnim()
        {
            if (roleSkinID <= 0)
            {
                return false;
            }
            return roleSkinData.Param12 > 0;
        }

        /// <summary>
        /// 获取动作名字相关Key
        /// </summary>
        /// <returns></returns>
        public string GetModelKey(string model)
        {
            string key = model;
            if (roleSkinID <= 0 || string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(model))
                return key;

            if (!NeedChangeAnim() && model.Contains(modelName))
            {
                return modelName;
            }
            return key;
        }


        /// <summary>
        /// 额外属性
        /// </summary>
        /// <param name="attributes"></param>
        public void ExtraAttr(RoleAttributeSet attributes)
        {
            if (roleSkinID <= 0/* || attributes == null*/)
                return;
            if (string.IsNullOrEmpty(this.RoleSkinData.Param2))
                return;
            RoleAttribute attr = (RoleAttribute)Enum.Parse(typeof(RoleAttribute), this.RoleSkinData.Param2);
            attributes[(int)attr] += this.RoleSkinData.Param10;

        }

        public void RoleSkinRoleReplace(RoleData roleData)
        {
            if (roleSkinID <= 0 || roleData == null)
                return;
            string key = this.GetKey(roleData.ID, (int)RoleSkinKeyType.Role);
            modelName = roleData.Model;
            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Param1))
                    roleData.Name = data.Param1;
                if (data.Param13 != null)
                {
                    roleData.VoiceShow.Clear();
                    roleData.VoiceShow.AddRange(data.Param13);
                }
                if (data.Param14 != null)
                {
                    roleData.VoiceMove.Clear();
                    roleData.VoiceMove.AddRange(data.Param14);
                }
                if (data.Param15 != null)
                {
                    roleData.VoiceUI.Clear();
                    roleData.VoiceUI.AddRange(data.Param15);
                }
                if (!string.IsNullOrEmpty(data.Param2))
                    roleData.VoiceUlt = data.Param2;
                if (data.Param16 != null)
                {
                    roleData.VoiceDeath.Clear();
                    roleData.VoiceDeath.AddRange(data.Param16);
                }
                if (data.Param18 != null)
                {
                    roleData.VoiceKill.Clear();
                    roleData.VoiceKill.AddRange(data.Param18);
                }

                if (!string.IsNullOrEmpty(data.Param3))
                    roleData.BirthAction = data.Param3;
            }
            //模型皮肤后缀名
            if (!string.IsNullOrEmpty(roleSkinData.Param1))
                roleData.Model += roleSkinData.Param1;
        }


        public void RoleSkinAbilityReplace(AbilityData abilityData)
        {
            if (roleSkinID <= 0 || abilityData == null)
                return;
            string key = this.GetKey(abilityData.ID, (int)RoleSkinKeyType.Ability);
            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Param1))
                    abilityData.Effect = data.Param1;
                if (data.Param10 != 0)
                    abilityData.RoleFX = data.Param10;
            }
        }

        public void RoleSkinTrapReplace(TrapData trapData)
        {
            if (roleSkinID <= 0 || trapData == null)
                return;
            string key = this.GetKey(trapData.TrapId, (int)RoleSkinKeyType.Trap);
            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Param1))
                    trapData.EffectName = data.Param1;
                if (!string.IsNullOrEmpty(data.Param2))
                    trapData.EndEffect = data.Param2;
                if (!string.IsNullOrEmpty(data.Param3))
                    trapData.EndSound = data.Param3;
                if (!string.IsNullOrEmpty(data.Param4))
                    trapData.StartSound = data.Param4;
                if (!string.IsNullOrEmpty(data.Param5))
                    trapData.TriggerSound = data.Param5;
            }
        }

        public void RoleSkinMarkReplace(MarkData markData)
        {
            if (roleSkinID <= 0 || markData == null)
                return;
            string key = this.GetKey(markData.ID, (int)RoleSkinKeyType.Mark);
            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Param1))
                    markData.Effect = data.Param1;
            }
        }

        public void RoleSkinSkillReplace(SkillData talentData)
        {
            if (roleSkinID <= 0 || talentData == null)
                return;
            string key = this.GetKey(talentData.ID, (int)RoleSkinKeyType.Skill);
            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Param1))
                    talentData.CastingEffect = data.Param1;
                if (!string.IsNullOrEmpty(data.Param2))
                    talentData.HitEffect = data.Param2;
                if (!string.IsNullOrEmpty(data.Param3))
                    talentData.HitSound = data.Param3;
                if (!string.IsNullOrEmpty(data.Param4))
                    talentData.FlyResource = data.Param4;
                if (data.Param10 != 0)
                    talentData.ProtectTime = data.Param10 * NeptuneConst.Hundredth;
                if (!string.IsNullOrEmpty(data.Param5))
                    talentData.DirectEffect = data.Param5;
                if (data.Param11 != 0)
                    talentData.EndEvent = data.Param11 * NeptuneConst.Hundredth;
                if (data.Param13 != null && data.Param13.Count > 0)
                {
                    talentData.PlayVoice.Clear();
                    talentData.PlayVoice.AddRange(data.Param13);
                }
                if (!string.IsNullOrEmpty(data.Param6))
                    talentData.MiniMapEffect = data.Param6;
                if (!string.IsNullOrEmpty(data.Param7))
                    talentData.StartSound = data.Param7;
                if (data.Param17 != null && data.Param17.Count > 0)
                {
                    talentData.ExtraEffects.Clear();
                    talentData.ExtraEffects.AddRange(data.Param17);
                }
            }
        }

        public string RoleSkinEventReplace(string curAnimName, RoleSkinEventType eventType, string name, int index = 0)
        {
            if (roleSkinID <= 0 || string.IsNullOrEmpty(curAnimName))
                return null;
            string key = curAnimName;

            RoleSkinData data = NeptuneBattle.Instance.DataProvider.GetRoleSkinData(roleSkinID, key);
            if (data == null)
            {
                return null;
            }
            if (eventType == RoleSkinEventType.PlaySound)
            {
                if (data.Param14 != null && data.Param14.Count > index)
                {
                    name = data.Param14[index].Replace('|', ',');
                }
            }
            else
            {
                if (data.Param13 != null && data.Param13.Count > index)
                {
                    name = data.Param13[index];
                }
            }

            return name;
        }
    }


}