using System;
using System.Collections.Generic;

namespace Neptune.GameData
{
    public class TalentGroupData: DataBase<TalentGroupData>
    {
        public int TalentGroupID { get; set; }
        public int ParentID { get; set; }
        public string TalentName { get; set; }
        public string DisplayName { get; set; }
        public int ChildGroupID { get; set; }
        public bool Legend { get; set; }
        public int BasicGrowth { get; set; }
        public int Index { get; set; }
        public int CD { get; set; }
        public bool Hide { get; set; }
        public string Icon { get; set; }
        public int InitLevel { get; set; }
        public bool Interruptible { get; set; }
        public float PowerIncr { get; set; }
        public float Priority { get; set; }

        public bool InitEnabled { get; set; }
        public LevelUpType LevelUpType { get; set; }
        public RoleAttribute Growth1 { get; set; }
        public float Growth1Value { get; set; }
        public float Growth1Ratio { get; set; }
        public string Growth1Tips { get; set; }
        public string Growth1Text { get; set; }

        public RoleAttribute GrowthRatio1 { get; set; }
        public float GrowthRatio1Value { get; set; }
        public float GrowthRatio1Ratio { get; set; }


        public RoleAttribute Growth2 { get; set; }
        public float Growth2Value { get; set; }
        public float Growth2Ratio { get; set; }
        public string Growth2Tips { get; set; }
        public string Growth2Text { get; set; }

        public RoleAttribute GrowthRatio2 { get; set; }
        public float GrowthRatio2Value { get; set; }
        public float GrowthRatio2Ratio { get; set; }


        public RoleAttribute Growth3 { get; set; }
        public float Growth3Value { get; set; }
        public float Growth3Ratio { get; set; }
        public string Growth3Tips { get; set; }
        public string Growth3Text { get; set; }

        public RoleAttribute GrowthRatio3 { get; set; }
        public float GrowthRatio3Value { get; set; }
        public float GrowthRatio3Ratio { get; set; }


        public RoleAttribute Growth4 { get; set; }
        public float Growth4Value { get; set; }
        public float Growth4Ratio { get; set; }
        public string Growth4Tips { get; set; }
        public string Growth4Text { get; set; }


        public RoleAttribute GrowthRatio4 { get; set; }
        public float GrowthRatio4Value { get; set; }
        public float GrowthRatio4Ratio { get; set; }

        public bool InheritCD { get; set; }
        public bool InheritCombo { get; set; }

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




        private string[] _growth_tips = null;
        public string[] GrowthTips
        {
            get
            {
                if (_growth_tips == null)
                {
                    _growth_tips = new string[]
                    {
                        this.Growth1Tips,
                        this.Growth2Tips,
                        this.Growth3Tips,
                        this.Growth4Tips
                    };
                }
                return _growth_tips;
            }
        }

        private string[] _growth_texts = null;
        public string[] GrowthTexts
        {
            get
            {
                if (_growth_texts == null)
                {
                    _growth_texts = new string[]
                    {
                        this.Growth1Text,
                        this.Growth2Text,
                        this.Growth3Text,
                        this.Growth4Text
                    };
                }
                return _growth_texts;
            }
        }


        public int Unlock { get; set; }
        public int UnlockMon { get; set; }
        public string[] TalentUIEffect { get; set; }
        public List<int> SlotDisabled { get; set; }
        public static int GetTalentSlotId(int tid, int tgid)
        {
            Dictionary<int, TalentGroupData> groups = NeptuneBattle.Instance.DataProvider.GetTalentGroups(tid);
            foreach (KeyValuePair<int, TalentGroupData> kv in groups)
            {
                int slot = kv.Key;
                TalentGroupData group = kv.Value;

                if (tgid == group.TalentGroupID)
                    return slot;
            }
            return 0;
        }
    }
}
