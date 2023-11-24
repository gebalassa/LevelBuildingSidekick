using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestPanelEditor : LBSCustomEditor
{
    TextField questTitle;

    public QuestPanelEditor(object target) : base(target)
    {
        CreateVisualElement();
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        LBSQuestGraph quest = target as LBSQuestGraph;

        questTitle.SetValueWithoutNotify(quest.Name);
    }

    protected override VisualElement CreateVisualElement()
    {
        LBSQuestGraph quest = target as LBSQuestGraph;

        questTitle = new TextField("Quest: ");

        questTitle.value = quest.Name;
        questTitle.RegisterValueChangedCallback(evt =>
        {
            quest.Name = evt.newValue;
        });

        return this;
    }
}
