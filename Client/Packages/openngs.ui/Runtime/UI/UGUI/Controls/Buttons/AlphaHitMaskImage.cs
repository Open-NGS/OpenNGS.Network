using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    public class AlphaHitMaskImage : Image
    {
        [SerializeField][Range(0.1f, 1f)] private float m_AlphaHitMinThreshold = 0.5f;
        [SerializeField] private bool m_ShowGraphic = false;

        private void Update()
        {
            if (Math.Abs(alphaHitTestMinimumThreshold - m_AlphaHitMinThreshold) > 0.01f)
                alphaHitTestMinimumThreshold = m_AlphaHitMinThreshold;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (m_ShowGraphic)
                base.OnPopulateMesh(toFill);
            else
                toFill.Clear();
        }
    }
}