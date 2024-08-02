using OpenNGS.Exchange.Data;
using System.Collections.Generic;
using OpenNGS.Technology.Data;
using OpenNGS.Exchange.Common;
using OpenNGS.Technology.Common;
using Systems;
using OpenNGS.Item.Service;
using OpenNGS.Exchange.Service;


namespace OpenNGS.Systems
{
    public class TechnologySystem : GameSubSystem<TechnologySystem>, ITechnologySystem
    {
        public uint technologyDots = 0;

        public List<SourceItem> sourceItems = new List<SourceItem>();
        public List<TargetItem> targetItems = new List<TargetItem>();

        private INgExchangeSystem m_exchangeSyetem = null;
        private INgItemSystem m_itemSystem = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_itemSystem = App.GetService<INgItemSystem>();
            m_exchangeSyetem = App.GetService<INgExchangeSystem>();

        }

        TechContainer TechContainer;

        public void AddTechContainer(TechContainer container)
        {
            if (container != null)
            {
                TechContainer = container;
            }
            else
            {
                TechContainer = new TechContainer();
            }
        }

        public uint GetTechnologyDots(uint technologyDotID)
        {
            //return m_itemSystem.GetItemCountByGuidID(technologyDotID);
            return 1;
        }

        public Dictionary<uint, NodeData> InitNodes(List<uint> rootNodeIDs)
        {
            Dictionary<uint, NodeData> technologyNodes = new Dictionary<uint, NodeData>();
            TechNodeSaveData data_tmp;
            for(int i = 0; i < rootNodeIDs.Count; i++)
            {
                Queue<uint> queue = new Queue<uint>();
                queue.Enqueue(rootNodeIDs[i]);
                while(queue.Count > 0)
                {
                    uint id = queue.Dequeue();
                    if (technologyNodes.ContainsKey(id))
                    {
                        continue;
                    }
                    NodeData nodeData = NGSStaticData.technologyNodes.GetItem(id);

                    data_tmp = TechContainer.GetTechNodeById(id);
                    //查找动态数据修改该节点状态,先判断动态数据是否有这个数据
                    if (data_tmp != null)
                    {
                        nodeData.Level = data_tmp.Level;
                        nodeData.Locked = data_tmp.Locked;
                        nodeData.Activated = data_tmp.Activated;
                    }
                    //存入数据
                    data_tmp = new TechNodeSaveData();
                    data_tmp.ID = nodeData.ID;
                    data_tmp.Level = nodeData.Level;
                    data_tmp.Locked = nodeData.Locked;
                    data_tmp.Activated = nodeData.Activated;
                    TechContainer.SetTechNode(data_tmp);

                    technologyNodes[nodeData.ID] = nodeData;
                    if (nodeData.ChildNodes == null)
                    {
                        continue;
                    }
                    for (int j = 0; j < nodeData.ChildNodes.Length; j++)
                    {
                        queue.Enqueue(nodeData.ChildNodes[j]);
                    }
                }
            }

            return technologyNodes;
        }
        //获取一棵树的所有结点
        public List<TechNodeSaveData> GetTreeNodes(uint treeIndex)
        {
            List<TechNodeSaveData> nodeDatas = new List<TechNodeSaveData>();
            foreach(var node in TechContainer.techDict)
            {
                if(node.ID == treeIndex)
                {
                    nodeDatas.Add(node);
                }
            }
            return nodeDatas;
        }

        //升级技能
        public TECHNOLOGY_RESULT_TYPE UpgradeNode(uint technologyNodeID, uint currencyID)
        {
            //已升级过
            if (TechContainer.GetTechNodeById(technologyNodeID).Activated)
            {
                return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_ERROR_UPGRADE;
            }
            //未解锁
            if (TechContainer.GetTechNodeById(technologyNodeID).Locked)
            {
                return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_UNLOCK;
            }

            sourceItems.Clear();
            targetItems.Clear();
            //科技点数交易科技点
            SourceItem technologyDots = new SourceItem();
            TargetItem technologyNode = new TargetItem();

            NodeData tNode = NGSStaticData.technologyNodes.GetItem(technologyNodeID);

            ////暂时还没定科技点资源的具体数据(ID)，和其他道具一起定义
            //uint id = m_itemSystem.GetGuidByItemID(currencyID);
            //technologyDots.GUID = id;//科技点数对应ItemID
            //technologyDots.Count = tNode.CostItemCount;//升级科技点需要的科技点数
            //sourceItems.Add(technologyDots);
            //technologyNode.ItemID = tNode.ID;//科技点
            //technologyNode.Count = 1;//升级科技点
            //targetItems.Add(technologyNode);
            ////科技点数不足
            //switch(m_exchangeSyetem.ExchangeItem(sourceItems, targetItems))
            //{
            //    case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NONE:
            //        return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NONE;
            //    case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH:
            //        return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_COUNT;
            //    case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM:
            //        return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_ERROR_UPGRADE;
            //}

            ExchangeByItemIDReq request = new ExchangeByItemIDReq();
            ItemSrcState source = new ItemSrcState();
            source.ItemID = currencyID;
            source.Counts = tNode.CostItemCount;
            request.Source.Add(source);
            ExchangeRsp reult = m_exchangeSyetem.ExchangeItemByID(request);
            switch (reult.result)
            {
                case ExchangeResultType.Failed_NotEnough:
                    return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_COUNT;
                case ExchangeResultType.Error_NotExist_Source:
                    return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_ERROR_UPGRADE;
            }

            //设置对应科技技能状态和解锁子技能
            TechContainer.GetTechNodeById(technologyNodeID).Level++;
            TechContainer.GetTechNodeById(technologyNodeID).Activated = true;
            if (tNode.ChildNodes != null)
            {
                for (int i = 0; i < tNode.ChildNodes.Length; i++)
                {
                    TechContainer.GetTechNodeById(tNode.ChildNodes[i]).Locked = false;
                }
            }

            //调整玩家属性
            //xxx();

            return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_SUCCESS; 
        }

        //重置技能
        public TECHNOLOGY_RESULT_TYPE ResetNode(uint currencyID)
        {
            //设置对应科技技能状态
            uint costSum = 0;
            NodeData tNode = null;
            foreach(TechNodeSaveData data in TechContainer.techDict)
            {
                tNode = NGSStaticData.technologyNodes.GetItem(data.ID);
                if (TechContainer.GetTechNodeById(data.ID).Activated)
                {
                    costSum += tNode.CostItemCount;
                }
                TechContainer.GetTechNodeById(data.ID).Level = 0;
                TechContainer.GetTechNodeById(data.ID).Activated = false;
                TechContainer.GetTechNodeById(data.ID).Locked = tNode.ParentNode != null;

                //m_itemSystem.RemoveItemsByID(data.ID, 1);
            }
            if(tNode == null)
            {
                return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_NO_COUNT;
            }
            AddItemReq req = new AddItemReq();
            req.ColIdx = 1;
            req.ItemID = currencyID;
            req.Counts = costSum;
            m_itemSystem.AddItemByID(req);

            //调整玩家属性
            //xxx();

            return TECHNOLOGY_RESULT_TYPE.TECHNOLOGY_RESULT_TYPE_SUCCESS;
        }

        //获取技能状态数据
        public TechNodeSaveData GetNodeSaveData(uint id)
        {
            return TechContainer.GetTechNodeById(id);
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.technology";
        }

        public List<TechNodeSaveData> GetAllActivedNodes()
        {
            List<TechNodeSaveData> result = new List<TechNodeSaveData>();
            foreach(TechNodeSaveData data in TechContainer.techDict)
            {
                if(data.Activated == true)
                {
                    result.Add(data);
                }
            }
            return result;
        }
    }
}
