using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.Statistic.Data;
using System.Collections;
using System.Collections.Generic;
using Systems;

namespace OpenNGS.Systems
{
    public class AchievementSystem : GameSubSystem<AchievementSystem>, IAchievementSystem
    {
        //private ISaveSystem m_saveSys;
        //private SaveFileData_Achievement m_saveAchi;
        private IStatSystem m_statSys;
        private bool m_bStatDirty = false;
        private IExchangeSystem m_exchangeSys;
        protected override void OnCreate()
        {
            //m_saveSys = App.GetService<ISaveSystem>();
            m_statSys = App.GetService<IStatSystem>();
            m_exchangeSys = App.GetService<IExchangeSystem>();
            base.OnCreate();

            //ISaveInfo saveInfo = m_saveSys.GetFileData("ACHIEVEMENT");
            //SaveFileData saveData = m_saveSys.GetFileData();
            //SaveFileData_Achievement saveInfo = saveData.achiData;
            //if (saveInfo != null && saveInfo is SaveFileData_Achievement)
            //{
            //    m_saveAchi = (SaveFileData_Achievement)saveInfo;
            //}
            //else
            //{
            //    m_saveAchi = new SaveFileData_Achievement();
            //}
            //foreach (OpenNGS.Achievement.Data.Achievement _statData in NGSStaticData.s_achiDatas.Items)
            //{
            //    if(m_saveAchi.DicAchievement.TryGetValue(_statData.ID, out AchievementInfo _achiInfo) == false)
            //    {
            //        m_saveAchi.DicAchievement[_statData.ID] = new AchievementInfo();
            //        m_saveAchi.DicAchievement[_statData.ID].ID = _statData.ID;
            //        m_saveAchi.DicAchievement[_statData.ID].status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_STATING;
            //        m_saveAchi.DicAchievement[_statData.ID].value = 0;
            //    }
            //    else
            //    {

            //    }
            //}
            //m_statSys.Subscribe((int)StatEventNotify.StatEventNotify_Update,_statUpdate);
        }
        private void _statUpdate()
        {
            m_bStatDirty = true;
        }
        public void UpdateAchievementIfNeed()
        {
            //if(m_bStatDirty)
            //{
            //    m_bStatDirty = false;
            //    foreach(KeyValuePair<uint, AchievementInfo> kvp in m_saveAchi.DicAchievement)
            //    {
            //        OpenNGS.Achievement.Data.Achievement _achiInfo = NGSStaticData.s_achiDatas.GetItem(kvp.Value.ID);
            //        if(_achiInfo != null)
            //        {
            //            if( m_statSys.GetStatValueByID(_achiInfo.StatID, out ulong ulStatVal) == true )
            //            {
            //                if ( kvp.Value.value < ulStatVal )
            //                {
            //                    kvp.Value.value = (uint)ulStatVal;
            //                    if(kvp.Value.value > _achiInfo.StatValue)
            //                    {
            //                        kvp.Value.status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_PENDING;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    _saveAchievement();
            //}
        }
        private void _saveAchievement()
        {
            //m_saveSys.SetFileData("ACHIEVEMENT", m_saveAchi);
            //SaveFileData saveData = m_saveSys.SetFileData();
            //m_itemSys.AddItemContainer(saveData.itemContainer);
            //m_saveSys.SaveFile();
        }
        public Dictionary<uint, AchievementInfo> GetAchievementData()
        {
            return null;
        }
        public ACHIEVEMENT_STATUS GetAchievementStatus(uint nAchieID)
        {
            ACHIEVEMENT_STATUS _status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_NONE;
            return _status;
        }

        public ACHIEVEMENT_RESULT GetAchievementReward(uint nAchieID)
        {
            ACHIEVEMENT_RESULT _res = ACHIEVEMENT_RESULT.ACHIEVEMENT_RESULT_NONE;
            //if (m_saveAchi.DicAchievement.ContainsKey(nAchieID))
            //{
            //    m_saveAchi.DicAchievement[nAchieID].status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_DONE;
            //    _saveAchievement();

            //    List<SourceItem> lstSource = new List<SourceItem>();
            //    List<TargetItem> lstTarget = new List<TargetItem>();

            //    List<AchievementAward> _lstAward = NGSStaticData.s_achiAward.GetItems(nAchieID);
            //    foreach(AchievementAward _award in _lstAward)
            //    {
            //        TargetItem _target = new TargetItem();
            //        _target.ItemID = _award.ItemID;
            //        _target.Count = _award.Counts;
            //        lstTarget.Add(_target);
            //    }
            //    EXCHANGE_RESULT_TYPE _ExchangeRes = m_exchangeSys.ExchangeItem(lstSource, lstTarget);
            //    switch(_ExchangeRes)
            //    {
            //        case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOENOUGH:
            //            {
            //                _res = ACHIEVEMENT_RESULT.ACHIEVEMENT_RESULT_AWARD_NOCOUNTS;
            //            }
            //            break;
            //        case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_SUCCESS:
            //            {
            //                _res = ACHIEVEMENT_RESULT.ACHIEVEMENT_RESULT_AWARD_SUCCESS;
            //            }
            //            break;
            //        case EXCHANGE_RESULT_TYPE.EXCHANGE_RESULT_TYPE_NOITEM:
            //            {
            //                _res = ACHIEVEMENT_RESULT.ACHIEVEMENT_RESULT_AWARD_ERROR_ITEMS;
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else
            //{
            //    _res = ACHIEVEMENT_RESULT.ACHIEVEMENT_RESULT_ACHI_NOT_EXIST;
            //}
            return _res;
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.achievement";
        }
        protected override void OnClear() 
        {
            UpdateAchievementIfNeed();
            m_statSys.Unsubscribe(0, _statUpdate);
            base.OnClear();
        }
    }
}
