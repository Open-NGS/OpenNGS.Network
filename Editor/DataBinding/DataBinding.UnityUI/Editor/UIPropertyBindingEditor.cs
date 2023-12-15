using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OpenNGS.UI.DataBinding.UnityUI;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OpenNGS.UI.DataBinding.UnityUI
{
    [CustomEditor(typeof(UIPropertyBinding), true)]
    public class UIPropertyBindingEditor : Editor
    {
        private UIPropertyBinding m_Binding;
        
        private SerializedProperty m_TargetTypeProperty;
        private SerializedProperty m_Target;
        private SerializedProperty m_Source;
        
        private SerializedProperty m_BindingPropertyProperty;
        private SerializedProperty m_BindingPathProperty;
        
        private string[] m_TargetOptions;

        private readonly List<Object> m_TargetList = new();
        private int m_TargetIndex;
        private int m_TargetPropertyIndex;
        private int m_BindingPathIndex;

        private string[] m_TargetPropertyOptions;
        private string[] m_BindingPathOptions;
        private readonly List<Type> m_BindingPathTypeOptions = new ();

        private Type m_BindingDataType;
        private Type m_SourceType;
        private void OnEnable()
        {
            m_Binding = target as UIPropertyBinding;
            
            m_TargetTypeProperty = serializedObject.FindProperty("TargetType");
            m_Target = serializedObject.FindProperty("Target");
            m_Source = serializedObject.FindProperty("Source");
            m_BindingPropertyProperty = serializedObject.FindProperty("BindingProperty");
            m_BindingPathProperty = serializedObject.FindProperty("BindingPath");
            
            AutoPickDataSource();
            InitBindingPath();
            InitTargetOptions();
            InitTargetPropertyOptions();
        }

        private void AutoPickDataSource()
        {
            if (m_Source.objectReferenceValue != null)
                return;
            
            var t = m_Binding.transform;
            while (t.parent != null)
            {
                var array = t.GetComponents<MonoBehaviour>();
                foreach (var mono in array)
                {
                    var attribute = Attribute.GetCustomAttribute(mono.GetType(), typeof(DataSourceAttribute));
                    if (attribute != null)
                    {
                        m_Source.objectReferenceValue = mono.gameObject;
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                }
                t = t.parent;
            }
        }
        
        private void InitBindingPath()
        {
            m_BindingPathTypeOptions.Clear();
            
            if (m_Source.objectReferenceValue == null)
                return;

            var list = new List<string>();
            m_SourceType = null;

            foreach (var mono in m_Binding.Source.GetComponents<MonoBehaviour>())
            {
                var attribute = Attribute.GetCustomAttribute(mono.GetType(), typeof(DataSourceAttribute));
                if (attribute != null)
                {
                    m_SourceType = ((DataSourceAttribute) attribute).GeViewModelType();
                    break;
                }
            }
            
            if (m_SourceType == null) 
                return;

            var index = 0;
            var paths = GetObjectTypePropertyBindingPaths(m_SourceType);
            for (index = 0; index < paths.Count; ++index)
            {
                list.Add(paths[index].Item1);
                m_BindingPathTypeOptions.Add(paths[index].Item2);
                if (paths[index].Item1.Equals(m_BindingPathProperty.stringValue))
                {
                    m_BindingPathIndex = index;
                    m_BindingDataType = paths[index].Item2;
                }
            }

            if (string.IsNullOrEmpty(m_BindingPathProperty.stringValue) && list.Count > 0)
            {
                m_BindingPathProperty.stringValue = list[0];
                m_BindingDataType = m_BindingPathTypeOptions[0];
            }
            
            m_BindingPathOptions = list.ToArray();
        }

        private static List<(string, Type)> GetObjectTypePropertyBindingPaths(Type type)
        {
            var set = new HashSet<Type>();
            return GetObjectTypePropertyBindingPaths("", type, set);
        }
        
        // use the type chain collection, fix the infinite recursion stackoverflow
        private static List<(string, Type)> GetObjectTypePropertyBindingPaths(string path, Type type, HashSet<Type> typeChain)
        {
            var isMono = type.IsSubclassOf(typeof(MonoBehaviour));
            var propertyInfos= type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            var ret = new List<(string, Type)>();
            
            foreach (var vmP in propertyInfos)
            {
                // fix the stackoverflow issue
                if (typeChain.Contains(vmP.PropertyType))
                    continue;
                
                // exclude mono behaviour properties
                var declaringType = vmP.DeclaringType;
                if (isMono && (declaringType == typeof(MonoBehaviour) || 
                               declaringType == typeof(Behaviour) || 
                               declaringType == typeof(Component) || 
                               declaringType == typeof(Object)))
                    continue;
                
                if (!vmP.PropertyType.IsValueType && vmP.PropertyType != typeof(string) && !BindingUtil.IsGenericEnumerableCollection(vmP.PropertyType))
                {
                    // custom reference type, recursive query
                    var chain = new HashSet<Type>(typeChain) {vmP.PropertyType};
                    var list = GetObjectTypePropertyBindingPaths($"{path}{vmP.Name}/", vmP.PropertyType, chain);
                    ret.AddRange(list);
                }
                else
                {
                    if (vmP.CanRead)
                        ret.Add(($"{path}{vmP.Name}", vmP.PropertyType));
                }
            }
            return ret;
        }
        
        /// <summary>
        /// check if is IEquatable type
        /// </summary>
        private static bool IsTypeIEquatable(Type type)
        {
            var interfaceType = typeof(IEquatable<>).MakeGenericType(type);
            return interfaceType.IsAssignableFrom(type);
        }

        private void InitTargetOptions()
        {
            var components = m_Binding.GetComponents<MonoBehaviour>();
            var list = new List<string>();
            m_TargetList.Clear();

            foreach (var graphic in components)
            {
                // exclude UIPropertyBinding type
                if (graphic is UIPropertyBinding)
                    continue;
                
                list.Add(graphic.GetType().ToString());
                m_TargetList.Add(graphic);
            }
            
            var rect = m_Binding.GetComponent<RectTransform>();
            if (rect)
            {
                list.Add(typeof(RectTransform).ToString());
                m_TargetList.Add(rect);
            }
            
            list.Add(typeof(GameObject).ToString());
            m_TargetList.Add(m_Binding.gameObject);

            if (list.Count == 0) return;
            
            if (string.IsNullOrEmpty(m_TargetTypeProperty.stringValue))
            {
                m_TargetTypeProperty.stringValue = list[0];
                m_Target.objectReferenceValue = m_TargetList[0];
                m_TargetIndex = 0;
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                m_TargetIndex = list.IndexOf(m_TargetTypeProperty.stringValue);
            }

            m_TargetOptions = list.ToArray();
        }

        private void InitTargetPropertyOptions()
        {
            if (m_Target.objectReferenceValue == null)
                return;
            var t = m_Target.objectReferenceValue.GetType();
            
            var list = new List<string>();
            var index = 0;
            m_TargetPropertyIndex = 0;
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || !p.CanWrite)
                    continue;

                var isIRecyclableScrollRectDataSource =
                    m_BindingDataType != null && BindingUtil.IsGenericEnumerableCollection(m_BindingDataType) &&
                    p.PropertyType == typeof(IRecyclableScrollRectDataSource);


                if ((m_BindingDataType == null || m_BindingDataType != p.PropertyType) &&
                    !isIRecyclableScrollRectDataSource) 
                    continue;
                
                list.Add(p.Name);
                if (p.Name.Equals(m_BindingPropertyProperty.stringValue))
                {
                    m_TargetPropertyIndex = index;
                }

                ++index;
            }

            if (list.Count > 0 && m_TargetPropertyIndex == 0)
            {
                m_BindingPropertyProperty.stringValue = list[0];
            }

            m_TargetPropertyOptions = list.ToArray();
        }

        public override void OnInspectorGUI()
        {
            if (m_TargetOptions.Length == 0)
                return;
            
            EditorGUILayout.Space();
            
            // 1. binding source
            EditorGUI.BeginChangeCheck();
            var bindingSourceChange = false;
            EditorGUILayout.PropertyField(m_Source);
            bindingSourceChange = EditorGUI.EndChangeCheck();
            if (bindingSourceChange)
            {
                serializedObject.ApplyModifiedProperties();
                InitBindingPath();
            }
            
            
            // 2. binding path
            EditorGUI.BeginChangeCheck();
            var bindingPathChange = false;
            if (m_BindingPathOptions is {Length: > 0})
            {
                m_BindingPathIndex = EditorGUILayout.Popup("Binding Path", m_BindingPathIndex, m_BindingPathOptions);
                m_BindingPathProperty.stringValue = m_BindingPathOptions[m_BindingPathIndex];
                m_BindingDataType = m_BindingPathTypeOptions[m_BindingPathIndex];
                bindingPathChange = EditorGUI.EndChangeCheck();
            }
            
            EditorGUILayout.Space();
            
            // 3. binding target
            EditorGUI.BeginChangeCheck();
            var bindingTargetChange = false;
            m_TargetIndex = EditorGUILayout.Popup("Target", m_TargetIndex, m_TargetOptions);
            m_TargetTypeProperty.stringValue = m_TargetOptions[m_TargetIndex];
            m_Target.objectReferenceValue = m_TargetList[m_TargetIndex];

            bindingTargetChange = EditorGUI.EndChangeCheck();
            
            if (bindingSourceChange || bindingPathChange || bindingTargetChange)
            {
                InitTargetPropertyOptions();
            }
            
            // 4. binding property
            EditorGUI.BeginChangeCheck();
            var bindingPropertyChange = false;
            if (m_TargetPropertyOptions is {Length: > 0})
            {
                m_TargetPropertyIndex =
                    EditorGUILayout.Popup("Target Property", m_TargetPropertyIndex, m_TargetPropertyOptions);
                m_BindingPropertyProperty.stringValue = m_TargetPropertyOptions[m_TargetPropertyIndex];
                bindingPropertyChange = EditorGUI.EndChangeCheck();
            }
            
            serializedObject.ApplyModifiedProperties();
            
            if (bindingSourceChange || bindingPathChange || bindingTargetChange || bindingPropertyChange)
            {
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_Binding);
            }
        }
    }
}
