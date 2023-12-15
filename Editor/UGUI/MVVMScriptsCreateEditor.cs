using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

/// <summary>
/// Tool for creating MVVM Scripts
/// </summary>
public class MVVMScriptsCreateEditor : EditorWindow
{
    [MenuItem("Assets/Create/MVVM Scripts", false, 0)]
    public static void ShowExample()
    {
        MVVMScriptsCreateEditor wnd = GetWindow<MVVMScriptsCreateEditor>();
        wnd.titleContent = new GUIContent("MVVM Create Toolkit");
        wnd.maxSize = new Vector2(640, 256);
        wnd.minSize = new Vector2(640, 256);
    }

    private VisualElement m_RightPane;
    private readonly string m_UIFolder = "Assets/Game/Scripts/UI/";
    private readonly string m_ModelFolder = "Assets/Game/Scripts/Model/";
    private readonly string m_SystemFolder = "Assets/Game/Scripts/Systems/";
    private readonly string m_TemplateFolder = "Packages/com.openngs.ni.ui/Editor/MVVMScriptTemplate/";

    private VisualElement m_CreateViewModelPanel, m_CreateModelPanel, m_CreateSystemPanel;
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        m_RightPane = new VisualElement();
        
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        
        root.Add(splitView);
        
        var leftPane = new ListView();
        
        var allObjects = new List<string>
        {
            "Create View/ViewModel Scripts",
            "Create Model Script",
            // "Create System Script"
        };
        
        leftPane.makeItem = () => new TextElement();
        leftPane.bindItem = (item, index) =>
        {
            var btn = item as TextElement;
            btn.AddToClassList("unity-button");
            btn.text = allObjects[index];
        };
        leftPane.itemsSource = allObjects;

        m_CreateViewModelPanel = AddCreateViewModelPanel();
        m_CreateModelPanel = AddCreateModelPanel();
        m_CreateSystemPanel = AddCreateSystemPanel();
        
        leftPane.onSelectionChange += (objects) =>
        {
            m_CreateViewModelPanel.visible = leftPane.selectedIndex == 0;
            m_CreateModelPanel.visible = leftPane.selectedIndex == 1;
            m_CreateSystemPanel.visible = leftPane.selectedIndex == 2;

            ResetPanelOrder(m_CreateModelPanel);
            ResetPanelOrder(m_CreateSystemPanel);
            ResetPanelOrder(m_CreateViewModelPanel);
        };
        
        leftPane.selectedIndex = 0;
        
        splitView.Add(leftPane);
        splitView.Add(m_RightPane);
    }

    private void ResetPanelOrder(VisualElement panel)
    {
        if (panel.visible)
            panel.SendToBack();
        else
            panel.BringToFront();
    }

    private VisualElement AddCreateViewModelPanel()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.openngs.ni.ui/Editor/MVVMScriptsCreateUI0.uxml");
        var elements = visualTree.Instantiate();

        m_RightPane.Add(elements);
        
        // bind ui control
        var classInput = elements.Q<TextField>("classInput");
        var namespaceInput = elements.Q<TextField>("namespaceInput");
        var createBtn = elements.Q<Button>("Create");
        
        var label0 = elements.Q<Label>("desLabel0");
        var label1 = elements.Q<Label>("desLabel1");
        var label2 = elements.Q<Label>("desLabel2");
        var errorLabel = elements.Q<Label>("errorLabel");

        classInput.RegisterValueChangedCallback((str) =>
        {
            label0.text = $"view class: {classInput.text}View";
            label1.text = $"viewModel class: {classInput.text}ViewModel";
            errorLabel.text = "";
        });

        namespaceInput.RegisterValueChangedCallback((str) =>
        {
            label2.text = $"namespace: UI.{namespaceInput.text}";
            errorLabel.text = "";
        });
        
        // bind click event
        createBtn.clicked += () =>
        {
            var className = classInput.text;
            if (string.IsNullOrEmpty(className))
            {
                errorLabel.text = "Input Class Name Prefix";
                return;
            }

            var viewClass = $"{className}View";
            var viewModelClass = $"{className}ViewModel";

            var parentFolder = m_UIFolder;
            var namespaceName = "UI";
            if (!string.IsNullOrEmpty(namespaceInput.text))
            {
                namespaceName = $"UI.{namespaceInput.text}";
                parentFolder += $"{namespaceInput.text}/";
            }

            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder);
            
            var viewPath = $"{parentFolder}{viewClass}.cs";
            if (!File.Exists(viewPath))
            {
                var str0 = File.ReadAllText($"{m_TemplateFolder}TemplateView.md");
                str0 = str0.Replace("TemplateView", viewClass);
                str0 = str0.Replace("TemplateViewModel", viewModelClass);
                str0 = str0.Replace("namespace UI", $"namespace {namespaceName}");
                File.WriteAllText(viewPath, str0);
            }

            var viewModelPath = $"{parentFolder}{viewModelClass}.cs";
            if (!File.Exists(viewModelPath))
            {
                var str1 = File.ReadAllText($"{m_TemplateFolder}TemplateViewModel.md");
                str1 = str1.Replace("TemplateViewModel", viewModelClass);
                str1 = str1.Replace("namespace UI", $"namespace {namespaceName}");
                File.WriteAllText(viewModelPath, str1);
            }
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(viewModelPath);
        };
        return elements;
    }

    private VisualElement AddCreateModelPanel()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.openngs.ni.ui/Editor/MVVMScriptsCreateUI1.uxml");
        var elements = visualTree.Instantiate();
        m_RightPane.Add(elements);
        
        // bind ui control
        var classInput = elements.Q<TextField>("classInput");
        var namespaceInput = elements.Q<TextField>("namespaceInput");
        var notifyTypeInput = elements.Q<TextField>("notifyTypeInput");
        var createBtn = elements.Q<Button>("Create");
        
        var label0 = elements.Q<Label>("desLabel0");
        var label1 = elements.Q<Label>("desLabel1");
        var label2 = elements.Q<Label>("desLabel2");
        var errorLabel = elements.Q<Label>("errorLabel");
        
        classInput.RegisterValueChangedCallback((str) => label0.text = $"class name: {classInput.text}");

        notifyTypeInput.RegisterValueChangedCallback((str) => label1.text = $"notify enum name: {notifyTypeInput.text}");
        
        namespaceInput.RegisterValueChangedCallback((str) => label2.text = $"namespace: Model.{namespaceInput.text}");
        
        // bind click event
        createBtn.clicked += () =>
        {
            var className = classInput.text;
            if (string.IsNullOrEmpty(className))
            {
                errorLabel.text = "Input Class Name";
                return;
            }
            
            var enumName = notifyTypeInput.text;
            if (string.IsNullOrEmpty(enumName))
            {
                errorLabel.text = "Input Enum Name";
                return;
            }

            var parentFolder = m_ModelFolder;
            
            var namespaceName = "Model";
            if (!string.IsNullOrEmpty(namespaceInput.text))
            {
                namespaceName = $"Model.{namespaceInput.text}";
                parentFolder += $"{namespaceInput.text}/";
            }
            
            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder);
            
            var str0 = File.ReadAllText($"{m_TemplateFolder}TemplateModel.md");
            str0 = str0.Replace("TemplateModel", className);
            str0 = str0.Replace("TemplateNotify", enumName);
            str0 = str0.Replace("namespace Model", $"namespace {namespaceName}");
            File.WriteAllText($"{parentFolder}{className}.cs", str0);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{parentFolder}{className}.cs");
        };
        return elements;
    }
    
    private VisualElement AddCreateSystemPanel()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.openngs.ni.ui/Editor/MVVMScriptsCreateUI2.uxml");
        var elements = visualTree.Instantiate();
        m_RightPane.Add(elements);
        
        // bind ui control
        var classInput = elements.Q<TextField>("classInput");
        var notifyTypeInput = elements.Q<TextField>("notifyTypeInput");
        var createBtn = elements.Q<Button>("Create");
        
        var label0 = elements.Q<Label>("desLabel0");
        var label1 = elements.Q<Label>("desLabel1");
        var label2 = elements.Q<Label>("desLabel2");
        var errorLabel = elements.Q<Label>("errorLabel");
        var context = elements.Q<DropdownField>("Context");
        context.choices = new List<string>
        {
            "Login",
            "World",
            "Bootstrap"
        };
        context.value = "World";
        
        classInput.RegisterValueChangedCallback((str) =>
        {
            label0.text = $"class name: {classInput.text}System";
            label2.text = $"namespace: Systems.{classInput.text}";
        });

        notifyTypeInput.RegisterValueChangedCallback((str) => label1.text = $"notify enum name: {notifyTypeInput.text}");
        
        
        // bind click event
        createBtn.clicked += () =>
        {
            var classPrefix = classInput.text;
            var className = $"{classPrefix}System";
            if (string.IsNullOrEmpty(classPrefix))
            {
                errorLabel.text = "Input Class Name";
                return;
            }
            
            var enumName = notifyTypeInput.text;
            if (string.IsNullOrEmpty(enumName))
            {
                errorLabel.text = "Input Enum Name";
                return;
            }

            var parentFolder = $"{m_SystemFolder}{classPrefix}/";
            var namespaceName = $"Systems.{classPrefix}";
       
            
            if (!Directory.Exists(parentFolder))
                Directory.CreateDirectory(parentFolder);

            var subsystem = "GameSubSystem";
            if (context.value == "Login")
                subsystem = "LoginSubSystem";
            else if (context.value == "Bootstrap")
                subsystem = "BootstrapSubSystem";
            
            var str0 = File.ReadAllText($"{m_TemplateFolder}TemplateSystem.md");
            str0 = str0.Replace("TemplateSystem", className);
            str0 = str0.Replace("TemplateNotify", enumName);
            str0 = str0.Replace("namespace Systems", $"namespace {namespaceName}");
            str0 = str0.Replace("GameSubSystem", subsystem);
            File.WriteAllText($"{parentFolder}{className}.cs", str0);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{parentFolder}{className}.cs");
        };
        return elements;
    }
}