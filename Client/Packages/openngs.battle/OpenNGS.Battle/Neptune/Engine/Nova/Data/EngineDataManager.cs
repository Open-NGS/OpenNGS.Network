using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.GameData;

namespace Neptune
{
    // 
    // Data Management Module V1.0 - mailto:Ray@RayMix.net
    // 

    public class EngineDataManager : IEngineDataProvider
    {
        public DictDataGetter<int, AbilityData> Abilities = new DictDataGetter<int, AbilityData>("Data/Ability");
        public DictDataGetter<int, MarkData> Marks = new DictDataGetter<int, MarkData>("Data/Mark");
        public DictDataGetter<int, Dictionary<int, CombatConfigData>> CombatConfigs = new DictDataGetter<int, Dictionary<int, CombatConfigData>>("Data/CombatConfig");
        public DictDataGetter<string, ModelData> Models = new DictDataGetter<string, ModelData>("Data/Model");
        public DictDataGetter<int, Dictionary<int, TalentData>> Talents = new DictDataGetter<int, Dictionary<int, TalentData>>("Data/Skill");
        public DictDataGetter<int, Dictionary<int, TalentGroupData>> TalentGroups = new DictDataGetter<int, Dictionary<int, TalentGroupData>>("Data/TalentGroup");
        public DictDataGetter<int, BattleLevelData> Levels = new DictDataGetter<int, BattleLevelData>("Data/Levels");
        public DictDataGetter<int, Dictionary<int, TransformData>> Transforms = new DictDataGetter<int, Dictionary<int, TransformData>>("Data/Transform");
        public DictDataGetter<int, RoleData> Roles = new DictDataGetter<int, RoleData>("Data/Actor");
        public DictDataGetter<int, Dictionary<int, RoleQualityData>> RoleQualitys = new DictDataGetter<int, Dictionary<int, RoleQualityData>>("Data/RoleQuality");
        public DictDataGetter<string, Dictionary<string, Dictionary<float, AniEventData>>> AniEvents = new DictDataGetter<string, Dictionary<string, Dictionary<float, AniEventData>>>("Data/AniEvent");
        public DictDataGetter<string, Dictionary<string, AnimationConfigData>> AniTimeConfig = new DictDataGetter<string, Dictionary<string, AnimationConfigData>>("Data/AnimationConfig");
        public DictDataGetter<int, Dictionary<int, RoleGrowthData>> RoleGrowthDatas = new DictDataGetter<int, Dictionary<int, RoleGrowthData>>("Data/RoleGrowth");
        public DictDataGetter<int, Dictionary<string, RoleSkinData>> RoleSkinDatas = new DictDataGetter<int, Dictionary<string, RoleSkinData>>("Data/RoleSkin");

        public DataGetter<Dictionary<int, string>> Effects = new DataGetter<Dictionary<int, string>>("Data/Effect");
        public DictDataGetter<int, LegendAttributeData> LegendAttributes = new DictDataGetter<int, LegendAttributeData>("Data/LegendAttribute");
        public DictDataGetter<int, Dictionary<int, RoleGearData>> RoleGears = new DictDataGetter<int, Dictionary<int, RoleGearData>>("Data/RoleGear");
        public DictDataGetter<int, Dictionary<int, SummonData>> Summons = new DictDataGetter<int, Dictionary<int, SummonData>>("Data/Summon");
        public DictDataGetter<int, Dictionary<int, TalentData>> TalentLevels = new DictDataGetter<int, Dictionary<int, TalentData>>("Data/TalentLevels");
        public DictDataGetter<int, TrapData> Traps = new DictDataGetter<int, TrapData>("Data/Trap");

        /// <summary>
        /// 获取关卡数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BattleLevelData GetLevelData(int id)
        {
            if (!Levels.Value.ContainsKey(id))
                return null;
            return Levels.Value[id];
        }

        /// <summary>
        /// 获取Skin数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public RoleSkinData GetRoleSkinData(int id, string key)
        {
            if (!RoleSkinDatas.Value.ContainsKey(id))
                return null;

            return RoleSkinDatas.Value[id][key];
        }

        public TrapData GetTrapData(int id)
        {
            if (!Traps.Value.ContainsKey(id))
                return null;

            return Traps.Value[id];
        }

        public AbilityData GetAbilityData(int id)
        {
            if (!Abilities.Value.ContainsKey(id))
                return null;

            return Abilities.Value[id];
        }

        public SummonData GetSummonData(int id, int level)
        {
            if (!Summons.Value.ContainsKey(id))
                return null;

            if (!Summons.Value[id].ContainsKey(level))
                return null;

            return Summons.Value[id][level];
        }

        public string GetEffects(int id)
        {
            if (!Effects.Value.ContainsKey(id))
                return null;

            return Effects.Value[id];
        }

        public Dictionary<int, CombatConfigData> GetCombatConfigDatas(int id)
        {
            if (!CombatConfigs.Value.ContainsKey(id))
                return null;

            return CombatConfigs.Value[id];
        }

        public Dictionary<int, TalentGroupData> GetTalentGroups(int id)
        {
            if (!TalentGroups.Value.ContainsKey(id))
                return null;

            return TalentGroups.Value[id];
        }

        public RoleData GetRoleData(int id)
        {
            if (!Roles.Value.ContainsKey(id))
                return null;

            return Roles.Value[id];
        }

        public Dictionary<int, RoleGrowthData> GetRoleGrowthDatas(int id)
        {
            if (!RoleGrowthDatas.Value.ContainsKey(id))
                return null;
            return RoleGrowthDatas.Value[id];
        }

        public Dictionary<int, TalentData> GetTalentDatas(int id)
        {
            if (!Talents.Value.ContainsKey(id))
                return null;

            return Talents.Value[id];
        }

        public Dictionary<int, TalentData> GetTalentLevelDatas(int id)
        {
            if (!TalentLevels.Value.ContainsKey(id))
                return null;

            return TalentLevels.Value[id];
        }

        public LegendAttributeData GetLegendAttributeData(int id)
        {
            return LegendAttributes.Value[id];
        }

        public RoleGearData GetRoleGearData(int id, int level)
        {
            if (!RoleGears.Value.ContainsKey(id))
                return null;

            if (!RoleGears.Value[id].ContainsKey(level))
                return null;

            return RoleGears.Value[id][level];
        }

        public Dictionary<int, RoleQualityData> GetRoleQualityDatas(int id)
        {
            if (!RoleQualitys.Value.ContainsKey(id))
                return null;

            return RoleQualitys.Value[id];
        }

        public ModelData GetModelData(string name)
        {
            if (!Models.Value.ContainsKey(name))
                return null;

            return Models.Value[name];
        }

        public Dictionary<string, AnimationConfigData> GetAnimationConfigDatas(string key)
        {
            if (!AniTimeConfig.Value.ContainsKey(key))
                return null;

            return AniTimeConfig.Value[key];
        }

        public TransformData GetTransformData(int id, int index)
        {
            if (!Transforms.Value.ContainsKey(id))
                return null;
            if (!Transforms.Value[id].ContainsKey(index))
                return null;
            return Transforms.Value[id][index];
        }


        public Dictionary<int, TransformData> GetTransformDatas(int id)
        {
            if (!Transforms.Value.ContainsKey(id))
                return null;
            return Transforms.Value[id];
        }

        public MarkData GetMarkData(int id)
        {
            if (!Marks.Value.ContainsKey(id))
                return null;

            return Marks.Value[id];
        }

        public Dictionary<string, Dictionary<float, AniEventData>> GetAniEvents(string key)
        {
            if (!AniEvents.Value.ContainsKey(key))
                return null;

            return AniEvents.Value[key];
        }
    }
}