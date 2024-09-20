using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptune.Datas
{
    public abstract class RoleAttributesData
    {
        public RoleAttributeSet Value { get; protected set; }
        public RoleAttributeSet Ratio { get; protected set; }

        public float this[RoleAttribute attr]
        {
            get { return this.Value.Get((int)attr); }

            set { this.Value.Set((int)attr, value); }
        }
    }

    public partial class RoleData : RoleAttributesData
    {
        partial void OnConstructor()
        {
            this.AttributesValue = new OpenNGS.Core.AttributesSingle();
            this.AttributesRatio = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.AttributesValue);
            this.Ratio = new RoleAttributeSet(this.AttributesRatio);
        }
    }

    public partial class RoleGrowthData : RoleAttributesData
    {
        partial void OnConstructor()
        {
            this.AttributesValue = new OpenNGS.Core.AttributesSingle();
            this.AttributesRatio = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.AttributesValue);
            this.Ratio = new RoleAttributeSet(this.AttributesRatio);
        }
    }

    public partial class RoleQualityData
    {
        public RoleAttributeSet Value { get; private set; }
        partial void OnConstructor()
        {
            this.Attributes = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.Attributes);
        }
        public float this[RoleAttribute attr]
        {
            get { return this.Value.Get((int)attr); }

            set { this.Value.Set((int)attr, value); }
        }
    }



    public partial class RoleGearData : Data.DataBase<RoleGearData>
    {
    }

    public partial class SkillData
    {

        public RoleAttributeSet Value { get; private set; }
        partial void OnConstructor()
        {
            this.Attributes = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.Attributes);
        }

        public bool HasPropertys(RoleAttribute key)
        {
            return key == RoleAttribute.BasicNum || key == RoleAttribute.BaseRatio || key == RoleAttribute.ExtraBasicNum || key == RoleAttribute.AddLifeSteal || key == RoleAttribute.AddMagicLifeSteal || key == RoleAttribute.CastNum || key == RoleAttribute.Param1 || key == RoleAttribute.Param2 || key == RoleAttribute.Param3 || key == RoleAttribute.Param4 || key == RoleAttribute.Param5 || key == RoleAttribute.Param6;
        }

        public void Reset()
        {
            //TODO : 2024 修复属性
            
        }

        public float this[RoleAttribute attr]
        {
            get { return this.Value.Get((int)attr); }

            set { this.Value.Set((int)attr, value); }
        }

        public SkillData Clone()
        {
            SkillData clone = this.MemberwiseClone() as SkillData;
            //clone.Values = (float[])this.Values.Clone();
            return clone;
        }
    }


    public partial class AbilityData : RoleAttributesData
    {
        partial void OnConstructor()
        {
            this.AttributesValue = new OpenNGS.Core.AttributesSingle();
            this.AttributesRatio = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.AttributesValue);
            this.Ratio = new RoleAttributeSet(this.AttributesRatio);
        }
     

        public AbilityData Clone()
        {
            AbilityData clone = this.MemberwiseClone() as AbilityData;
            //clone.Values = (float[])this.Values.Clone();
            return clone;
        }
    }


    public partial class SummonData : RoleAttributesData
    {
        partial void OnConstructor()
        {
            this.AttributesValue = new OpenNGS.Core.AttributesSingle();
            this.AttributesRatio = new OpenNGS.Core.AttributesSingle();
            this.Value = new RoleAttributeSet(this.AttributesValue);
            this.Ratio = new RoleAttributeSet(this.AttributesRatio);
        }
    }
    public partial class AbilityEffects
    {
        public void Reset()
        {
            this.Attributes.Reset();
        }

        public ControlEffectSet Value { get; private set; }
        partial void OnConstructor()
        {
            this.Attributes = new OpenNGS.Core.AttributesInt32();
            this.Value = new ControlEffectSet(this.Attributes);
        }


        public bool this[ControlEffect attr]
        {
            get { return this.Value.Get((int)attr) == 1; }

            set { this.Value.Set((int)attr, value ? 1 : 0); }
        }
    }


    public partial class SkillGroupData
    {
        private RoleAttribute[] _growths = null;
        public RoleAttribute[] Growths
        {
            get
            {
                if (_growths == null)
                {
                    _growths = new RoleAttribute[]
                    {
                        this.Growth1,
                        this.Growth2,
                        this.Growth3,
                        this.Growth4
                    };
                }

                return _growths;
            }
        }

        private RoleAttribute[] _growth_ratios = null;
        public RoleAttribute[] Growth_Ratios
        {
            get
            {
                if (_growth_ratios == null)
                {
                    _growth_ratios = new RoleAttribute[]
                    {
                        this.GrowthRatio1,
                        this.GrowthRatio2,
                        this.GrowthRatio3,
                        this.GrowthRatio4
                    };
                }
                return _growth_ratios;
            }
        }

        private float[] _growths_ratio = null;
        public float[] GrowthRatios
        {
            get
            {
                if (_growths_ratio == null)
                {
                    _growths_ratio = new float[]
                    {
                        this.Growth1Ratio,
                        this.Growth2Ratio,
                        this.Growth3Ratio,
                        this.Growth4Ratio
                    };
                }
                return _growths_ratio;
            }
        }

        private float[] _growth_values = null;
        public float[] GrowthValues
        {
            get
            {
                if (_growth_values == null)
                {
                    _growth_values = new float[]
                    {
                        this.Growth1Value,
                        this.Growth2Value,
                        this.Growth3Value,
                        this.Growth4Value
                    };
                }
                return _growth_values;
            }
        }

        private float[] _growth_ratio_values = null;
        public float[] GrowthRatioValues
        {
            get
            {
                if (_growth_ratio_values == null)
                {
                    _growth_ratio_values = new float[]
                    {
                        this.GrowthRatio1Value,
                        this.GrowthRatio2Value,
                        this.GrowthRatio3Value,
                        this.GrowthRatio4Value
                    };
                }
                return _growth_ratio_values;
            }
        }

        private float[] _growth_ratios_ratio = null;
        public float[] GrowthRatio_Ratios
        {
            get
            {
                if (_growth_ratios_ratio == null)
                {
                    _growth_ratios_ratio = new float[]
                    {
                        this.GrowthRatio1Ratio,
                        this.GrowthRatio2Ratio,
                        this.GrowthRatio3Ratio,
                        this.GrowthRatio4Ratio
                    };
                }
                return _growth_ratios_ratio;
            }
        }

        public static int GetSkillSlotId(int tid, int tgid)
        {
            Dictionary<int, SkillGroupData> groups = NeptuneBattle.Instance.DataProvider.GetSkillGroups(tid);
            foreach (KeyValuePair<int, SkillGroupData> kv in groups)
            {
                int slot = kv.Key;
                SkillGroupData group = kv.Value;

                if (tgid == group.SkillGroupID)
                    return slot;
            }
            return 0;
        }
    }


    // RoleState与名字的映射类，扩展RoleState时需要把对应的名称添加到下面的映射中
    // 逻辑代码中使用RoleStateName.Instance[RoleState]代替RoleState.ToString()
    public class RoleStateName
    {
        public static RoleStateName Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoleStateName();
                }
                return _instance;
            }
        }

        public string this[Neptune.Datas.RoleState index]
        {
            get { return _names[(int)index]; }
        }

        private static RoleStateName _instance = null;

        private string[] _names = null;
        private RoleStateName()
        {
            _names = new string[(int)Neptune.Datas.RoleState.RoleStateMax];
            _names[(int)Neptune.Datas.RoleState.Base] = "Base";
            _names[(int)Neptune.Datas.RoleState.Idle] = "Idle";
            _names[(int)Neptune.Datas.RoleState.Move] = "Move";
            _names[(int)Neptune.Datas.RoleState.Attack] = "Attack";
            _names[(int)Neptune.Datas.RoleState.Damaged] = "Damaged";
            _names[(int)Neptune.Datas.RoleState.Stun] = "Stun";
            _names[(int)Neptune.Datas.RoleState.Death] = "Death";
            _names[(int)Neptune.Datas.RoleState.Dead] = "Dead";
            _names[(int)Neptune.Datas.RoleState.Birth] = "Birth";
            _names[(int)Neptune.Datas.RoleState.Cheer] = "Cheer";
            _names[(int)Neptune.Datas.RoleState.Max] = "Max";
            _names[(int)Neptune.Datas.RoleState.Transform] = "Transform";
        }
    }
}