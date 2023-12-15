using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    public class CustomTabItem : MonoBehaviour
    {
        protected CustomTabView m_TabView;
        protected int m_Index = -1;

        private void Awake()
        {
            //GetComponentInChildren<Button>().onClick.AddListener(() => m_TabView.SelectTab(m_Index));
        }

        public virtual void Init(CustomTabView tabView, int index)
        {
            if (m_Index == index)
            {
                OpenNGSDebug.LogError("[CustomTabItem] Repeated Init!");
                return;
            }

            m_TabView = tabView;
            m_Index = index;

            GetComponentInChildren<Button>().onClick.AddListener(() => m_TabView.SelectTab(m_Index));
        }

        public virtual void Select(bool select)
        {
            // tabImage.overrideSprite = select ? activeImage : normalImage;
        }

        public virtual void HighlightTab()
        {
            GetComponentInChildren<Button>().Select();
        }
    }
}