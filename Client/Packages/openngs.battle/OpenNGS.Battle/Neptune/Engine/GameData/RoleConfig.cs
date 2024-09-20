using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune
{
    /// <summary>
    /// RoleConfig
    /// 单位配置类
    /// </summary>
    public class RoleConfig
    {
        /// <summary>
        /// 是否为怪物
        /// </summary>
        public bool IsMonster;
        /// <summary>
        /// 是否为BOSS
        /// </summary>
        public bool IsBoss;
        /// <summary>
        /// 是否为召唤兽
        /// </summary>
        public bool IsDemon;

        /// <summary>
        /// 是否为国王
        /// </summary>
        public bool IsHomeBase;
        /// <summary>
        /// 是否为王后
        /// </summary>
        public bool IsVisitorBase;

        /// <summary>
        /// 是否为王后2
        /// </summary>
        public bool IsVisitorBaseSecond;

        /// <summary>
        /// 是否为英雄
        /// </summary>
        public bool IsHero;

        /// <summary>
        /// 是否为模型
        /// </summary>
        public bool IsModel;

        /// <summary>
        /// 是否为附加角色
        /// </summary>
        public bool IsAttached;
        /// <summary>
        /// HP系数
        /// </summary>
        public float HPFactor = 1;
        /// <summary>
        /// Dmg系数
        /// </summary>
        public float DmgPerSecFactor = 1;
        /// <summary>
        /// 比例
        /// </summary>
        public float Scale = 1;
        /// <summary>
        /// 携带金币
        /// </summary>
        public long Money;
        /// <summary>
        /// 估算品质
        /// </summary>
        public bool PredictQuality;
        /// <summary>
        /// 估算技能等级
        /// </summary>
        public bool PredictSkill;
        /// <summary>
        /// 估算最高品质
        /// </summary>
        public bool PredictMaxQuality;
        /// <summary>
        /// 契约者
        /// </summary>
        public Actor Promisor;

        public int MaxHP;
        public int AttackDamage;
        public int Armor;
        public int MagicResistance;
        public int WaveIndex = 1;
        /// <summary>
        /// 皮肤ID
        /// </summary>
        public int RoleSkinID;
    }

}