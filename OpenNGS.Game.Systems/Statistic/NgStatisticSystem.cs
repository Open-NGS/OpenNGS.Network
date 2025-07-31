using OpenNGS;
using OpenNGS.ERPC;
using OpenNGS.Item.Data;
using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Data;
using OpenNGS.Statistic.Service;
using OpenNGS.Systems;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems;

namespace OpenNGS.Systems
{
    public class NgStatisticSystem : GameSubSystem<NgStatisticSystem>, INgStatisticSystem
    {
        private Dictionary<int, double> globalStatistics;  //全局
        //private Dictionary<int, double> gameStatistics = new Dictionary<int, double>();    //局内
        private Dictionary<uint, NgStatisticItem> Items = new Dictionary<uint, NgStatisticItem>();

        private bool loaded = false;
        private StatisticContainer m_Container = null;
        public void RegisterEventHandler(INgStatisticEvent item)
        {
            if (item.StatID == 0) return;
            var stat = this.GetItem(item.StatID);
            if (stat != null)
            {
                stat.OnValueChanged += item.OnStatValueChange;
            }
        }
        public void RemoveEventHandler(INgStatisticEvent item)
        {
            if (item.StatID == 0) return;
            var stat = this.GetItem(item.StatID);
            if (stat != null)
            {
                stat.OnValueChanged -= item.OnStatValueChange;
            }
        }

        public void AddStatContainer(StatisticContainer Container)
        {
            // 每次加载存档时，将之前存储的数据先清空，避免新开档时没有正确恢复
            Init();

            if (Container != null)
            {
                m_Container = Container;
            }
            else
            {
                m_Container = new StatisticContainer();
            }
            if(m_Container.StatisticSaveData == null)
            {
                m_Container.StatisticSaveData = new Dictionary<ulong, StatValue>();
            }

            for (int nIdx = 0; nIdx < StatisticStaticData.s_statDatas.Items.Count; nIdx++)
            {
                StatData _statDataInfo = StatisticStaticData.s_statDatas.Items[nIdx];
                if (this.Items.ContainsKey(_statDataInfo.Id) == false)
                {
                    var item = new NgStatisticItem(_statDataInfo);
                    this.Items.Add(_statDataInfo.Id, item);
                    if (m_Container.StatisticSaveData.ContainsKey(_statDataInfo.Id) == true)
                    {
                        item.Set(m_Container.StatisticSaveData[_statDataInfo.Id].totalval);
                    }
                    else
                    {
                        item.Set(0);
                    }
                    item.OnValueChanged += OnStatValueChanged;
                }
            }
        }

        public override void Init()
        {
            //SaveDataManager.Instance.OnLoaded += OnLoaded;

            this.Items.Clear();
            //foreach (var kv in AchievementConfig.GetInstance().GetStatistics())
            //{
            //    var item =  new NgStatisticItem(kv.Value);
            //    item.OnValueChanged += OnStatValueChanged;
            //    this.Items.Add(kv.Key, item);
            //}
        }

        private void OnStatValueChanged(uint statId, ulong value)
        {
            if (m_Container != null)
            {
                m_Container.SetStat(statId, value);
            }
        }

        private void OnLoaded()
        {
            //todo 状态系统存档添加
            //if (SaveDataManager.Instance.Current != null)
            //{
            //    loaded = false;
            //    AchievementSystem.GetInstance().OnLoaded();
            //    foreach (var kv in this.Items)
            //    {
            //        kv.Value.Value = 0;
            //    }
            //    this.globalStatistics = SaveDataManager.Instance.Current.Statistics;
            //    foreach (var kv in this.globalStatistics)
            //    {
            //        NgStatisticItem item = null;
            //        if (this.Items.TryGetValue(kv.Key, out item))
            //        {
            //            item.Set(kv.Value);
            //        }
            //    }
            //    loaded = true;
            //}
        }

        public void StatByStatisticID(uint statId, double val)
        {
            StatData _statData = StatisticStaticData.s_statDatas.GetItem(statId);
            if (_statData != null)
            {
                Stat(_statData.StatEvent, _statData.ObjCategory, _statData.ObjType, _statData.ObjSubType, _statData.ObjID, val);
            }
        }

        public void Stat(STAT_EVENT @event, uint category, uint type, uint subType, uint objId, double value)
        {
            foreach (var kv in this.Items)
            {
                kv.Value.Execute(@event, category, type, subType, objId, (ulong)value);
            }
        }

        public void ResetStatsByEvent(STAT_EVENT @event)
        {
            foreach (var kv in this.Items)
            {
                if (kv.Value.Config.StatEvent == @event)
                {
                    kv.Value.Set(0);
                }
            }
        }

        public int GetStatInt(uint id)
        {
            return (int)GetStat(id);
        }

        public double GetStat(uint id)
        {
            var item = this.GetItem(id);
            if (item == null) return 0;
            return item.Value;
        }

        private NgStatisticItem GetItem(uint id)
        {
            if (id == 0) return null;
            NgStatisticItem item;
            this.Items.TryGetValue(id, out item);
            return item;
        }

        //internal Dictionary<int, double> GetGameStatistic()
        //{
        //    return this.gameStatistics;
        //}

        //internal void ResetGameStats(Dictionary<int, double> stats)
        //{
        //    //this.gameStatistics.Clear();
        //    foreach (var kv in this.Items)
        //    {
        //        NgStatisticItem item = kv.Value;
        //        if (!item.Config.Global)
        //        {
        //            item.Value = 0;
        //            if (stats != null)
        //            {
        //                double val;
        //                if (stats.TryGetValue(kv.Key, out val))
        //                {
        //                    item.Value = (ulong)val;
        //                    this.gameStatistics.Add(kv.Key, val);
        //                }
        //            }
        //        }
        //    }
        //}

        public void ResetStat(uint id)
        {
            var item = this.GetItem(id);
            if (item == null) return;
            item.Set(0);
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.statistic";
        }

        protected override void OnClear()
        {
            m_Container = null;
            base.OnClear();
        }


    }
}