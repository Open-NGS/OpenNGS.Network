namespace MissQ.Common
{
    public class MissQBaseConst
    {
        // --------------------通用--------------------
        // 资源版本号KEY
        public const string CONSTVersionKey = "VersionKey";
        public const string CONSTResVersionKey = "ResVersionKey";
        // 单位在游戏界面中的根目录
        public const string CONSTUnitSetName = "UnitPrefabSet";
		public const string UndestroyUnitSetName = "UndestroyUnitPrefabSet";
        
        
        // --------------------单位--------------------
        // 千分比
        public const int CONSTUnitThousand = 1000;
        // 万分比
        public const int CONSTUnitTenthousand = 10000;
        // 无效ID值
        public const int CONSTInvalidID = -1;
        
		// 攻击CD间隔的key
        public const int CONSTUnitDicAttackName = 1000;
        
        
        // 节点停止距离
        public static readonly FP CONSTMinStopDistance = 0.25f;
        // icon路径
        public const string CONSTIconPath = "UI/Icon/Unit/";
        // 默认icon名字
        public const string CONSTDefaultIconName = "cm_hero_000";
        
		
        public const float CONST_ATK_ANGLE = 180;

        // --------------------单位--------------------
        // 更换目标决策周期
        public const int TargetCycleTime = 15;
        // 搜索决策周期
        public const int SearchCycleTime = 8;
		
        // --------------------动画--------------------
        
        /// <summary>
        /// 动作信息最小id
        /// </summary>
        public const int ANIMATORATTRIBUTE_DEFAULE_ID = 1000;
        
        // --------------------卡牌--------------------
        // 默认icon名字
        public const string DefaultCardPNG = "Textures/Cards/Card_Temp_Spell";

        // 死亡时间
        public const float DeadTime = 6f;

        // 英雄死亡时间
        public const float HeroDeadTime = 100f;
    }
}
