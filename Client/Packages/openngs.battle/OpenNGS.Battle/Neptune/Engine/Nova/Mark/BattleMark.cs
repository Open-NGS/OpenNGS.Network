using System;
using Neptune.GameData;
using UnityEngine;

namespace Neptune
{

    /// <summary>
    /// 印记系统描述：
    /// 一个英雄A，在初始化时，身上带有若干技能；
    /// 有些技能在打到目标B时，会给B身上添加印记，还可能试图触发目标身上的某些印记；
    /// B身上的某个印记触发时，会激发此印记添加者(A)身上的某个技能，技能的目标是B；
    /// </summary>
    public class BattleMark : PoolObj
    {
        private MarkData _data;
        private BattleActor _owner;            //被添加者
        private BattleActor _caster;           //添加者

        public int _count;             //当前层数 创建时层数为零
                                       //    private float _timeCreated;     //创建时间
        private float _timeCounter;
        private float _timeLastTrigger; //上次成功触发的时间 -100代表首次触发
        private float _lifeTime;
        private int _currentEffectId; //当前播放特效的ID
        public MarkData Data { get { return _data; } }

        public BattleMark()
        {

        }

        public override void OnDelete()
        {

        }

        public void Create(MarkData data, BattleActor owner, BattleActor caster)
        {
            _data = data.Clone();
            _owner = owner;
            _caster = caster;
            _caster.RoleSkin.RoleSkinMarkReplace(_data);
            _count = 0;
            _timeCounter = 0;
            _timeLastTrigger = -100;
            _lifeTime = _data.LastTime / 1000f;
            _currentEffectId = 0;
            CreateEffect();
        }
        public override void Delete()
        {
            NObjectPool<BattleMark>.Delete(this);
        }

        public virtual void OnEnterFrame(float dt)
        {
            _timeCounter += dt;

            if (_lifeTime != 0)
            {
                if (_timeCounter >= _lifeTime)
                {
                    //持续时间到达后，从Owner身上移除。这样算法简单，程序健壮。
                    //由于印记有持续时间，创建不会很频繁，不会有性能问题。

                    _owner.RemoveMarkByID(_data.ID);
                }
            }
        }

        /// <summary>
        /// 印记加深
        /// </summary>
        public void Deepen()
        {
            if (_count < _data.MaxCount || _data.MaxCount <= 0)
            {
                _count++; //每次加一层
            }

            _timeLastTrigger = UFloat.Round(_timeLastTrigger - _timeCounter);
            if (_data.MarkOverlayType == OverlayType.ResetTime)
            {
                _timeCounter = 0;
                SetEffectCount(_count - 1);
            }

            if (_data.Auto)
            {
                //自动触发类的印记 每帧尝试触发
                InnerTrigger();
            }

        }

        public void SetEffectCount(int index)
        {
            if (_owner.Joint != null && _owner.Joint.Controller != null)
            {
                _owner.Joint.SetEffectLayer(_currentEffectId, index);
            }
        }
        public void CreateEffect()
        {
            if (!string.IsNullOrEmpty(_data.Effect) && (_owner.Data.RoleType & _data.EffectRoleType) <= 0 && _owner.Joint != null && _owner.Joint.Controller != null)
            {
                if (_currentEffectId > 0)
                {
                    _owner.Joint.RemoveEffect(_currentEffectId);
                }
                IActorController rolecontroller = null;
                if (_owner != _caster && _caster.Joint != null)
                {
                    rolecontroller = _caster.Joint.Controller;
                }
                _currentEffectId = _owner.Joint.AddEffect(_data.Effect, EffectType.Ability, EngineConst.Vector3Zero, rolecontroller);
                SetEffectCount(_count - 1);
            }
        }

        /// <summary>
        /// 外部触发 只判断层数
        /// </summary>
        /// 触发成功返回true 触发失败返回false
        public bool OuterTrigger(UVector2 position)
        {
            if (_count >= _data.TriggerCount)
            {
                TriggerTalent(position);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 自动触发 判断层数和触发CD
        /// </summary>
        /// 触发成功返回true 触发失败返回false
        private bool InnerTrigger()
        {
            if (_count >= _data.TriggerCount)
            {
                float interval = UFloat.Round(_timeCounter - _timeLastTrigger);
                float cd = _data.TriggerCD / 1000f;
                if (_timeLastTrigger == -100 || interval >= cd)
                {
                    AddMarkTrigger();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private void AddMarkTrigger()
        {
            //_count -= _data.CostCount;
            ResetCount();
            _timeLastTrigger = _timeCounter;

            //释放技能 Start
            int talentID = _data.AddTalentID;

            if (_data.TalentIDs != null)
            {
                int count = _data.TalentIDs.Count;
                int randomIndex = (int)Math.Floor((float)Util.Random.Rand() * count); //[0,count-1]

                talentID = _data.TalentIDs[randomIndex];
            }

            if (talentID == 0)
            {
                return;
            }

            BattleSkill talent = _caster.GetTalentById(talentID);
            //_caster.UseTalent(talent, _owner);
            if (talent != null)
            {
                //talent.HitTarget(this._owner);
                ResultType ret = talent.CanUse(_owner, true, true);
                if (ret == ResultType.Success)//return true;
                    talent.Start(_owner);

            }
            //释放技能 End

            //在印记层数为0时移出印记
            if (_count <= 0)
            {
                _owner.RemoveMarkByID(_data.ID);
            }
        }

        private void TriggerTalent(UVector2 position)
        {
            //_count -= _data.CostCount;
            ResetCount();

            _timeLastTrigger = _timeCounter;

            //释放技能 Start
            int talentID = _data.TriggerTalentID;
            if (talentID == 0)
            {
                return;
            }

            BattleSkill talent = _caster.GetTalentById(talentID);
            //_caster.UseTalent(talent, _owner);
            if (talent != null)
            {
                ResultType ret = talent.CanUse(_owner, true, true);
                if (ret == ResultType.Success)
                {
                    if (_data.UseTargetPosition)
                    {
                        talent.Start(_owner, position);
                    }
                    else
                        talent.Start(_owner);
                }//return true;

            }
            //释放技能 End

            //在印记层数为0时移出印记特效
            if (_count <= 0)
            {
                _owner.RemoveMarkByID(_data.ID);
            }
        }

        private void ResetCount()
        {
            _count -= Data.CostCount;
            if (_count > 0 && _data.MarkOverlayType == OverlayType.ResetTime)
            {
                SetEffectCount(_count - 1);
            }
        }
        public void OnDestroyed()
        {
            ClearEffect();
            Delete();
        }
        public void ClearEffect()
        {
            if (_currentEffectId > 0 && _owner.Joint != null)
            {
                _owner.Joint.RemoveEffect(_currentEffectId);
            }
        }
    }
}