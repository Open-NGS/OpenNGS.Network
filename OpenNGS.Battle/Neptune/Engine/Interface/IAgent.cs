using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.Datas;

namespace Neptune
{
    /// <summary>
    /// Agent Interface, used to controll display object
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// 获取 Joint 所属的元件
        /// </summary>
        Entity Entity { get; }

        /// <summary>
        /// 获取是否拥有游戏对象
        /// </summary>
        bool HaveGameObject { get; }

        /// <summary>
        /// 获取对象是否运行
        /// </summary>
        /// <returns></returns>
        bool IsRunning { get; }

        /// <summary>
        /// 暂停
        /// </summary>
        void Suspend();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void Resume();

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="pos"></param>
        void SetPosition(Vector2 pos);

        /// <summary>
        /// 着色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        void SetColor(float r, float g, float b);

        /// <summary>
        /// 设置高亮状态
        /// </summary>
        /// <param name="flag">是否设置高亮</param>
        void Highlight(bool flag);

        /// <summary>
        /// 终结Joint对象
        /// </summary>
        void Stop(float delay = 0);

        /// <summary>
        /// 每帧处理
        /// </summary>
        /// <param name="passTime"></param>
        void OnEnterFrame(float passTime);
        void PlaySound(string name);
        void StopSound();

        void PlayVoice(string name);
        void StopVoice();

        void OnHit(Entity target);
    }

}