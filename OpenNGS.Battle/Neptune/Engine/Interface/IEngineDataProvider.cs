using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;

/// <summary>
/// Battle engine data provider
/// </summary>
public interface IEngineDataProvider
{
    /// <summary>
    /// 获取关卡数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    LevelData GetLevelData(int id);

    /// <summary>
    /// 获取Skin数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    RoleSkinData GetRoleSkinData(int id, string key);

    /// <summary>
    /// 获取陷阱数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    TrapData GetTrapData(int id);

    MarkData GetMarkData(int id);

    /// <summary>
    /// 获取 Ability 数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    AbilityData GetAbilityData(int id);

    SummonData GetSummonData(int id,int level);

    /// <summary>
    /// 获取特效
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    string GetEffects(int id);

    Dictionary<int, CombatConfigData> GetCombatConfigDatas(int id);


    Dictionary<int, SkillGroupData> GetSkillGroups(int id);

    RoleData GetRoleData(int id);

    Dictionary<int, RoleGrowthData> GetRoleGrowthDatas(int id);

    SkillData GetSkillDatas(int id);

    List<SkillModifyData> GetSkillModifyDataLst();

    List<SkillModifyGroupData> GetSkillModifyGroupDataLst();

    SkillModifyData GetSkillModifyData(int id);

    List <SkillModifyGroupData> GetSkillModifyGroupData(uint id);

    Dictionary<int, SkillData> GetSkillLevelDatas(int id);

    LegendAttributeData GetLegendAttributeData(int id);


    RoleGearData GetRoleGearData(int id, int level);

    Dictionary<int, RoleQualityData> GetRoleQualityDatas(int id);

    ModelData GetModelData(string name);

    Dictionary<string, AnimationConfigData> GetAnimationConfigDatas(string key);
    Dictionary<string, Dictionary<float, AniEventData>> GetAniEvents(string key);

    /// <summary>
    /// Get a transform data by id and index
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="index">索引</param>
    /// <returns></returns>
    TransformData GetTransformData(int id, int index);

    /// <summary>
    /// Get all transform datas by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Dictionary<int, TransformData> GetTransformDatas(int id);
}