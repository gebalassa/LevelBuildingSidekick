using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSWindowEditor : EditorWindow
{
    protected VisualElement root;

    public abstract void OnCreateGUI();

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        OnCreateGUI();

        var saveBtn = root.Q<ToolbarButton>("SaveButton");
        saveBtn.clicked += SaveAction;

        var saveAsBtn = root.Q<ToolbarButton>("SaveAsButton");
        saveAsBtn.clicked += SaveAsAction;
    }

    protected void ImportUXML(string name)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>(name);
        visualTree.CloneTree(root);
    }

    protected void ImportStyleSheet(string name)
    {
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>(name);
        root.styleSheets.Add(styleSheet);
    }

    private void SaveAction()
    {
        LBSController.SaveFile();
    }

    private void SaveAsAction()
    {
        LBSController.SaveFileAs();
    }


}
