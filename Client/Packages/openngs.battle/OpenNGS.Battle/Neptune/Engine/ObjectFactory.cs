using UnityEngine;
using System;
using System.Collections;
using Neptune.Datas;
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
        protected virtual Actor CreateActor(RoleInfo info, RoleSide side, RoleConfig config, RoleExtra extraData, UVector3 pos, Vector2 orientation, RoleData data, bool isRobot)
        {
            if (data == null)
            {
                data = Actor.LoadRoleData(info.tid);
            }
            if (data == null)
                return null;

            Actor role = new Actor();
            role.Create(info, data, side, config, extraData, pos, orientation);
            return role;
        }

        protected virtual Skill CreateSkill(SkillGroupData group, SkillData data, Actor caster, int level)
        {
            Skill talent = new Skill();
            talent.Create(group, data, caster, level);
            return talent;
        }


        protected virtual Ability CreateAbility(AbilityData data, Actor owner, Actor caster, Skill talent, int tid = -100)
        {
            Ability ability = new Ability();
            ability.Create(data, owner, caster, talent, tid);
            return ability;
        }

        protected virtual Mark CreateMark(MarkData data, Actor owner, Actor caster, int tid = -100)
        {
            Mark mark = new Mark();
            mark.Create(data, owner, caster);
            return mark;
        }

        protected virtual Trap CreateTrap(int trapId, Skill talent, Vector2 position, Actor target = null)
        {
            Trap trap = new Trap();
            trap.Create(trapId, talent, position, target);
            return trap;
        }

        protected virtual IEffectAgent CreateEffectJoint(string effectRes, Vector2 pos, Vector2 direction, float height, int z, Effect effect, Action<IEffectController> onload = null, Entity element = null)
        {
            throw new Exception("must implement CreateEffectJoint in you ObjectFactory");
        }

        protected virtual IEffectAgent CreateEffectJoint(Effect effect, EffectType type)
        {
            throw new Exception("must implement CreateEffectJoint in you ObjectFactory");
        }

        protected virtual IActorAgent CreateRoleJoint(Actor role)
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
        public static Actor Create(RoleInfo info, RoleSide side, RoleConfig config, RoleExtra extraData, UVector3 pos, Vector2 orientation, RoleData data = null, bool isRobot = false)
        {
            return Instance.CreateActor(info, side, config, extraData, pos, orientation, data, isRobot);
        }

        /// <summary>
        /// 创建一个Skill
        /// </summary>
        /// <param name="group"></param>
        /// <param name="data"></param>
        /// <param name="caster"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Skill Create(SkillGroupData group, SkillData data, Actor caster, int level)
        {
            return Instance.CreateSkill(group, data, caster, level);
        }


        public static Ability Create(AbilityData data, Actor owner, Actor caster, Skill talent, int tid = -100)
        {
            return Instance.CreateAbility(data, owner, caster, talent, tid);
        }

        public static Mark Create(MarkData data, Actor owner, Actor caster, int tid = -100)
        {
            return Instance.CreateMark(data, owner, caster, tid);
        }

        public static Trap Create(int trapId, Skill talent, Vector2 position, Actor target = null)
        {
            return Instance.CreateTrap(trapId, talent, position, target);
        }

        public static IEffectAgent Create(string effectRes, Vector2 pos, Vector2 direction, float height, int z, Effect effect, Action<IEffectController> onload = null, Entity element = null)
        {
            return Instance.CreateEffectJoint(effectRes, pos, direction, height, z, effect, onload, element);
        }

        public static IEffectAgent Create(Effect effect, EffectType type)
        {
            return Instance.CreateEffectJoint(effect, type);
        }


        public static IActorAgent Create(Actor role)
        {
            return Instance.CreateRoleJoint(role);
        }
    }

}