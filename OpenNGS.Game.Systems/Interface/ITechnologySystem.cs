using OpenNGS.Exchange.Common;
using System.Collections;
using System.Collections.Generic;

using OpenNGS.Technology.Common;
using OpenNGS.Technology.Data;

namespace OpenNGS.Systems
{
    public interface ITechnologySystem
    {
        //UI层初始化获取科技点数量
        public uint GetTechnologyDots(uint technologyDotID);

        //UI层初始化获取科技技能数据
        public Dictionary<uint, NodeData> InitNodes(List<uint> rootNodeIDs);
        //获取一棵树的所有结点
        public List<TechNodeSaveData> GetTreeNodes(uint treeIndex);
        //获取技能状态数据
        public TechNodeSaveData GetNodeSaveData(uint id);

        //升级科技点
        public TECHNOLOGY_RESULT_TYPE UpgradeNode(uint technologyNodeID, uint currencyID);

        //重置科技点
        public TECHNOLOGY_RESULT_TYPE ResetNode(uint currencyID);

        public void AddTechContainer(TechContainer container);

        public List<TechNodeSaveData> GetAllActivedNodes();
        public void SetTechSaveData(TechNodeSaveData data);
    }
}