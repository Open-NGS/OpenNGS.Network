using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    
    [RequireComponent(typeof(Toggle), typeof(Animator))]
    public class CustomToggleTabItem : CustomTabItem
    {
        private Toggle m_Toggle;
        private Animator m_Animator;
        private static readonly int IsOn = Animator.StringToHash("IsOn");
        
        public override void Init(CustomTabView tabView, int index)
        {
            if (m_Index == index)
            {
                OpenNGSDebug.LogError("[CustomTabItem] Repeated Init!");
                return;
            }

            m_TabView = tabView;
            m_Index = index;
            
            m_Toggle = GetComponentInChildren<Toggle>();
            m_Animator = GetComponentInChildren<Animator>();

            var toggleGroup = tabView.GetComponent<ToggleGroup>();
            if (toggleGroup == null)
                toggleGroup = tabView.gameObject.AddComponent<ToggleGroup>();

            m_Toggle.group = toggleGroup;
            
            m_Toggle.onValueChanged.AddListener(delegate
            {
                if (m_Toggle.isOn)
                {
                    m_TabView.SelectTab(m_Index);
                }
                m_Animator.SetBool(IsOn, m_Toggle.isOn);
            });
            
            m_Animator.SetBool(IsOn, true);
        }

        public override void HighlightTab()
        {
            m_Toggle.isOn = true;
        }
    }
}