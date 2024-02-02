using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Technology.Common;
using OpenNGS.Technology.Data;

namespace OpenNGS.Systems
{
    public interface ITechnologySystem
    {
        //UI层初始化获取科技点数量
        public uint GetTechnologyDots(uint technologyDotID);
        //UI层初始化获取科技技能数据

        public Dictionary<uint, NodeData> GetNodes(int treeCount);

        //升级科技点
        public TECHNOLOGY_RESULT_TYPE UpgradeNode(uint id);
        //重置科技点
        public TECHNOLOGY_RESULT_TYPE ResetNode();
    }
}