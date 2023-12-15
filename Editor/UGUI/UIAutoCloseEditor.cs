using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenNGS.UI
{
    [CustomEditor(typeof(UIAutoClose), true)]
    public class UIAutoCloseEditor : Editor
    {
        private UIAutoClose m_UIAutoClose;
        private SerializedProperty m_Trigger;
        private SerializedProperty m_Action;
        private SerializedProperty m_NoInteractionTimeThreshold;
        private SerializedProperty m_DistanceThreshold;
        private GUIContent m_NoInteractionTimeContent;
        private void OnEnable()
        {
            m_UIAutoClose = target as UIAutoClose;
            m_Trigger = serializedObject.FindProperty("m_Trigger");
            m_Action = serializedObject.FindProperty("m_Action");
            m_NoInteractionTimeThreshold = serializedObject.FindProperty("m_NoInteractionTimeThreshold");
            m_DistanceThreshold = serializedObject.FindProperty("m_DistanceThreshold");
            m_NoInteractionTimeContent = EditorGUIUtility.TrTextContent("Time to close after no interaction");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Trigger);
            EditorGUILayout.PropertyField(m_Action);
            
            switch (m_UIAutoClose.Trigger)
            {
                case UIAutoClose.TriggerType.Both:
                    EditorGUILayout.PropertyField(m_NoInteractionTimeThreshold, m_NoInteractionTimeContent);
                    EditorGUILayout.PropertyField(m_DistanceThreshold);
                    break;
                case UIAutoClose.TriggerType.NoInteraction:
                    EditorGUILayout.PropertyField(m_NoInteractionTimeThreshold, m_NoInteractionTimeContent);
                    break;
                case UIAutoClose.TriggerType.LongDistance:
                    EditorGUILayout.PropertyField(m_DistanceThreshold);
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
