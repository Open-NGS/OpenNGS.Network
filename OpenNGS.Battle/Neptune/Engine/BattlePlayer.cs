using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;
using UnityEngine;

public class BattlePlayer
{
    public int ID;
    public string Name;
    public int Level;

    public TeamSide Team;

    public RoleSide Side;
    /// <summary>
    /// 是不是当前主控英雄
    /// </summary>
    public bool isMainPlayer;

    /// <summary>
    /// 获取玩家的当前角色
    /// </summary>
    public virtual BattleActor CurrentRole { get; set; }

    public virtual void RecordDamage(int damage, BattlePlayer attacker, RoleType roleType)
    {
    }

    public virtual void RecordInjured(int damage, BattleActor from)
    {

    }

    public virtual RoleSide GetEnemySide()
    {
        return this.Side == RoleSide.SideA ? RoleSide.SideB : RoleSide.SideA;
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    /// <param name="target"></param>
    public virtual void OnDeath(BattleActor attacker)
    {

    }


    /// <summary>
    /// 助攻击杀掉目标
    /// </summary>
    /// <param name="target"></param>
    public virtual void OnAssistKill(BattleActor target, BattleActor attacker, int TotalAsset)
    {

    }
    public virtual void SetMoveDirection(Vector2 moveDirect, bool isRobot = false)
    {

    }
}
