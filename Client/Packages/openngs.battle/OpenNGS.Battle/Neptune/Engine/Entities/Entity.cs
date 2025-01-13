using System;
using UnityEngine;
using Neptune.Datas;



namespace Neptune
{
    /// <summary>
    /// 元件类
    /// 角色、NPC、抛射器等游戏元件的公共基类
    /// </summary>
    public abstract class Entity : PoolObj
    {
        /// <summary>
        /// 元件所属阵营
        /// </summary>
        public RoleSide Side;

        /// <summary>
        /// 当前立场
        /// </summary>
        public RelativeSide StandSide;

        /// <summary>
        /// 碰撞半径
        /// </summary>
        public int Radius;
        /// <summary>
        /// 位置
        /// </summary>
        public UVector2 Position = UVector2.one * 10000;
        /// <summary>
        /// 速度
        /// </summary>
        public UVector2 Speed;
        public UVector2 ShoveSpeed;
        /// <summary>
        /// 高度
        /// </summary>
        public int Height;
        /// <summary>
        /// 方向
        /// </summary>
        public Vector2 Orientation;
        /// <summary>
        /// 元件是否暂停
        /// </summary>
        public bool IsPause;
        /// <summary>
        /// Joint是否暂停
        /// </summary>
        public bool IsJointPause;
        /// <summary>
        /// 是否终止
        /// </summary>
        public bool Running;
        /// <summary>
        /// 伤害修正
        /// </summary>
        public float DamageFactor;


        public int AOIIndex = -1;

        /// <summary>
        /// 获取元件名称
        /// </summary>
        public virtual string Name { get { return "Entity"; } }

        /// <summary>
        /// 获取元件全名（包含阵营标识及死亡状态）
        /// </summary>
        public virtual string FullName
        {
            get { return string.Format("[{0}{1}]{2}", this.SideSign, !this.Running ? "D" : "", this.Name); }
        }

        public RelativeSide GetRelation(Entity other)
        {
            return (RelativeSide)((int)this.StandSide * (int)other.StandSide * (int)(this.Side == other.Side ? RelativeSide.Friend : RelativeSide.Enemy));
        }

        public RelativeSide GetRelation(RoleSide otherSide)
        {
            return (RelativeSide)((int)this.StandSide * (int)(this.Side == otherSide ? RelativeSide.Friend : RelativeSide.Enemy));
        }

        /// <summary>
        /// 返回对象是否离开场景区域
        /// </summary>
        /// <returns></returns>
        public bool IsOutOfBattleArea
        {
            get
            {
                Rect rect = BattleField.Current.rect;//获取战斗区域
                return !rect.Contains((Vector2)this.Position);
            }
        }

        /// <summary>
        /// 获取阵营标识
        /// </summary>
        public string SideSign
        {
            get
            {
                //#if BATTLE_LOG
                //           if (EngineGlobal.BattleLog)
                //                return string.Empty;
                //#endif
                switch (this.Side)
                {
                    case RoleSide.SideA:
                        return NeptuneConst.SymbolPlus;
                    case RoleSide.SideB:
                        return NeptuneConst.SymbolMinus;
                    default:
                        return string.Empty;
                }
            }
        }

        protected IAgent joint;

        /// <summary>
        /// Entity 构造函数
        /// 进行数据初始化
        /// </summary>
        public Entity()
        {
            this.Side = RoleSide.All;
            this.StandSide = RelativeSide.Friend;
            this.Radius = 0;
            this.Position = UVector2.zero;
            this.Orientation = UVector2.right;
            this.Speed = NeptuneConst.Vector2Zero;
            this.IsPause = false;
            this.IsJointPause = false;
            this.Running = true;
            this.DamageFactor = 0;
        }

        protected void Create()
        {
            this.Side = RoleSide.All;
            this.StandSide = RelativeSide.Friend;
            this.Radius = 0;
            this.Position = UVector2.zero;
            this.Orientation = UVector2.right;
            this.Speed = NeptuneConst.Vector2Zero;
            this.IsPause = false;
            this.IsJointPause = false;
            this.Running = true;
            this.DamageFactor = 0;
            this.ShoveSpeed = NeptuneConst.Vector2Zero;
            this.Height = 0;
            this.AOIIndex = -1;
            this.joint = null;
        }

        /// <summary>
        /// 设置位置
        /// 检测是否超出战斗区域并进行修正
        /// </summary>
        /// <param name="pos"></param>
        public virtual void SetPosition(UVector2 pos)
        {
            this.Position.x = (int)Math.Max(Math.Min(BattleField.Current.rect.xMax, pos.x), BattleField.Current.rect.xMin);
            this.Position.y = (int)Math.Max(Math.Min(BattleField.Current.rect.yMax, pos.y), BattleField.Current.rect.yMin);
        }

        /// <summary>
        /// 碰撞测试
        /// 检测是否与目标碰撞
        /// </summary>
        /// <param name="another">碰撞目标</param>
        /// <returns></returns>
        public bool HitTest(Entity element, int radius = 0)
        {
            int sqrRadius = this.Radius + element.Radius + radius;
            return (this.Position - element.Position).sqrMagnitude < sqrRadius * sqrRadius;
        }
        /// <summary>
        /// 获取与target的距离
        /// </summary>
        /// <param name="target">目标target</param>
        /// <param name="radius">是否计算半径</param>
        /// <returns></returns>
        public int Distance(Entity target, bool radius)
        {
            int distance = (this.Position - target.Position).magnitude;
            if (radius)
                distance = Math.Max(distance - this.Radius - target.Radius, 0);
            return distance;
        }
        /// <summary>
        /// 获取与特定目标点的距离
        /// </summary>
        /// <param name="pos">目标点坐标</param>
        /// <param name="radius">目标的半径</param>
        /// <param name="selfradius">是否计算自己的半径</param>
        /// <returns></returns>
        public int Distance(UVector2 pos, int radius, bool selfradius)
        {
            int distance = (this.Position - pos).magnitude;
            if (selfradius)
                distance = distance - this.Radius;
            distance = Math.Max(distance - radius, 0);
            return distance;
        }

        /// <summary>
        /// 元件更新
        /// 根据速度更新元件位置
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnEnterFrame(float dt)
        {
            this.Position = this.Position + this.Speed * dt;
#if GAME_2D
        if (this.Speed.x * this.Orientation.x < 0)
            this.Orientation = this.Orientation * -1;
#endif
        }

        /// <summary>
        /// 暂停当前元件
        /// </summary>
        public void Suspend(bool needMask = true)
        {
            //Debug.Log("Entity Suspend");
            this.IsPause = true;
            if (!this.IsJointPause && this.joint != null)
            {
                this.IsJointPause = true;
                this.joint.Suspend();

                if (needMask)
                {
                    //                this.joint.SetColor(0.5f, 0.5f, 0.5f);
                    this.joint.Highlight(false);
                }
            }
        }

        /// <summary>
        /// 取消暂停
        /// </summary>
        public void Resume()
        {
            //Debug.Log("Entity Resume");
            bool needSetColor = this.IsPause;
            this.IsPause = false;
            this.ResumeActor();

            //		if (this.joint != null && needSetColor)
            if (this.joint != null)
            {
                //            this.joint.SetColor(1.0f, 1.0f, 1.0f);
                this.joint.Highlight(false);
            }
        }

        /// <summary>
        /// 高亮显示元件
        /// </summary>
        public void Highlight()
        {
            if (this.joint != null)
            {
                //            this.joint.SetColor(2.5f, 2.5f, 2.5f);
                this.joint.Highlight(true);
            }
        }

        /// <summary>
        /// 解冻元件包含的Actor
        /// </summary>
        public void ResumeActor()
        {
            if (this.IsJointPause)
            {
                this.IsJointPause = false;
                if (this.joint != null)
                {
                    //this.joint.SetColor(2.5f, 2.5f, 2.5f);
                    this.joint.Resume();
                }

            }
        }


        /// <summary>
        /// 终止当前元件
        /// </summary>
        public virtual void Stop()
        {
            this.Running = false;
        }

        public override void OnDelete()
        {

        }

        public override void Delete()
        {

        }


        public static bool RemoveStopedElement(Entity e)
        {
            if (e.Running == false)
            {
                e.Delete();
                return true;
            }
            return false;
        }
    }
}