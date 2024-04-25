using OpenNGS;
using OpenNGS.Item.Data;
using OpenNGS.SaveData;
using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Data;
using OpenNGS.Systems;
using System.Collections.Generic;
using Systems;

namespace OpenNGS.Systems
{
    class StatisticSystem : GameSubSystem<StatisticSystem>, IStatisticSystem
    {
        private Dictionary<int, double> globalStatistics;  //全局
        //private Dictionary<int, double> gameStatistics = new Dictionary<int, double>();    //局内
        private Dictionary<int, StatisticItem> Items = new Dictionary<int, StatisticItem>();

        private bool loaded = false;
        private StatisticContainer m_Container = null;
        public void RegisterEventHandler(IStatisticEvent item)
        {
            if (item.StatID == 0) return;
            var stat = this.GetItem(item.StatID);
            if (stat != null)
            {
                stat.OnValueChanged += item.OnStatValueChange;
            }
        }
        public void AddStatContainer(StatisticContainer Container)
        {
            if (Container != null)
            {
                m_Container = Container;
            }
            else
            {
                m_Container = new StatisticContainer();
            }
        }

        public override void Init()
        {
            //SaveDataManager.Instance.OnLoaded += OnLoaded;

            this.Items.Clear();
            //foreach (var kv in AchievementConfig.GetInstance().GetStatistics())
            //{
            //    var item =  new StatisticItem(kv.Value);
            //    item.OnValueChanged += OnStatValueChanged;
            //    this.Items.Add(kv.Key, item);
            //}
        }

        private void OnStatValueChanged(uint statId, ulong value)
        {
            if(m_Container != null)
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
            //        StatisticItem item = null;
            //        if (this.Items.TryGetValue(kv.Key, out item))
            //        {
            //            item.Set(kv.Value);
            //        }
            //    }
            //    loaded = true;
            //}
        }

        public void Stat(STAT_EVENT @event, int category, int type, int subType, int objId, double value)
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

        public int GetStatInt(int id)
        {
            return (int)GetStat(id);
        }

        public double GetStat(int id)
        {
            var item = this.GetItem(id);
            if (item == null) return 0;
            return item.Value;
        }

        private StatisticItem GetItem(int id)
        {
            if (id == 0) return null;
            StatisticItem item;
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
        //        StatisticItem item = kv.Value;
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

        public void ResetStat(int id)
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