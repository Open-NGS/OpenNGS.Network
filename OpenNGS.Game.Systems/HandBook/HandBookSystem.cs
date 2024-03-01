using OpenNGS.Achievement.Common;
using OpenNGS.Achievement.Data;
using OpenNGS.Exchange.Common;
using OpenNGS.Exchange.Data;
using OpenNGS.HandBook.Common;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;

namespace OpenNGS.Systems
{
    public class HandBookSystem : GameSubSystem<HandBookSystem>, IHandBookSystem
    {
        private ISaveSystem m_saveSys;
        private SaveFileData_HandBook m_saveHandBook;
        private IStatSystem m_statSys;
        private bool m_bStatDirty = false;
        private IExchangeSystem m_exchangeSys;

        private uint GroupID;

        protected override void OnCreate()
        {
            m_saveSys = App.GetService<ISaveSystem>();
            m_statSys = App.GetService<IStatSystem>();
            base.OnCreate();

            ISaveInfo saveInfo = m_saveSys.GetFileData("HANDBOOK");
            if (saveInfo != null && saveInfo is SaveFileData_HandBook)
            {
                m_saveHandBook = (SaveFileData_HandBook)saveInfo;
            }
            else
            {
                m_saveHandBook = new SaveFileData_HandBook();
            }

            foreach (OpenNGS.HandBook.Data.HandBook _statData in NGSStaticData.s_handBook.Items)
            {
                if (m_saveHandBook.DicHandBook.TryGetValue(_statData.ID, out HandBookInfo _HandBookInfo) == false)
                {
                    m_saveHandBook.DicHandBook[_statData.ID] = new HandBookInfo();
                    m_saveHandBook.DicHandBook[_statData.ID].ID = _statData.ID;
                    m_saveHandBook.DicHandBook[_statData.ID].status = HANDBOOK_STATUS.HANDBOOK_STATUS_STATING;
                }
            }
            m_statSys.Subscribe((int)StatEventNotify.StatEventNotify_Update, _statUpdate);
        }

        private void _statUpdate()
        {
            m_bStatDirty = true;
        }

        public void UpdateHandBookIfNeed()
        {
            if (m_bStatDirty)
            {
                m_bStatDirty = false;
                foreach (KeyValuePair<uint, HandBookInfo> kvp in m_saveHandBook.DicHandBook)
                {
                    OpenNGS.HandBook.Data.HandBook _handBookInfo = NGSStaticData.s_handBook.GetItem(kvp.Key);
                    if (_handBookInfo != null)
                    {
                        if (m_statSys.GetStatValueByID(_handBookInfo.StatID, out ulong ulStatVal) == true)
                        {
                            if (ulStatVal > 0)
                            {
                                kvp.Value.status = HANDBOOK_STATUS.HANDBOOK_STATUS_ACTIVE;
                            }
                        }
                    }
                }
                _saveHandBook();
            }
        }
        private void _saveHandBook()
        {
            m_saveSys.SetFileData("HANDBOOK", m_saveHandBook);
            m_saveSys.SaveFile();
        }

        public void SetHandBookIndex(uint groupID)
        {
            this.GroupID = groupID;
        }

        public Dictionary<uint, HandBookInfo> GetHandBookData()
        {
            Dictionary<uint, HandBookInfo> m_handBook = new Dictionary<uint, HandBookInfo> ();
            foreach (KeyValuePair<uint, HandBookInfo> kvp in m_saveHandBook.DicHandBook)
            {
                if(GroupID == NGSStaticData.s_handBook.GetItem(kvp.Key).GroupID)
                {
                    m_handBook[kvp.Key] = kvp.Value;
                }
            }
            
            return m_saveHandBook.DicHandBook;
        }

        public HANDBOOK_STATUS GetHandBookStatus(uint nHandBookID)
        {
            HANDBOOK_STATUS _status = HANDBOOK_STATUS.HANDBOOK_STATUS_NONE;
            if (m_saveHandBook.DicHandBook.TryGetValue(nHandBookID, out HandBookInfo info))
            {
                _status = info.status;
            }

            return _status;
        }

        public override string GetSystemName()
        {
            return "com.openngs.system.handbook";
        }

        protected override void OnClear()
        {
            UpdateHandBookIfNeed();
            m_statSys.Unsubscribe(0, _statUpdate);
            base.OnClear();
        }
    }
}

