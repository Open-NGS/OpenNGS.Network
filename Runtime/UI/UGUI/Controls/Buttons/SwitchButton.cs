using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OpenNGS.UI
{
    public class SwitchButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject m_OffGameObject, m_OnGameObject;
        
        [SerializeField]
        private bool isOn = false;
        
        [Serializable]
        public class SwitchEvent : UnityEvent<bool>
        {}

        public SwitchEvent OnSwitchEvent = new SwitchEvent();
        
        public bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                if (m_OffGameObject != null)
                    m_OffGameObject.SetActive(!isOn);
                
                if (m_OnGameObject != null)
                    m_OnGameObject.SetActive(isOn);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            IsOn = !isOn;
            
            OnSwitchEvent.Invoke(isOn);
        }

    }
}