using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI
{
    public class UIBillboard : MonoBehaviour
    {
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

            var pos = m_Transform.position;
            var lookAtPos = pos + pos - m_CamTransform.position;
            m_Transform.LookAt(lookAtPos);
        }
    }
}
