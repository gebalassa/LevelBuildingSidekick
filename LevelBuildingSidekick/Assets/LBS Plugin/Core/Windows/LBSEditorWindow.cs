using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSEditorWindow : EditorWindow
{
    protected VisualElement root;

    public abstract void OnCreateGUI();

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        OnCreateGUI();

        var toolBar = new Toolbar();
        var fileMenu = new ToolbarMenu();
        fileMenu.text = "File";
        fileMenu.menu.AppendAction("Level.../Load",(dma)=> { LBSController.LoadFile(); });
        fileMenu.menu.AppendAction("Level.../Save", (dma) => { LBSController.SaveFile(); });
        fileMenu.menu.AppendAction("Level.../Save as", (dma) => { LBSController.SaveFileAs(); });
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Representation.../Load", (dma) => { Debug.Log("[Implementar loadRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Representation.../Save", (dma) => { Debug.Log("[Implementar saveRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Representation.../Save as", (dma) => { Debug.Log("[Implementar saveRep]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Help.../Documentation", (dma) => { Debug.Log("[Implementar documnetation]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendAction("Help.../About", (dma) => { Debug.Log("[Implementar about]"); }); // ver si es necesaria (!)
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Close", (dma) => { this.Close(); }); 
        fileMenu.menu.AppendAction("Close All", (dma) => {
            LBSController.GetSubClassTypes<LBSEditorWindow>().ForEach(t => EditorWindow.GetWindow(t).Close());
        });

        var search = new ToolbarPopupSearchField();
        search.tooltip = "[Implementar]";

        root.Insert(0,toolBar);
        toolBar.Add(fileMenu);
        toolBar.Add(search); 
    }

    private void OnDestroy()
    {
        var answer = EditorUtility.DisplayDialog(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "save",
                   "discard");

        if (answer)
            LBSController.SaveFile();
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

}
