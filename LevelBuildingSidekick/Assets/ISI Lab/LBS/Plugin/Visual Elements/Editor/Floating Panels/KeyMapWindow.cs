using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyMapWindow : EditorWindow
{
    [MenuItem("Window/ISILab/Hints controller", priority = 100)]
    public static void ShowWindow()
    {
        var window = GetWindow<KeyMapWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo");
        window.titleContent = new GUIContent("Key map", icon);
        window.minSize = new Vector2(400, 600);
    }

    public virtual void CreateGUI()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("HintsController");
        visualTree.CloneTree(rootVisualElement);

    }
}
