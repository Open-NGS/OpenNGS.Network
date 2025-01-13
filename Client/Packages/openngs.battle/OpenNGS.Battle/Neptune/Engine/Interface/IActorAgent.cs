using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.Datas;


namespace Neptune
{
    /// <summary>
    /// ActorAgent Interface, used to controll display object
    /// </summary>
    public interface IActorAgent : IAgent
    {
        /// <summary>
        /// 获取或设置是否禁用
        /// </summary>
        bool Disable { get; set; }

        /// <summary>
        /// 获取 Controller
        /// </summary>
        IActorController Controller { get; }

        /// <summary>
        /// 创建控制器
        /// </summary>
        void CreateController();
        /// <summary>
        /// 设置方向2D
        /// </summary>
        /// <param name="dir"></param>
        void SetDirection(Vector2 dir);
        /// <summary>
        /// 设置方向3D
        /// </summary>
        /// <param name="dir"></param>
        void SetDirection(Vector3 dir);
        void Jump(float repelHeight, float repelGravityFactor);
        /// <summary>
        /// 在角色身上增加特效，通常为 Ability 效果等等
        /// </summary>
        /// <param name="effect">特效名称</param>
        /// <param name="type">特效类型</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="source">特效来源</param>
        /// <param name="bindingNode">绑定节点</param>
        /// <returns>Effect ID</returns>
        int AddEffect(string effect, EffectType type, Vector3 offset, IActorController source = null, string bindingNode = "");
        /// <summary>
        /// 移除指定特效
        /// </summary>
        /// <param name="id">Effect ID</param>
        void RemoveEffect(int id);
        /// <summary>
        /// 移除指定特效
        /// </summary>
        /// <param name="name">Effect 名称</param>
        void RemoveEffect(string name);
        /// <summary>
        /// 设置特效层数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="layer"></param>
        void SetEffectLayer(int id, int layer);
        /// <summary>
        /// 设置可见性
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="isTrasnparent"></param>
        void SetVisible(bool visible, bool isTrasnparent);
        /// <summary>
        /// 设置角色效果
        /// </summary>
        /// <param name="fx"></param>
        /// <returns></returns>
        int SetRoleFX(int fx);
        /// <summary>
        /// 重置角色效果
        /// </summary>
        /// <param name="id"></param>
        void ResetRoleFX(int id);
        /// <summary>
        /// 播放随机声音
        /// </summary>
        /// <param name="name"></param>
        /// <param name="playnow"></param>
        void PlayRandomVoice(List<string> name, bool playnow = false);
        /// <summary>
        /// 新角色出生
        /// </summary>
        /// <param name="index">一组角色部署时的索引</param>
        void OnBorn(int index);
        /// <summary>
        /// 受伤事件通知
        /// </summary>
        void OnDamaged();
        /// <summary>
        /// 死亡事件通知
        /// </summary>
        void OnDeath();
        /// <summary>
        /// 新动作通知
        /// </summary>
        /// <param name="immediately"></param>
        void OnNewAction(bool immediately = false);

        void DoActionEvent(ActionEventType actionEventType);
        void OnDodge();
    }

}