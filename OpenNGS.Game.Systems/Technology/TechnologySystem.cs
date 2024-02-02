using OpenNGS.Exchange.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Technology.Data;
using OpenNGS.Exchange.Common;
using OpenNGS.Technology.Common;
using OpenNGS.SaveData;
using OpenNGSCommon;

namespace OpenNGS.Systems
{
    public class TechnologySystem : EntitySystem, ITechnologySystem
    {
        public uint technologyDots = 0;

        public List<SourceItem> sourceItems = new List<SourceItem>();
        public List<TargetItem> targetItems = new List<TargetItem>();

        private IExchangeSystem m_exchangeSyetem = null;
        private IItemSystem m_itemSystem = null;
        private ISaveSystem m_saveSystem = null;
        private TechnologyData m_technologyData;
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override void InitSystem()
        {
            m_exchangeSyetem = App.GetService<IExchangeSystem>();
            m_itemSystem = App.GetService<IItemSystem>(); 
            m_saveSystem = App.GetService<ISaveSystem>();
            InitData();
        }

        public void InitData()
        {
            //获取存档数据
            ISaveInfo saveInfo = m_saveSystem.GetFileData("TECHNOLOGY");
            if(saveInfo != null && saveInfo is TechnologyData)
            {
                m_technologyData = (TechnologyData)saveInfo;
            }
            else
            {
                m_technologyData = new TechnologyData();
            }
            
        }
        public uint GetTechnologyDots(uint technologyDotID)
        {
            return m_itemSystem.GetItemCountByGuidID(technologyDotID);
        }
        public Dictionary<uint, NodeData> GetNodes(int treeCount)
        {
            Dictionary<uint, NodeData> technologyNodes = new Dictionary<uint, NodeData>();
            TechnologyNodeSaveData data_tmp;
            for(int i = 1; i <= treeCount; i++)
            {
                Queue<uint> queue = new Queue<uint>();
                queue.Enqueue((uint)i);
                while(queue.Count > 0)
                {
                    uint id = queue.Dequeue();
                    if (technologyNodes.ContainsKey(id))
                    {
                        continue;
                    }
                    NodeData nodeData = NGSStaticData.technologyNodes.GetItem(id);

                    //查找动态数据修改该节点状态,先判断动态数据是否有这个数据
                    if(m_technologyData.nodesSaveData.TryGetValue(id,out data_tmp))
                    {
                        nodeData.Level = data_tmp.level;
                        nodeData.Locked = data_tmp.locked;
                        nodeData.Activated = data_tmp.activated;
                    }
                    //存入数据
                    data_tmp = new TechnologyNodeSaveData();
                    data_tmp.id = nodeData.ID;
                    data_tmp.level = nodeData.Level;
                    data_tmp.locked = nodeData.Locked;
                    data_tmp.activated = nodeData.Activated;
                    m_technologyData.nodesSaveData[id] = data_tmp;

                    technologyNodes[nodeData.ID] = nodeData;
                    if (nodeData.SonNode == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < nodeData.SonNode.Length; j++)
                    {
                        queue.Enqueue(nodeData.SonNode[j]);
                    }
                }
            }

            return technologyNodes;
        }

        //升级技能
        public TECHNOLOGY_RESULT_TYPE UpgradeNode(uint technologyNodeID)
        {
            sourceItems.Clear();
            targetItems.Clear();
            //科技点数交易科技点
            SourceItem technologyDots = new SourceItem();
            TargetItem technologyNode = new TargetItem();

            NodeData tNode = NGSStaticData.technologyNodes.GetItem(technologyNodeID);
            uint id = m_itemSystem.GetGuidByItemID(tNode.CostItemID);
            technologyDots.GUID = id;//科技点数对应ItemID
            technologyDots.Count = tNode.CostItemCount;//升级科技点需要的科技点数
            sourceItems.Add(technologyDots);
            technologyNode.ItemID = tNode.ID;//科技点
            technologyNode.Count = 1;//升级科技点
            targetItems.Add(technologyNode);
            //科技点数不足
            if (m_exchangeSyetem.ExchangeItem(sourceItems, targetItems) == EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOCOUNT)
            {
                return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_COUNT;
            }

            //设置对应科技技能状态和解锁子技能
            m_technologyData.nodesSaveData[technologyNodeID].level++;
            m_technologyData.nodesSaveData[technologyNodeID].activated = true;
            if (tNode.SonNode != null)
            {
                for (int i = 0; i < tNode.SonNode.Length; i++)
                {
                    m_technologyData.nodesSaveData[tNode.SonNode[i]].locked = true;
                }
            }


            //调整玩家属性
            //xxx();

            SaveTechnologyData();
            return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_SUCCESS; 
        }

        //重置技能
        public TECHNOLOGY_RESULT_TYPE ResetNode()
        {
            //设置对应科技技能状态
            uint costSum = 0;
            NodeData tNode = null;
            foreach(var node in m_technologyData.nodesSaveData.Values)
            {
                tNode = NGSStaticData.technologyNodes.GetItem(node.id);
                node.level = 0;
                node.activated = false;
                node.locked = tNode.ParentNode != 0;
                costSum += tNode.CostItemCount;
            }
            if(tNode != null)
            {
                return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_COUNT;
            }
            m_itemSystem.AddItemsByID(costSum, tNode.CostItemID);

            //调整玩家属性
            //xxx();
            
            SaveTechnologyData();
            return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_SUCCESS;
        }
        //保存数据
        public void SaveTechnologyData()
        {
            m_saveSystem.SetFileData("TECHNOLOGY", m_technologyData);
            m_saveSystem.SaveFile();
        }
    }
}
