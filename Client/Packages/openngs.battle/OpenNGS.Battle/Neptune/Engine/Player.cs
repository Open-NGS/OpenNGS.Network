using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;
using UnityEngine;

namespace Neptune
{
    public class Player
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
        public virtual Actor CurrentRole { get; set; }

        public virtual void RecordDamage(int damage, Player attacker, RoleType roleType)
        {
        }

        public virtual void RecordInjured(int damage, Actor from)
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
        public virtual void OnDeath(Actor attacker)
        {

        }
        public virtual void OnDamage(Actor target, Skill sourceSkill)
        {

        }

        /// <summary>
        /// 助攻击杀掉目标
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnAssistKill(Actor target, Actor attacker, int TotalAsset)
        {

        }
        public virtual void SetMoveDirection(Vector2 moveDirect, bool isRobot = false)
        {

        }
    }

}