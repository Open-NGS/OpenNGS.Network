using System;
using System.Collections.Generic;

namespace Neptune.GameData
{
    /// <summary>
    /// 特效数据
    /// </summary>
    public class EffectData
    {
        /// <summary>
        /// 特效名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 特效播放类型
        /// </summary>
        public EffectPlayType Type { get; set; }
        /// <summary>
        /// 位置参考类型
        /// </summary>
        public EffectPosRefType RefPos { get; set; }
        /// <summary>
        /// 位置参考偏移
        /// </summary>
        public EffectDirRefType RefDir { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}