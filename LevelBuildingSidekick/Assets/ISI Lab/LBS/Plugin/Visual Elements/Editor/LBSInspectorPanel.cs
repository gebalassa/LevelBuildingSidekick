using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSInspectorPanel : VisualElement 
{
    public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }

    private MainView view;
    private VisualElement content;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    private ButtonGroup subTabs;
    private ButtonGroup mainTab;

    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // SubTabs
        subTabs = this.Q<ButtonGroup>("SubTabs");
        subTabs.Init();

        // MainButtonGroup
        mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();
        SetSubTab();

        // Content
        content = this.Q<VisualElement>("InspectorContent");
    }

    public LBSInspectorPanel(ref MainView mainView, VisualElement content)
    {
        this.view = mainView;
        this.content = content;
    }

    private void SetSubTab()
    {
        var tabs = mainTab.Children().Select(st => st as IGrupable).ToList();

        //tabs[0].AddClickEvent =


        subTabs.Choices = "Tags,Bundles,Assistants";
    }

    public void AddInspector(LBSInspector inspector)
    {
        inspectors.Add(inspector);
        content.Add(inspector);
    }

    public void RemoveInspector(LBSInspector inspector)
    {
        inspectors.Remove(inspector);
        if(content.Contains(inspector))
            content.Remove(inspector);
    }
}
