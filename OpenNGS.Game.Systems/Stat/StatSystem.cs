using OpenNGS.Item.Common;
using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;
namespace OpenNGS.Systems
{
    public class StatSystem : GameSubSystem<StatSystem>, IStatSystem
    {
        private ISaveSystem m_saveSys;
        private SaveFileData_Stat m_saveStat;
        private OpenNGS.Events.EventSystem m_eventSys = new Events.EventSystem();
        //public void 
        protected override void OnCreate()
        {
            m_saveSys = App.GetService<ISaveSystem>();
            base.OnCreate();

            ISaveInfo saveInfo = m_saveSys.GetFileData("STAT");
            if (saveInfo != null && saveInfo is SaveFileData_Stat)
            {
                m_saveStat = (SaveFileData_Stat)saveInfo;
            }
            else
            {
                m_saveStat = new SaveFileData_Stat();
            }

            foreach (OpenNGS.Statistic.Data.StatData _statData in NGSStaticData.s_statDatas.Items)
            {
                List<OpenNGS.Statistic.Common.StatValue> _lst = null;
                StatValue _statVal = null;
                if (m_saveStat.DicStatValue.ContainsKey(_statData.StatEvent) == false)
                {
                    _lst = new List<StatValue>();
                    m_saveStat.DicStatValue.Add(_statData.StatEvent, _lst);
                }
                else
                {
                    _lst = m_saveStat.DicStatValue[_statData.StatEvent];
                }
                foreach(StatValue _statValFound in _lst)
                {
                    if(_statValFound.id == _statData.Id)
                    {
                        _statVal = _statValFound;
                        break;
                    }
                }
                if(_statVal == null)
                {
                    _statVal = new StatValue();
                    _lst.Add(_statVal);
                    _statVal.id = _statData.Id;
                    _statVal.totalval = 0;
                }
            }
        }

        protected override void OnClear() 
        { 
            base.OnClear();
        }
        public bool GetStatValueByID(uint nStatID, out ulong ulValue)
        {
            ulValue = 0;
            StatData _statData = NGSStaticData.s_statDatas.GetItem(nStatID);
            if(_statData != null)
            {
                return GetStatValue(_statData.StatEvent, nStatID, out ulValue);
            }
            return false;
        }
        public bool GetStatValue(STAT_EVENT eStatEvt, uint nStatID, out ulong ulValue)
        {
            if (m_saveStat.DicStatValue.TryGetValue(eStatEvt, out List<StatValue> _lstStatValue) == true)
            {
                foreach (StatValue _statValue in _lstStatValue)
                {
                    if(_statValue.id == nStatID)
                    {
                        ulValue = _statValue.totalval;
                        return true;
                    }
                }
            }
            ulValue = 0;
            return false;
        }
        public void UpdateStat(STAT_EVENT _statEvt, ulong lParam1, ulong lParam2)
        {
            if(m_saveStat.DicStatValue.ContainsKey(_statEvt) == true)
            {
                if(m_saveStat.DicStatValue.TryGetValue(_statEvt, out  List<StatValue> _lstStatValue) == true) 
                {
                    foreach( StatValue _statValue in _lstStatValue)
                    {
                        StatData _statData = NGSStaticData.s_statDatas.GetItem(_statValue.id);
                        if(_statData != null)
                        {
                            switch(_statEvt )
                            {
                                case STAT_EVENT.STAT_EVENT_USE_ITEM:
                                    {
                                        _updateValUseItem(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_ADD_ITEM:
                                    {
                                        _updateValAddItem(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_LEVEL_TIME:
                                    {
                                        _updateValLevelTime(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_KILL_ENEMY_COUNTS:
                                    {
                                        _updateValKillEnemy(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_UPGRADE_TECH:
                                    {
                                        _updateValUpgradeTech(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_UPGRADE_ITEM:
                                    {
                                        _updateValUpgradeItem(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_COLLECT_SUIT:
                                    {
                                        _updateValCollectSuit(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_PUZZLE_PLAY:
                                    {
                                        _updateValPuzzlePlay(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_PROCESS_MOVEON:
                                    {
                                        _updateValProcessMoveOn(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_UI_CLICK:
                                    {
                                        _updateValUIClicked(lParam1, lParam2, _statValue, _statData);
                                    }
                                    break;
                                case STAT_EVENT.STAT_EVENT_LEVEL_GOT_DAMAGE:
                                case STAT_EVENT.STAT_EVENT_LEVEL_DEATH_COUNTS:
                                case STAT_EVENT.STAT_EVENT_LEVEL_DAMAGE:
                                case STAT_EVENT.STAT_EVENT_LEVEL_DAMAGE_PER_SECOND:
                                case STAT_EVENT.STAT_EVENT_LEVEL_FINISH:
                                case STAT_EVENT.STAT_EVENT_LEVEL_SURVIVE_TIME:
                                case STAT_EVENT.STAT_EVENT_LEVEL_KILL_ENEMY:
                                default:
                                    {
                                        _updateValLevel(lParam1, lParam2, _statValue, _statData);
                                        break;
                                    }
                            }
                        }
                    }
                }
            }

            m_saveSys.SetFileData("STAT", m_saveStat);
            m_saveSys.SaveFile();

            m_eventSys.PostEvent((int)StatEventNotify.StatEventNotify_Update);
        }

        private void _updateValLevel(ulong lLevelID, ulong lParam2, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Levels.Data.NGSLevelInfo _item = NGSStaticData.levelInfo.GetItem((uint)lLevelID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)lLevelID)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += lParam2;
                }
                else if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_MAX)
                {
                    _statVal.totalval = _statVal.totalval > lParam2 ? _statVal.totalval : lParam2;
                }
                else if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_MIN)
                {
                    _statVal.totalval = _statVal.totalval < lParam2 ? _statVal.totalval : lParam2;
                }
                else if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_AVERAGE)
                {
                    _statVal.totalval = (_statVal.totalval + lParam2)/ 2;
                }
            }
        }

        //STAT_EVENT_USE_ITEM = 1,
        private void _updateValUseItem(ulong lParam1, ulong lParam2, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if(_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Item.Data.Item _item = NGSStaticData.items.GetItem((uint)lParam1);
                if(_item != null )
                {
                    if(_statData.ObjCategory == (uint)lParam1)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if(bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += lParam2;
                }
            }
        }

        private void _updateValAddItem(ulong lParam1, ulong lParam2, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Item.Data.Item _item = NGSStaticData.items.GetItem((uint)lParam1);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)lParam1)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += lParam2;
                }
            }
        }

        //STAT_EVENT_LEVEL_TIME (关卡时长)
        private void _updateValLevelTime(ulong levelID, ulong levelDuration, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Levels.Data.NGSLevelInfo _item = NGSStaticData.levelInfo.GetItem((uint)levelID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)levelID)
                    {
                        bCanUpdate = true;
                    }
                }
            }
            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_UPDATE)
                {
                    _statVal.totalval = levelDuration;
                }
                else if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_MIN)
                {
                    if (_statVal.totalval < levelDuration)
                        _statVal.totalval = levelDuration;
                    else return;
                }
                else if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_MAX)
                {
                    if (_statVal.totalval > levelDuration)
                        _statVal.totalval = levelDuration;
                    else return;
                }
            }
        }

        //STAT_EVENT_KILL_ENEMY_COUNTS（杀敌数量）
        private void _updateValKillEnemy(ulong enemyID, ulong killCount, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Enemy.Data.EnemyInfo _item = NGSStaticData.enemyInfo.GetItem((uint)enemyID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)enemyID)
                    {
                        bCanUpdate = true;
                    }
                }
            }
            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += killCount;
                }
            }
        }


        //STAT_EVENT_UPGRADE_TECH(科技升级)
        private void _updateValUpgradeTech(ulong techID, ulong levelNum, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Technology.Data.NodeData _item = NGSStaticData.technologyNodes.GetItem((uint)techID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)techID)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_UPDATE)
                {
                    _statVal.totalval = levelNum;
                }
            }
        }
        //STAT_EVENT_UPGRADE_ITEM(物品升级)
        private void _updateValUpgradeItem(ulong itemID, ulong levelNum, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Item.Data.Item _item = NGSStaticData.items.GetItem((uint)itemID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)itemID)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_UPDATE)
                {
                    _statVal.totalval = levelNum;
                }
            }
        }
        //STAT_EVENT_COLLECT_SUIT
        private void _updateValCollectSuit(ulong suitID, ulong lParam2, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.Suit.Data.SuitData _item = NGSStaticData.suitInfo.GetItem((uint)suitID);
                if (_item != null)
                {
                    if (_statData.ObjCategory == (uint)suitID)
                    {
                        bCanUpdate = true;
                    }
                }
            }

            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += 1;
                }
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_COUNT)
                {
                    _statVal.totalval += 1;
                }
            }
        }
        //STAT_EVENT_PUZZLE_PLAY
        private void _updateValPuzzlePlay(ulong lParam1, ulong lParam2, StatValue _statVal, StatData _statData)
        {
        }
        //STAT_EVENT_PROCESS_MOVEON
        private void _updateValProcessMoveOn(ulong lParam1, ulong lParam2, StatValue _statVal, StatData _statData)
        {
        }
        //STAT_EVENT_UI_CLICK
        private void _updateValUIClicked(ulong uiID, ulong lParam2, StatValue _statVal, StatData _statData)
        {
            bool bCanUpdate = false;
            if (_statData.ObjCategory == 0)
            {
                bCanUpdate = true;
            }
            else
            {
                OpenNGS.UI.Data.UIConfig _item = NGSStaticData.uiConfig.GetItem((uint)uiID);
                if (_statData.ObjCategory == (uint)uiID)
                {
                    bCanUpdate = true;
                }
            }
            if (bCanUpdate == true)
            {
                if (_statData.StatType == STAT_TYPE.STAT_TYPE_CALCULATE_SUM)
                {
                    _statVal.totalval += 1;
                }
            }
        }


        public override string GetSystemName()
        {
            return "com.openngs.system.stat";
        }

        public void Subscribe(int EventID, Action handler)
        {
            m_eventSys.Subscribe(EventID, handler);
        }
        public void Unsubscribe(int EventID, Action handler)
        {
            m_eventSys.Unsubscribe(EventID, handler);
        }
    }
}