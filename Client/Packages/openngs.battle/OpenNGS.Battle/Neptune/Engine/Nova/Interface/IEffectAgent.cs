using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Neptune.GameData;

namespace Neptune
{

    /// <summary>
    /// IEffectJoint Interface
    /// </summary>
    public interface IEffectAgent : IBattleAgent
    {
        /// <summary>
        /// 获取 Controller
        /// </summary>
        IEffectController Controller
        {
            get;
        }
    }
}