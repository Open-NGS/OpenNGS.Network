using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.Datas;
// 
// Data Management Module V1.0 - mailto:Ray@RayMix.net
// 

public class EngineDataManager: IEngineDataProvider
{
    public DictDataGetter<int, AbilityData> Abilities = new DictDataGetter<int, AbilityData>("Data/Ability");
    public DictDataGetter<int, MarkData> Marks = new DictDataGetter<int, MarkData>("Data/Mark");
    public DictDataGetter<int, Dictionary<int, CombatConfigData>> CombatConfigs = new DictDataGetter<int, Dictionary<int, CombatConfigData>>("Data/CombatConfig");
    public DictDataGetter<string, ModelData> Models = new DictDataGetter<string, ModelData>("Data/Model");
    public DictDataGetter<int, Dictionary<int, SkillData>> Skills = new DictDataGetter<int, Dictionary<int, SkillData>>("Data/Skill");
    public DictDataGetter<int, Dictionary<int, SkillGroupData>> SkillGroups = new DictDataGetter<int, Dictionary<int, SkillGroupData>>("Data/SkillGroup");
    public DictDataGetter<int, LevelData> Levels = new DictDataGetter<int, LevelData>("Data/Levels");
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
    public DictDataGetter<int, Dictionary<int, SkillData>> SkillLevels = new DictDataGetter<int, Dictionary<int, SkillData>>("Data/SkillLevels");
    public DictDataGetter<int, TrapData> Traps = new DictDataGetter<int, TrapData>("Data/Trap");
   
    /// <summary>
    /// 获取关卡数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public LevelData GetLevelData(int id)
    {
        if (!this.Levels.Value.ContainsKey(id))
            return null;
        return this.Levels.Value[id];
    }

    /// <summary>
    /// 获取Skin数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public RoleSkinData GetRoleSkinData(int id, string key)
    {
        if (!this.RoleSkinDatas.Value.ContainsKey(id))
            return null;

        return this.RoleSkinDatas.Value[id][key];
    }

    public TrapData GetTrapData(int id)
    {
        if (!this.Traps.Value.ContainsKey(id))
            return null;

        return this.Traps.Value[id];
    }

    public AbilityData GetAbilityData(int id)
    {
        if (!this.Abilities.Value.ContainsKey(id))
            return null;

        return this.Abilities.Value[id];
    }

    public SummonData GetSummonData(int id, int level)
    {
        if (!this.Summons.Value.ContainsKey(id))
            return null;

        if (!this.Summons.Value[id].ContainsKey(level))
            return null;

        return this.Summons.Value[id][level];
    }

    public string GetEffects(int id)
    {
        if (!this.Effects.Value.ContainsKey(id))
            return null;

        return this.Effects.Value[id];
    }

    public Dictionary<int, CombatConfigData> GetCombatConfigDatas(int id)
    {
        if (!this.CombatConfigs.Value.ContainsKey(id))
            return null;

        return this.CombatConfigs.Value[id];
    }

    public Dictionary<int, SkillGroupData> GetSkillGroups(int id)
    {
        if (!this.SkillGroups.Value.ContainsKey(id))
            return null;

        return this.SkillGroups.Value[id];
    }

    public RoleData GetRoleData(int id)
    {
        if (!this.Roles.Value.ContainsKey(id))
            return null;

        return this.Roles.Value[id];
    }

    public Dictionary<int, RoleGrowthData> GetRoleGrowthDatas(int id)
    {
        if (!this.RoleGrowthDatas.Value.ContainsKey(id))
            return null;
        return this.RoleGrowthDatas.Value[id];
    }

    public SkillData GetSkillDatas(int id)
    {
        if (!this.Skills.Value.ContainsKey(id))
            return null;

        return this.Skills.Value[id][0];
    }

    public Dictionary<int, SkillData> GetSkillLevelDatas(int id)
    {
        if (!this.SkillLevels.Value.ContainsKey(id))
            return null;

        return this.SkillLevels.Value[id];
    }

    public LegendAttributeData GetLegendAttributeData(int id)
    {
        return this.LegendAttributes.Value[id];
    }

    public RoleGearData GetRoleGearData(int id, int level)
    {
        if (!this.RoleGears.Value.ContainsKey(id))
            return null;

        if (!this.RoleGears.Value[id].ContainsKey(level))
            return null;

        return this.RoleGears.Value[id][level];
    }

    public Dictionary<int, RoleQualityData> GetRoleQualityDatas(int id)
    {
        if (!this.RoleQualitys.Value.ContainsKey(id))
            return null;

        return this.RoleQualitys.Value[id];
    }

    public ModelData GetModelData(string name)
    {
        if (!this.Models.Value.ContainsKey(name))
            return null;

        return this.Models.Value[name];
    }

    public Dictionary<string, AnimationConfigData> GetAnimationConfigDatas(string key)
    {
        if (!this.AniTimeConfig.Value.ContainsKey(key))
            return null;

        return this.AniTimeConfig.Value[key];
    }

    public TransformData GetTransformData(int id, int index)
    {
        if (!this.Transforms.Value.ContainsKey(id))
            return null;
        if (!this.Transforms.Value[id].ContainsKey(index))
            return null;
        return this.Transforms.Value[id][index];
    }


    public Dictionary<int, TransformData> GetTransformDatas(int id)
    {
        if (!this.Transforms.Value.ContainsKey(id))
            return null;
        return this.Transforms.Value[id];
    }

    public MarkData GetMarkData(int id)
    {
        if (!this.Marks.Value.ContainsKey(id))
            return null;

        return this.Marks.Value[id];
    }

    public Dictionary<string, Dictionary<float, AniEventData>> GetAniEvents(string key)
    {
        if (!this.AniEvents.Value.ContainsKey(key))
            return null;

        return this.AniEvents.Value[key];
    }

    public SkillModifyData GetSkillModifyData(int id)
    {
        return null;
    }

    public List< SkillModifyGroupData> GetSkillModifyGroupData(uint id)
    {
        return null;
    }

    public List<SkillModifyData> GetSkillModifyDataLst()
    {
        return null;
    }

    public List<SkillModifyGroupData> GetSkillModifyGroupDataLst()
    {
        return null;
    }
}
