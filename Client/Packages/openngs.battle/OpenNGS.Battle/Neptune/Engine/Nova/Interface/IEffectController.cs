using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;
using System;
using Neptune;

namespace Neptune
{
    /// <summary>
    /// Effect controller interface，Effect controller must implement this interface
    /// </summary>
    public interface IEffectController : IController
    {
        void SetType(EffectType type);
        void SetEffect(BattleEffect effect);
        void SetEffectID(int id);
        void SetLayer(int layer);
        void SetPosition(Vector3 beginPos, Vector3 endPos);
        /// <summary>
        /// 设置特效动作
        /// </summary>
        /// <param name="anim_name">动作名称</param>
        void SetAnimation(string action_name);
        /// <summary>
        /// 设置特效动作流逝时间
        /// </summary>
        /// <param name="elapsed">流逝时间</param>
        void SetAnimationElapsed(float elapsed);
        /// <summary>
        /// 设置特效动作速率
        /// </summary>
        /// <param name="i">速度</param>
        void SetAnimationSpeed(float v);
        /// <summary>
        /// 设置是否循环
        /// </summary>
        /// <param name="loop">是否循环</param>
        void SetAnimationLoop(bool loop);
        /// <summary>
        /// 动作完成事件
        /// </summary>
        void OnAnimationEnd();
        //void JointPlaySound(string name);
        //void PlayVoice(string name);

        void AddFloorEffect(BattleActor ownerRole, int type, List<float> indicatorParams);

        void OnHit(BattleEntity target);
    }
    public class NullEffect : IEffectController
    {
        public string anim_name;
        public bool loop;

        public bool IsRunning { get { return false; } }

        public void SetAnimationLoop(bool loop)
        {
            this.loop = loop;
        }

        public void SetAnimation(string anim_name)
        {

        }

        public void SetRotation(float rotation)
        {

        }

        public void SetAnimationSpeed(float v)
        {
        }
        public void SetAnimationElapsed(float elapsed)
        {

        }
        public void OnAnimationEnd()
        {
        }

        public void SetPosition(Vector3 pos)
        {

        }



        public void SetScale(Vector3 svale)
        {

        }



        public void SetRotation(bool reverse)
        {

        }


        public void SetVisible(bool visible, bool isTransparent, string ignoreEffectNames = "NA")
        {

        }


        public Vector3 Direction { get; set; }


        public void Suspend()
        {

        }

        public void Resume()
        {

        }

        public void Stop(float delay = 0)
        {

        }

        public void SetType(EffectType type)
        {
        }


        public void SetPosition(Vector3 beginPos, Vector3 endPos)
        {
        }


        public void SetLayer(int layer)
        {
        }

        public void Reset()
        {
        }

        public void SetEffect(BattleEffect effect)
        {
        }
        public void JointPlaySound(string name)
        {

        }
        public void PlayVoice(string name)
        {

        }

        public void SetEffectID(int id)
        {

        }

        public void AddFloorEffect(BattleActor ownerRole, int type, List<float> indicatorParams)
        {

        }

        public void OnHit(BattleEntity target)
        {

        }
    }

}