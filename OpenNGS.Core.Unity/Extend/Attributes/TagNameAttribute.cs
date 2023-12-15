using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class TagNameAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(TagNameAttribute))]
public class TagNameAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            return 2 * base.GetPropertyHeight(property, label);
        }
        else
        {
            return base.GetPropertyHeight(property, label);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            var pos = new Rect(position.x, position.y, position.width, position.height * 0.5f);
            EditorGUI.HelpBox(pos, "This attribute only support \"String\" type!", MessageType.Error);
            pos.y += pos.height;
            EditorGUI.PropertyField(pos, property, label, true);
        }
        else
        {
            var tagNames = UnityEditorInternal.InternalEditorUtility.tags;

            var value = property.stringValue;
            int selectedIndex = Array.FindIndex(tagNames, tag => tag == value);
            if (selectedIndex == -1)
            {
                selectedIndex = 0;
            }

            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, UnityEditorInternal.InternalEditorUtility.tags);

            property.stringValue = tagNames[selectedIndex];
        }
    }
}

#endif