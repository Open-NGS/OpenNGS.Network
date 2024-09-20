
using Neptune.Datas;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
namespace Neptune
{
    enum eModifyCategory
    {
        eModifyCategory_none,
        eModifyCategory_Skill,
        eModifyCategory_Ability,
        eModifyCategory_Mark,
        eModifyCategory_Trap,
    }
    enum eModifyType
    {
        eModifyType_none,
        eModifyType_Add,
        eModifyType_Update
    }
    public class SkillModifyManager : Singleton<SkillModifyManager>
    {
        private const int ModifyCategorySkill   = (int)eModifyCategory.eModifyCategory_Skill;
        private const int ModifyCategoryAbility = (int)eModifyCategory.eModifyCategory_Ability;
        private const int ModifyCategoryMark    = (int)eModifyCategory.eModifyCategory_Mark;
        private const int ModifyCategoryTrap    = (int)eModifyCategory.eModifyCategory_Trap;

        private const int ModifyType_Add    = (int)eModifyType.eModifyType_Add;
        private const int ModifyType_Update = (int)eModifyType.eModifyType_Update;
        Dictionary<int, List<SkillModifyGroupData>> m_dicGroupData = new Dictionary<int, List<SkillModifyGroupData>>();
        private Dictionary<Actor, HashSet<Skill>> m_hashSkills = new Dictionary<Actor, HashSet<Skill>>();
        public void Init()
        {
            InitSkillModifyGroup();
        }
        public void ResetAllModifySkill(Actor _actor, uint nModifySkillGroup)
        {
            if(m_hashSkills.TryGetValue(_actor, out HashSet<Skill> _skillSet)) 
            { 
                foreach(Skill _skill in _skillSet)
                {
                    _skill.ResetModifyData();
                }
            }
        }
        private void InitSkillModifyGroup()
        {
            //List<SkillModifyData> lstModify = NeptuneBattle.Instance.DataProvider.GetSkillModifyDataLst();
            //foreach (SkillModifyData _modifyData in lstModify)
            //{
            //    List<SkillModifyGroupData> _lst = NeptuneBattle.Instance.DataProvider.GetSkillModifyGroupData((uint)_modifyData.SourceGroupID);
            //    if (_lst == null)
            //    {
            //        continue;
            //    }
            //    foreach (SkillModifyGroupData _groupData in _lst)
            //    {
            //        if (m_dicGroupData.TryGetValue(_groupData.SkillGroupID, out List<SkillModifyGroupData> val) == false)
            //        {
            //            m_dicGroupData[_groupData.SkillGroupID] = new List<SkillModifyGroupData>();
            //        }
            //        m_dicGroupData[_groupData.SkillGroupID].Add(_groupData);
            //    }
            //}
            List<SkillModifyData> lstModify = NeptuneBattle.Instance.DataProvider.GetSkillModifyDataLst();
            foreach (SkillModifyData _modifyData in lstModify)
            {
                List<SkillModifyGroupData> _lst = NeptuneBattle.Instance.DataProvider.GetSkillModifyGroupData((uint)_modifyData.SourceGroupID);
                if (_lst == null)
                {
                    continue;
                }
                foreach (SkillModifyGroupData _groupData in _lst)
                {
                    if (m_dicGroupData.TryGetValue(_groupData.SkillGroupID, out List<SkillModifyGroupData> val) == false)
                    {
                        m_dicGroupData[_groupData.SkillGroupID] = new List<SkillModifyGroupData>();
                    }
                    
                    m_dicGroupData[_groupData.SkillGroupID].Add(_groupData);
                }
                _lst = NeptuneBattle.Instance.DataProvider.GetSkillModifyGroupData((uint)_modifyData.TargetGroupID);
                if (_lst == null)
                {
                    continue;
                }
                foreach (SkillModifyGroupData _groupData in _lst)
                {
                    if (m_dicGroupData.TryGetValue(_groupData.SkillGroupID, out List<SkillModifyGroupData> val) == false)
                    {
                        m_dicGroupData[_groupData.SkillGroupID] = new List<SkillModifyGroupData>();
                    }
                    if (!m_dicGroupData[_groupData.SkillGroupID].Contains(_groupData))
                    {
                        m_dicGroupData[_groupData.SkillGroupID].Add(_groupData);
                    }   
                }
            }
        }
        public bool ModifyData(Actor _actor, int ModifyID)
        {
            if(_actor == null) return false; 
            if (ModifyID == 0) return false;

            SkillModifyData _SkillModify = NeptuneBattle.Instance.DataProvider.GetSkillModifyData(ModifyID);
            if(_SkillModify != null)
            {
                if(m_dicGroupData.TryGetValue(_SkillModify.TargetGroupID, out List<SkillModifyGroupData> lstVal) == true)
                {
                    if(lstVal != null)
                    {
                        foreach (SkillModifyGroupData _group in lstVal)
                        {
                            object entity = null;
                            if(_SkillModify.ModifyCategory == ModifyCategorySkill)
                            {
                                entity = _actor.GetActiveSkillByID(_group.SkillID);
                            }
                            else if(_SkillModify.ModifyCategory == ModifyCategoryAbility)
                            {
                                entity = _actor.GetAbility(_group.SkillID);
                                //foreach(Skill skill in NeptuneBattle.Instance.)
                                if (entity == null)//玩家没有获得这个Ability，也就是说这个是技能触发的Ability
                                {
                                    //如何得到Skill实例对应的Ability呢？现在只有AbilityID或者AbilityData
                                    AbilityData _data = NeptuneBattle.Instance.DataProvider.GetAbilityData(_group.SkillID);
                                    if(_data != null)
                                    {
                                        Actor _player = null;
                                        foreach(Actor act in NeptuneBattle.Instance.Roles)
                                        {
                                            if(act.Player != null)
                                            {
                                                _player = act;
                                            }
                                        }
                                        if (_player != null)
                                        {
                                            for (int i = 0; i < _player.tActiveSkills.Length; i++)
                                            {
                                                SkillData _skillData = _player.tActiveSkills[i].Data;
                                                if (_skillData != null && _skillData.Abilities != null && _skillData.Abilities.Count > 0)
                                                {
                                                    for (int j = 0; j < _skillData.Abilities.Count; j++)
                                                    {
                                                        if (_skillData.Abilities[j].ID == _data.ID)
                                                        {
                                                            entity = _skillData.Abilities[j];
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if(_SkillModify.ModifyCategory == ModifyCategoryMark)
                            {
                                entity = _actor.GetMarkByID(_group.SkillID);
                                if(entity == null)
                                {
                                    entity = NeptuneBattle.Instance.DataProvider.GetMarkData(_group.SkillID);
                                }
                            }
                            else if(_SkillModify.ModifyCategory == ModifyCategoryTrap)
                            {
                                entity = NeptuneBattle.Instance.DataProvider.GetTrapData(_group.SkillID);
                            }

                            if (entity != null)
                            {
                                //添加
                                if(_SkillModify.ModifyType == ModifyType_Add)
                                {
                                    AddTargetModifyData(_SkillModify, entity);
                                }
                                //改变
                                else if(_SkillModify.ModifyType == ModifyType_Update)
                                {
                                    //百分比改变，一定是改变
                                    if (_SkillModify.ParamVal != null && _SkillModify.ParamVal.Length > 0)
                                    {
                                        if (_SkillModify.ParamVal[0] == 2)
                                        {
                                            //暂定百分比改变的只有技能
                                            ChangeTargetModifyDataByPercent(_SkillModify, (Skill)entity);
                                        }
                                        else
                                        {
                                            ChangeTargetModifyData(_SkillModify, entity);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        private void ChangeTargetModifyData(SkillModifyData modify, object _skill)
        {
            object data = null;
            if (_skill.GetType() == typeof(Skill))
            {
                data = ((Skill)_skill).Data;
            }
            else if (_skill.GetType() == typeof(Trap))
            {
                data = ((Trap)_skill).TrapData;
            }
            else if(_skill.GetType() == typeof(Ability))
            {
                data = ((Ability)_skill).AbilityData;
            }
            else if(_skill.GetType() == typeof(AbilityData))
            {
                data = ((AbilityData)_skill);
            }
            else if(_skill.GetType() == typeof(Mark))
            {
                data = ((Mark)_skill).Data;
            }
            else if(_skill.GetType() == typeof(MarkData))
            {
                data = ((MarkData)_skill);
            }
            else if(_skill.GetType() == typeof(TrapData))
            {
                data = ((TrapData)_skill);
            }
            PropertyInfo propertyInfo = data.GetType().GetProperty(modify.ParamName);
            bool bModifySuccess = true;
            if (propertyInfo != null)
            {
                Type _typ = propertyInfo.PropertyType;
                if (_typ == typeof(int))
                {
                    if (int.TryParse(modify.ParamVal2, out int result))
                    {
                        propertyInfo.SetValue(data, result);
                    }
                }
                else if (_typ == typeof(float))
                {
                    if (float.TryParse(modify.ParamVal2, out float result))
                    {
                        propertyInfo.SetValue(data, result);
                    }
                }
                else if (_typ == typeof(bool))
                {
                    if (bool.TryParse(modify.ParamVal2, out bool result))
                    {
                        propertyInfo.SetValue(data, result);
                    }
                }
                else if (_typ == typeof(string))
                {
                    propertyInfo.SetValue(data, modify.ParamVal2);
                }
                else if (_typ == typeof(int[]))
                {
                    string[] stringArray = modify.ParamVal2.Split(';');

                    // 创建一个列表来存储整数
                    List<int> intList = new List<int>();

                    // 遍历每个子字符串并尝试转换为整数
                    foreach (string str in stringArray)
                    {
                        if (int.TryParse(str, out int result))
                        {
                            intList.Add(result);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to convert '{str}' to int.");
                        }
                    }
                    int[] ValArray = intList.ToArray();
                    propertyInfo.SetValue(data, ValArray);
                }
                else if (_typ == typeof(float[]))
                {
                    string[] stringArray = modify.ParamVal2.Split(';');

                    // 创建一个列表来存储整数
                    List<float> floatLst = new List<float>();

                    // 遍历每个子字符串并尝试转换为整数
                    foreach (string str in stringArray)
                    {
                        if (float.TryParse(str, out float result))
                        {
                            floatLst.Add(result);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to convert '{str}' to int.");
                        }
                    }
                    float[] ValArray = floatLst.ToArray();
                    propertyInfo.SetValue(data, ValArray);
                }
                else if (_typ.IsEnum)
                {
                    object instance = Activator.CreateInstance(_typ);
                    object enumValue = Enum.Parse(_typ, modify.ParamVal2);
                    propertyInfo.SetValue(data, enumValue);
                }
                else
                {
                    NgDebug.LogError(string.Format("Type [{0}] not supported", _typ.FullName));
                    bModifySuccess = false;
                }
                if( bModifySuccess == true)
                {
                    if(_skill.GetType() == typeof(Skill))
                    {
                        Skill temp = (Skill)_skill;
                        if (temp.Caster != null)
                        {
                            if (m_hashSkills.TryGetValue(temp.Caster, out HashSet<Skill> skills) == false)
                            {
                                m_hashSkills.Add(temp.Caster, new HashSet<Skill>());
                            }
                            if (m_hashSkills[temp.Caster].Contains(temp) == false)
                            {
                                m_hashSkills[temp.Caster].Add(temp);
                            }
                        }
                    }

                }
            }
        }
        private void AddTargetModifyData(SkillModifyData modify, object _skill)
        {
            object data = null;
            object _instanceData = null;
            if (_skill.GetType() == typeof(Skill))
            {
                data = ((Skill)_skill).Data;
            }
            else if (_skill.GetType() == typeof(Trap))
            {
                data = ((Trap)_skill).TrapData;
            }
            else if(_skill.GetType() == typeof(TrapData))
            {
                data = ((TrapData)_skill);
            }
            else if (_skill.GetType() == typeof(Ability))
            {
                data = ((Ability)_skill).AbilityData;
            }
            else if (_skill.GetType() == typeof(Mark))
            {
                _instanceData = ((Mark)_skill);
                data = ((Mark)_skill).Data;
            }
            else if(_skill.GetType() == typeof(MarkData))
            {
                data = ((MarkData)_skill);
            }
            else if(_skill.GetType() == typeof(AbilityData))
            {
                data = ((AbilityData)_skill); 
            }
            PropertyInfo propertyInfo = null;
            if (data != null)
            {
                propertyInfo = data.GetType().GetProperty(modify.ParamName);
                _addPropertyInfo(propertyInfo, modify, _skill, data);
            }
            if(_instanceData != null)
            {
                propertyInfo = _instanceData.GetType().GetProperty(modify.ParamName);
                _addPropertyInfo(propertyInfo, modify, _skill, _instanceData);
            }
        }

        private void _addPropertyInfo(PropertyInfo propertyInfo, SkillModifyData modify, object _targetObject, object data)
        {
            bool bModifySuccess = true;
            if (propertyInfo != null)
            {
                Type _typ = propertyInfo.PropertyType;
                if (_typ == typeof(int))
                {
                    if (int.TryParse(modify.ParamVal2, out int result))
                    {
                        int source = (int)propertyInfo.GetValue(data);
                        source += result;
                        propertyInfo.SetValue(data, source);
                    }
                }
                else if (_typ == typeof(float))
                {
                    if (float.TryParse(modify.ParamVal2, out float result))
                    {
                        float source = (float)propertyInfo.GetValue(data);
                        source += result;
                        propertyInfo.SetValue(data, source);
                    }
                }
                else if (_typ == typeof(int[]))
                {
                    string[] stringArray = modify.ParamVal2.Split(';');

                    // 创建一个列表来存储整数
                    List<int> intList = new List<int>();

                    // 遍历每个子字符串并尝试转换为整数
                    foreach (string str in stringArray)
                    {
                        if (int.TryParse(str, out int result))
                        {
                            intList.Add(result);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to convert '{str}' to int.");
                        }
                    }
                    int[] ValArray = intList.ToArray();
                    int[] resourceArray = propertyInfo.GetValue(data) as int[];
                    //可能本来就是空的，所以可以直接赋值
                    if (resourceArray == null || (resourceArray != null && resourceArray.Length == 0))
                    {
                        propertyInfo.SetValue(data, ValArray);
                    }
                    if (resourceArray != null && resourceArray.Length > 0)
                    {
                        List<int> temp = MergeArray(ValArray, resourceArray);
                        ValArray = temp.ToArray();
                    }
                    propertyInfo.SetValue(data, ValArray);
                }
                else if (_typ == typeof(float[]))
                {
                    string[] stringArray = modify.ParamVal2.Split(';');

                    // 创建一个列表来存储整数
                    List<float> floatLst = new List<float>();

                    // 遍历每个子字符串并尝试转换为整数
                    foreach (string str in stringArray)
                    {
                        if (float.TryParse(str, out float result))
                        {
                            floatLst.Add(result);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to convert '{str}' to int.");
                        }
                    }
                    float[] ValArray = floatLst.ToArray();
                    float[] resourceArray = propertyInfo.GetValue(data) as float[];
                    if (resourceArray == null)//转换失败，保护
                    {
                        bModifySuccess = false;
                        Console.WriteLine("数组为空");
                    }
                    if (resourceArray != null && resourceArray.Length == 0)
                    {
                        propertyInfo.SetValue(data, ValArray);
                    }
                    if (resourceArray != null && resourceArray.Length > 0)
                    {
                        List<float> temp = MergeArray(ValArray, resourceArray);
                        ValArray = temp.ToArray();
                    }
                    propertyInfo.SetValue(data, ValArray);
                }
                else
                {
                    NgDebug.LogError(string.Format("Type [{0}] not supported", _typ.FullName));
                    bModifySuccess = false;
                }
                if (bModifySuccess == true)
                {
                    if (_targetObject.GetType() == typeof(Skill))
                    {
                        Skill temp = (Skill)_targetObject;
                        if (temp.Caster != null)
                        {
                            if (m_hashSkills.TryGetValue(temp.Caster, out HashSet<Skill> skills) == false)
                            {
                                m_hashSkills.Add(temp.Caster, new HashSet<Skill>());
                            }
                            if (m_hashSkills[temp.Caster].Contains(_targetObject) == false)
                            {
                                m_hashSkills[temp.Caster].Add(temp);
                            }
                        }
                        SkillData skillData = temp.Data;
                        skillData.Abilities.Clear();
                        if (skillData.AbilityIDs != null && skillData.AbilityIDs.Length > 0)
                        {
                            foreach (int abilityID in skillData.AbilityIDs)
                            {
                                AbilityData _abilityData = NeptuneBattle.Instance.DataProvider.GetAbilityData(abilityID);
                                if (_abilityData != null)
                                {
                                    AbilityData ability_info = _abilityData.Clone();
                                    skillData.Abilities.Add(ability_info);
                                }
                                else
                                {
                                    NgDebug.LogFormat("Actor [{0}] Ability data {1} not found.", skillData.ID, abilityID);
                                }
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 暂定为百分比改变的值一定是技能相关的值,8.28暂定没有百分比的改变
        /// </summary>
        private void ChangeTargetModifyDataByPercent(SkillModifyData modify, Skill _skill)
        {
            //从静态数据中获得SkillData
            SkillData skillData = NeptuneBattle.Instance.DataProvider.GetSkillDatas(_skill.Data.ID);
            //通过反射得到对应的字段
            PropertyInfo skillPropertyInfo = _skill.Data.GetType().GetProperty(modify.ParamName);
            PropertyInfo skillDataPropertyInfo = skillData.GetType().GetProperty(modify.ParamName);
            if(skillPropertyInfo != null && skillDataPropertyInfo != null)
            {
                Type _skillPropertyType = skillPropertyInfo.PropertyType;
                Type _skillDataPropertyType = skillDataPropertyInfo.PropertyType;
                if (_skillPropertyType == typeof(int))
                {
                    //res是要减的百分比
                    if(int.TryParse(modify.ParamVal2, out int res))
                    {
                        //静态数据中的Data，最原始的Data
                        int sourceValue =  (int)skillDataPropertyInfo.GetValue(skillData);
                        int value = (int)(sourceValue * (1 - res * 0.01f));
                        skillPropertyInfo.SetValue(_skill.Data, value);
                    }
                }
            }
        }

        private List<T> MergeArray<T>(T[] array1, T[] array2)
        {
            List<T> res = new List<T>();
            for(int i = 0; i < array1.Length;i++)
            {
                res.Add(array1[i]);
            }
            for(int i = 0; i < array2.Length;i++)
            {
                res.Add(array2[i]);
            }

            return res;
        }
    }
}
