using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 角色信息
/// </summary>
public class RoleInfo
{
    public int tid;  // 英雄编号
    public int rank;	// 品阶
    public int level;  // 等级
    public int stars;  // 星星数
    public int exp;	// 当前经验值
    public int skinid;//皮肤编号 
    public LegendInfo legend; // 传奇
    public List<EquipInfo> items = new List<EquipInfo>();
    // 技能列表
    public List<SkillInfo> skillLevels = new List<SkillInfo>();
    //repeated HeroSkill      skillList   =   8; // 技能列表



}
