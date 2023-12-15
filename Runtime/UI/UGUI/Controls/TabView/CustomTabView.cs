using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    public class CustomTabView: MonoBehaviour
    {
        public UnityAction<int> OnTabSelect;
        public List<CustomTabItem> m_TabButtons = new();
        
        [SerializeField]
        private CustomTabItem m_TabItem;
        
        // FIFO
        private Queue<GameObject> m_Pool = new();

        private int m_CurrentIndex = -1;


        public void InitTabButtonList(int startIndex = 0)
        {
            for (int i = 0; i < m_TabButtons.Count; i++)
            {
                CustomTabItem item = m_TabButtons[i];
                item.Init(this, i);
            }

            if(m_TabButtons.Count > startIndex)
                SelectTab(startIndex, true);  
        }
        
        public void Clear()
        {
            m_CurrentIndex = -1;
            m_TabButtons.Clear();
            m_Pool.Clear();
            foreach (Transform tabItem in m_TabItem.transform.parent)
            {
                tabItem.gameObject.SetActive(false);
                m_Pool.Enqueue(tabItem.gameObject);
            }
        }

        public CustomTabItem AddTabButton(int index)
        {
            var tabItem = m_Pool.Count > 0 ? m_Pool.Dequeue() : null;
            if (null == tabItem)
            {
                tabItem = Instantiate(m_TabItem.gameObject, m_TabItem.transform.parent);
            }
            tabItem.GetComponent<CustomTabItem>().Init(this, index);
            tabItem.gameObject.SetActive(true);

            var item = tabItem.GetComponent<CustomTabItem>();
            m_TabButtons.Add(item);
            return item;
        }
        
        public void SelectTab(int index, bool isCodeCall = false)
        {
            if (m_CurrentIndex == index)
            {
                return;
            }

            for (var i = 0; i < m_TabButtons.Count; i++)
            {
                m_TabButtons[i].Select(i == index);
            }

            if (isCodeCall)
            {
                m_TabButtons[index].HighlightTab();
            }

            m_CurrentIndex = index;
            OnTabSelect?.Invoke(index);
        }

        public void HighlightTab(int index)
        {
            m_CurrentIndex = index;
            m_TabButtons[index].HighlightTab();
        }

    }
}