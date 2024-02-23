using OpenNGS.Achievement.Common;
using OpenNGS.Statistic.Data;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;

namespace OpenNGS.Systems
{
    public class AchievementSystem : GameSubSystem<AchievementSystem>, IAchievementSystem
    {
        private ISaveSystem m_saveSys;
        private SaveFileData_Achievement m_saveAchi;
        private IStatSystem m_statSys;
        private bool m_bStatDirty = false; 
        protected override void OnCreate()
        {
            m_saveSys = App.GetService<ISaveSystem>();
            m_statSys = App.GetService<IStatSystem>();
            base.OnCreate();

            ISaveInfo saveInfo = m_saveSys.GetFileData("ACHIEVEMENT");
            if (saveInfo != null && saveInfo is SaveFileData_Stat)
            {
                m_saveAchi = (SaveFileData_Achievement)saveInfo;
            }
            else
            {
                m_saveAchi = new SaveFileData_Achievement();
            }
            foreach (OpenNGS.Achievement.Data.Achievement _statData in NGSStaticData.s_achiDatas.Items)
            {
                if(m_saveAchi.DicAchievement.TryGetValue(_statData.ID, out AchievementInfo _achiInfo) == false)
                {
                    m_saveAchi.DicAchievement[_statData.ID] = new AchievementInfo();
                    m_saveAchi.DicAchievement[_statData.ID].ID = _statData.ID;
                    m_saveAchi.DicAchievement[_statData.ID].status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_STATING;
                    m_saveAchi.DicAchievement[_statData.ID].value = 0;
                }
                else
                {

                }
            }
            m_statSys.Subscribe((int)StatEventNotify.StatEventNotify_Update,_statUpdate);
        }
        private void _statUpdate()
        {
            m_bStatDirty = true;
        }
        public void UpdateAchievementIfNeed()
        {
            if(m_bStatDirty)
            {
                m_bStatDirty = false;
                foreach(KeyValuePair<uint, AchievementInfo> kvp in m_saveAchi.DicAchievement)
                {
                    OpenNGS.Achievement.Data.Achievement _achiInfo = NGSStaticData.s_achiDatas.GetItem(kvp.Value.ID);
                    if(_achiInfo != null)
                    {
                        if( m_statSys.GetStatValueByID(_achiInfo.StatID, out ulong ulStatVal) == true )
                        {
                            if ( kvp.Value.value < ulStatVal )
                            {
                                kvp.Value.value = (uint)ulStatVal;
                                if(kvp.Value.value > _achiInfo.StatValue)
                                {
                                    kvp.Value.status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_PENDING;
                                }
                            }
                        }
                    }
                }
                _saveAchievement();
            }
        }
        private void _saveAchievement()
        {
            m_saveSys.SetFileData("ACHIEVEMENT", m_saveAchi);
            m_saveSys.SaveFile();
        }
        public Dictionary<uint, AchievementInfo> GetAchievementData()
        {
            return m_saveAchi.DicAchievement;
        }
        public ACHIEVEMENT_STATUS GetAchievementStatus(uint nAchieID)
        {
            ACHIEVEMENT_STATUS _status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_NONE;
            return _status;
        }

        public bool GetAchievementReward(uint nAchieID)
        {
            if(m_saveAchi.DicAchievement.ContainsKey(nAchieID))
            {
                m_saveAchi.DicAchievement[nAchieID].status = ACHIEVEMENT_STATUS.ACHIEVEMENT_STATUS_DONE;
                _saveAchievement();
                //todo 交易系统处理物品
                return true;
            }
            return false;
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
