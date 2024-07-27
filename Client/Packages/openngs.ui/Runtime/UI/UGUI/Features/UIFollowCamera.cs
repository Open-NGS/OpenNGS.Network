using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI
{
    public class UIFollowCamera : MonoBehaviour
    {
        [SerializeField] private float m_Distance = 0.5f;
        private Transform m_CamTransform;
        private Transform m_Transform;

        private void Awake()
        {
            m_Transform = transform;
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (!m_CamTransform)
            {
                if (!Camera.main)
                {
                    return;
                }

                m_CamTransform = Camera.main.transform;
            }

            var dir = m_CamTransform.forward;
            var pos0 = m_Transform.position;
            var pos1 = m_CamTransform.position + dir * m_Distance;
            
            pos1.y = pos0.y;
            m_Transform.position = pos1;

            var rot = m_CamTransform.eulerAngles;
            m_Transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        }
    }
}