using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalAssistants : LBSInspector
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalAssistants, VisualElement.UxmlTraits> { }
    #endregion

    private readonly Color colorAS = LBSSettings.Instance.view.assitantsColor;// new Color(135f / 255f, 215f / 255f, 246f / 255f);

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentAssist;

    private LBSLayer target;

    private ToolKit toolkit;

    public LBSLocalAssistants()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalAssistants");
        visualTree.CloneTree(this);

        this.content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
        this.contentAssist = this.Q<VisualElement>("ContentAssist");

        toolkit = ToolKit.Instance;
    }

    public void SetInfo(LBSLayer target)
    {
        contentAssist.Clear();

        this.target = target;

        if(target.Assitants.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
            return;
        }

        noContentPanel.SetDisplay(false);

        foreach (var assist in target.Assitants)
        {
            var type = assist.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                .Where(t => t.Item2.Any(v => v.type == type));

            if (ves.Count() == 0)
            {
                Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                continue;
            }

            var ovg = ves.First().Item1;
            var ve = Activator.CreateInstance(ovg, new object[] { assist });
            if (!(ve is VisualElement))
            {
                Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                continue;
            }

            if (ve is IToolProvider)
            {
                ((IToolProvider)ve).SetTools(toolkit);
            }


            var content = new BehaviourContent(ve as LBSCustomEditor, assist.Name, assist.Icon, colorAS);
            contentAssist.Add(content);

        }
    }

    public override void Init( MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        throw new NotImplementedException();
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
        ToolKit.Instance.SetActive(0);
    }
}
