using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class LabelAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public LabelAttribute(string label)
    {
        Label = label;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var labelAttri = attribute as LabelAttribute;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, new GUIContent(labelAttri.Label), true);
        EditorGUI.EndProperty();
    }
}

#endif