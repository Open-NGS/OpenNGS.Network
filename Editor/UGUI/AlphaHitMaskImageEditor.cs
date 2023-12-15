using System.Collections;
using System.Collections.Generic;
using OpenNGS.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    [CustomEditor(typeof(AlphaHitMaskImage), true)]
    public class AlphaHitMaskImageEditor : ImageEditor
    {
        private SerializedProperty m_AlphaHitMinThreshold;
        private SerializedProperty m_ShowGraphic;

        private AlphaHitMaskImage m_Image;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Image = (AlphaHitMaskImage) target;
            m_AlphaHitMinThreshold = serializedObject.FindProperty("m_AlphaHitMinThreshold");
            m_Image.alphaHitTestMinimumThreshold = m_AlphaHitMinThreshold.floatValue;
            m_ShowGraphic = serializedObject.FindProperty("m_ShowGraphic");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpriteGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AlphaHitMinThreshold);
            if (EditorGUI.EndChangeCheck())
            {
                m_Image.alphaHitTestMinimumThreshold = m_AlphaHitMinThreshold.floatValue;
            }

            EditorGUILayout.PropertyField(m_ShowGraphic);

            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
