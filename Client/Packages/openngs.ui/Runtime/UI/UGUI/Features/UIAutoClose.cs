using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OpenNGS.UI
{
    [RequireComponent(typeof(UView))]
    public class UIAutoClose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum TriggerType
        {
            NoInteraction,      // 一段时间没有交互
            LongDistance,       // 超出一定距离
            Both,               // 超出距离或者一段时间没有交互
        }

        public enum ActionType
        {
            Close,
            Hide,
        }
        
        
        [SerializeField] private TriggerType m_Trigger = TriggerType.NoInteraction;
        [SerializeField] private ActionType m_Action = ActionType.Hide;
        [SerializeField] private float m_NoInteractionTimeThreshold = 5f;
        [SerializeField] private float m_DistanceThreshold = 10f;
        
        private float m_NoInteractionTime = 0f;
        private Transform m_CameraTransform;

        public TriggerType Trigger => m_Trigger;

        private UView m_View;

        private Action m_OnHideCallback;
        private Action m_OnCloseCallback;
        private void Awake()
        {
            m_View = GetComponent<UView>();
        }

        private void OnEnable()
        {
            m_NoInteractionTime = 0f;
        }

        void Start()
        {
            
        }
        
        public void ResetNoInteractionTime()
        {
            m_NoInteractionTime = 0f;
        }

        public void SetHideCallback(Action action)
        {
            m_OnHideCallback = action;
        }
        
        public void SetCloseCallback(Action action)
        {
            m_OnCloseCallback = action;
        }

        private void OnDestroy()
        {
            m_OnCloseCallback = null;
            m_OnHideCallback = null;
        }

        void Update()
        {
            if (m_Trigger == TriggerType.NoInteraction || m_Trigger == TriggerType.Both)
            {
                CheckIfNoInteractionForAWhile();    
            }

            if (m_Trigger == TriggerType.LongDistance || m_Trigger == TriggerType.Both)
            {
                CheckIfLongDistance();
            }
        }

        private void CheckIfNoInteractionForAWhile()
        {
            m_NoInteractionTime += UnityEngine.Time.deltaTime;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                
            }

            if (m_NoInteractionTime >= m_NoInteractionTimeThreshold)
            {
                CloseView();
            }
        }

        private void CheckIfLongDistance()
        {
            if (!m_CameraTransform)
            {
                if (Camera.main == null)
                {
                    return;
                }

                m_CameraTransform = Camera.main.transform;
            }

            var dis = Vector3.Distance(m_CameraTransform.position, transform.position);
            if (dis >= m_DistanceThreshold)
            {
                CloseView();
            }
        }

        private void CloseView()
        {
            if (m_View != null)
            {
                if (m_Action == ActionType.Close)
                {
                    m_View.Close();
                    m_OnCloseCallback?.Invoke();
                }
                else
                {
                    m_View.Hide();
                    m_OnHideCallback?.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_NoInteractionTime = 0f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_NoInteractionTime = 0f;
        }
    }
}
