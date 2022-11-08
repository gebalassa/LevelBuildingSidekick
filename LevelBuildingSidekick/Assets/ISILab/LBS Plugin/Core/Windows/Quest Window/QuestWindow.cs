using Commons.Optimization.Evaluator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestWindow : EditorWindow, INameable
{
    public VisualElement actionsContent;

    [MenuItem("ISILab/LBS plugin/Quest window", priority = 1)]
    public static void ShowWindow()
    {
        var window = GetWindow<QuestWindow>();
        window.titleContent = new GUIContent(window.GetName());
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("QuestWindowUXML");
        visualTree.CloneTree(root);

        actionsContent = root.Q<VisualElement>("Actions");
    }

    public void AddAction()
    {
        var act = new ActionBtn();
        actionsContent.Add(act);
    }

    public string GetName()
    {
        return "Quest window";
    }
}
