using UnityEngine;
using System;
using System.Collections;
using Neptune.GameData;

namespace Neptune
{
    public class ObjectFactory
    {
        static ObjectFactory instance = null;
        public static ObjectFactory Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = new ObjectFactory();
                return instance;
            }
        }

        public static void Setup(ObjectFactory factory)
        {
            instance = factory;
            instance.Init();
        }

        public virtual void Init()
        {

        }

        /// <summary>
        /// 创建一个 Actor
        /// </summary>
        /// <param name="info">英雄数据</param>
        /// <param name="side">阵营</param>
        /// <param name="config">配置</param>
        /// <param name="extraData">扩展数据</param>
        /// <param name="pos">初始位置</param>
        /// <param name="orientation">初始方向</param>
        /// <returns></returns>
        protected virtual BattleActor CreateActor(RoleInfo info, RoleSide side, RoleConfig config, RoleExtra extraData, UVector3 pos, Vector2 orientation, RoleData data, bool isRobot)
        {
            if (data == null)
            {
                data = BattleActor.LoadRoleData(info.tid);
            }
            if (data == null)
                return null;

            BattleActor role = new BattleActor();
            role.Create(info, data, side, config, extraData, pos, orientation);
            return role;
        }

        protected virtual BattleSkill CreateTalent(TalentGroupData group, TalentData data, BattleActor caster, int level)
        {
            BattleSkill talent = new BattleSkill();
            talent.Create(group, data, caster, level);
            return talent;
        }


        protected virtual Ability CreateAbility(AbilityData data, BattleActor owner, BattleActor caster, BattleSkill talent, int tid = -100)
        {
            Ability ability = new Ability();
            ability.Create(data, owner, caster, talent, tid);
            return ability;
        }

        protected virtual BattleMark CreateMark(MarkData data, BattleActor owner, BattleActor caster, int tid = -100)
        {
            BattleMark mark = new BattleMark();
            mark.Create(data, owner, caster);
            return mark;
        }

        protected virtual BattleTrap CreateTrap(int trapId, BattleSkill talent, Vector2 position, BattleActor target = null)
        {
            BattleTrap trap = new BattleTrap();
            trap.Create(trapId, talent, position, target);
            return trap;
        }

        protected virtual IEffectAgent CreateEffectJoint(string effectRes, Vector2 pos, Vector2 direction, float height, int z, BattleEffect effect, Action<IEffectController> onload = null, BattleEntity element = null)
        {
            throw new Exception("must implement CreateEffectJoint in you ObjectFactory");
        }

        protected virtual IEffectAgent CreateEffectJoint(BattleEffect effect, EffectType type)
        {
            throw new Exception("must implement CreateEffectJoint in you ObjectFactory");
        }

        protected virtual IActorAgent CreateRoleJoint(BattleActor role)
        {
            throw new Exception("must implement CreateRoleJoint in you ObjectFactory");
        }


        /// <summary>
        /// 创建一个 Actor
        /// </summary>
        /// <param name="info">英雄数据</param>
        /// <param name="side">阵营</param>
        /// <param name="config">配置</param>
        /// <param name="extraData">扩展数据</param>
        /// <param name="pos">初始位置</param>
        /// <returns></returns>
        public static BattleActor Create(RoleInfo info, RoleSide side, RoleConfig config, RoleExtra extraData, UVector3 pos, Vector2 orientation, RoleData data = null, bool isRobot = false)
        {
            return Instance.CreateActor(info, side, config, extraData, pos, orientation, data, isRobot);
        }

        /// <summary>
        /// 创建一个Talent
        /// </summary>
        /// <param name="group"></param>
        /// <param name="data"></param>
        /// <param name="caster"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static BattleSkill Create(TalentGroupData group, TalentData data, BattleActor caster, int level)
        {
            return Instance.CreateTalent(group, data, caster, level);
        }


        public static Ability Create(AbilityData data, BattleActor owner, BattleActor caster, BattleSkill talent, int tid = -100)
        {
            return Instance.CreateAbility(data, owner, caster, talent, tid);
        }

        public static BattleMark Create(MarkData data, BattleActor owner, BattleActor caster, int tid = -100)
        {
            return Instance.CreateMark(data, owner, caster, tid);
        }

        public static BattleTrap Create(int trapId, BattleSkill talent, Vector2 position, BattleActor target = null)
        {
            return Instance.CreateTrap(trapId, talent, position, target);
        }

        public static IEffectAgent Create(string effectRes, Vector2 pos, Vector2 direction, float height, int z, BattleEffect effect, Action<IEffectController> onload = null, BattleEntity element = null)
        {
            return Instance.CreateEffectJoint(effectRes, pos, direction, height, z, effect, onload, element);
        }

        public static IEffectAgent Create(BattleEffect effect, EffectType type)
        {
            return Instance.CreateEffectJoint(effect, type);
        }


        public static IActorAgent Create(BattleActor role)
        {
            return Instance.CreateRoleJoint(role);
        }
    }
}