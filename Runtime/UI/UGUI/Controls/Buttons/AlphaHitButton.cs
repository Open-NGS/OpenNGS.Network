using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    [RequireComponent(typeof(AlphaHitMaskImage))]
    public class AlphaHitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,  IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_Image;

        [SerializeField] private Sprite m_SpriteNormal;
        [SerializeField] private Sprite m_SpriteHover;
        [SerializeField] private Sprite m_SpritePress;
        
        [SerializeField] private Color m_ColorNormal = Color.white;
        [SerializeField] private Color m_ColorHover = Color.white;
        [SerializeField] private Color m_ColorPress = Color.white;
        
        [SerializeField] private Button.ButtonClickedEvent m_OnClick = new ();

        private void Start() { }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (m_Image != null)
            {
                m_Image.color = m_ColorHover;
                
                if (m_SpriteHover != null)
                {
                    m_Image.sprite = m_SpriteHover;
                }
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (m_Image != null)
            {
                m_Image.color = m_ColorNormal;
                if (m_SpriteNormal != null)
                {
                    m_Image.sprite = m_SpriteNormal;
                }
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (m_Image != null)
            {
                m_Image.color = m_ColorPress;
                if (m_SpritePress != null)
                {
                    m_Image.sprite = m_SpritePress;
                }
            }
            m_OnClick.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (m_Image != null)
            {
                m_Image.color = m_ColorNormal;
                if (m_SpriteNormal != null)
                {
                    m_Image.sprite = m_SpriteNormal;    
                }
            }
        }
    }
}