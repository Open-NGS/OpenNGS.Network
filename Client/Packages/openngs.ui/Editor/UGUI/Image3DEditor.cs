using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    /// <summary>
    /// Custom Editor for Image3D Component
    /// </summary>
    [CustomEditor(typeof(Image3D), true)]
    public class Image3DEditor : ImageEditor
    {
        protected SerializedProperty m_Sprite3D;
        protected SerializedProperty m_Thickness;
        protected SerializedProperty m_SideColor;
        protected SerializedProperty m_SpriteType;
        protected SerializedProperty m_Type;

        private Image3D m_Image3D;
        protected override void OnEnable()
        {
            m_Sprite3D = serializedObject.FindProperty("sprite3D");
            m_Thickness = serializedObject.FindProperty("thickness");
            m_SideColor = serializedObject.FindProperty("sideColor");

            m_SpriteType = serializedObject.FindProperty("m_SpriteType");
            m_Type = serializedObject.FindProperty("m_Type");
            
            m_Image3D = target as Image3D;
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Sprite3D);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_Image3D.sprite3D != null && m_Image3D.sprite3D.type == Sprite3D.Type.Sprite2D && m_Image3D.sprite3D.sprite2D != m_Image3D.sprite)
                {
                    m_Image3D.sprite = m_Image3D.sprite3D.sprite2D;
                }
            }
            
            
            EditorGUILayout.PropertyField(m_Thickness);
            
            EditorGUILayout.PropertyField(m_Color);
            if (m_Image3D.sprite3D != null && m_Image3D.sprite3D.type == Sprite3D.Type.Sprite2D)
            {
                if (!m_Image3D.sprite3D.generateUv)
                    EditorGUILayout.PropertyField(m_SideColor);
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_SpriteType);
                if (EditorGUI.EndChangeCheck())
                {
                    switch ((Image3D.SpriteType)m_SpriteType.enumValueIndex)
                    {
                        case Image3D.SpriteType.Simple:
                            m_Type.enumValueIndex = (int)Image.Type.Simple;
                            break;
                        case Image3D.SpriteType.Sliced:
                            m_Type.enumValueIndex = (int)Image.Type.Sliced;
                            break;
                    }
                }
            }

            EditorGUILayout.PropertyField(m_Material);
            
            RaycastControlsGUI();
            MaskableControlsGUI();
            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed)
            {
                m_Image3D.SetAllDirty();
            }
        }
    }
}