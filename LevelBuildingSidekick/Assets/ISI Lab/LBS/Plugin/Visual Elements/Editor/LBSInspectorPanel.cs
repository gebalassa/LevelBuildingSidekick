using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSInspectorPanel : VisualElement 
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSInspectorPanel, VisualElement.UxmlTraits> { }
    #endregion

    #region FIELDS
    private MainView view;
    private VisualElement content;

    private List<LBSInspector> inspectors = new List<LBSInspector>();

    private ButtonGroup subTab;
    private ButtonGroup mainTab;

    private Dictionary<string, Dictionary<string, VisualElement>> VEs = new Dictionary<string, Dictionary<string, VisualElement>>();
    #endregion

    #region CONSTRUCTORS
    public LBSInspectorPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSInspectorPanel");
        visualTree.CloneTree(this);

        // SubTabs
        subTab = this.Q<ButtonGroup>("SubTabs");
        subTab.Init();

        // MainButtonGroup
        mainTab = this.Q<ButtonGroup>("MainTabs");
        mainTab.Init();

        // Content
        content = this.Q<VisualElement>("InspectorContent");

        InitTabs();
        SetTabs("Global");

        //SetSubTab();
    }
    #endregion

    #region METHODS
    private void InitTabs()
    {
        var gt = new LBSGlobalTagsInspector();
        AddTab("Global", "Tags", gt);

        var gb = new VisualElement();
        gb.style.backgroundColor = Color.blue;
        gb.style.flexGrow = 1;
        AddTab("Global", "Bundles", gb);


        var l1 = new VisualElement();
        l1.style.backgroundColor = Color.magenta;
        l1.style.flexGrow = 1;
        AddTab("Local", "Current data", l1);
        var l2 = new VisualElement();
        l2.style.backgroundColor = Color.gray;
        l2.style.flexGrow = 1;
        AddTab("Local", "Behaviours", l2);
        var l3 = new VisualElement();
        l3.style.backgroundColor = Color.yellow;
        l3.style.flexGrow = 1;
        AddTab("Local", "Assistants", l3);


        var aset = new VisualElement();
        aset.style.backgroundColor = Color.green;
        aset.style.flexGrow = 1;
        AddTab("Advanced", "Settings", aset);
        var aset2 = new VisualElement();
        aset2.style.backgroundColor = Color.cyan;
        aset2.style.flexGrow = 1;
        AddTab("Advanced", "Settings", aset2);
    }

    private void AddTab(string mainTabName, string subTabName, VisualElement element)
    {
        if(VEs.ContainsKey(mainTabName))
        {
            VEs.TryGetValue(mainTabName, out var dic);
            if (dic.ContainsKey(subTabName))
            {
                VEs.TryGetValue(mainTabName, out var visualElement);
            }
            else
            {
                dic.Add(subTabName, element);
            }
        }
        else
        {
            var newDic = new Dictionary<string, VisualElement>();
            newDic.Add(subTabName, element);
            VEs.Add(mainTabName, newDic);
        }

        var mainTabs = mainTab.Children().ToList();
        var subTabs = subTab.Children().ToList();

        mainTabs.Select(s => s.name).Any();

    }

    private void SetTabs(string name)
    {
        mainTab.Choices = string.Join(",", VEs.Select(pv => pv.Key));

        var mTabs = mainTab.Children().Select(st => st as IGrupable).ToList();
        foreach (var btn in mTabs)
        {
            btn.OnFocusEvent += () =>
            {
                var btn2 = btn as GrupalbeButton;
                SetSubTabs(btn2.text);

                this.ClearContent();
                VEs.TryGetValue(btn2.text, out var ve);
                var xx = ve.Keys.ToList();
                ve.TryGetValue(xx[0], out var inspct);
                this.SetContent(inspct);
            };
        }

        SetSubTabs(name);

        this.ClearContent();
        VEs.TryGetValue(name, out var ve);
        var xx = ve.Keys.ToList();
        ve.TryGetValue(xx[0], out var inspct);
        this.SetContent(inspct);
    }

    private void SetSubTabs(string name)
    {
        VEs.TryGetValue(name, out var ve);
        subTab.Choices = string.Join(",", ve.Select(pv => pv.Key));

        var sTabs = subTab.Children().Select(st => st as IGrupable).ToList();
        for (int i = 0; i < sTabs.Count(); i++)
        {
            var btn = sTabs[i];

            btn.OnFocusEvent += () =>
            {
                var btn2 = btn as GrupalbeButton;
                this.ClearContent();
                ve.TryGetValue(btn2.text,out var inspct);
                this.SetContent(inspct);
            };
        }
    }

    private void ClearContent()
    {
        content.Clear();
    }

    private void SetContent(VisualElement inspector)
    {
        if (inspector == null)
            return;

        //inspector.style.height = 30; // parche para ver lo que esta pasando (!!)
        content.Add(inspector);
    }

    public void AddInspector(LBSInspector inspector) // relacionado con toolkit (!!!)
    {
        inspectors.Add(inspector);
        content.Add(inspector);
    }

    public void RemoveInspector(LBSInspector inspector) // relacionado con toolkit (!!!)
    {
        inspectors.Remove(inspector);
        if(content.Contains(inspector))
            content.Remove(inspector);
    }
    #endregion
}
