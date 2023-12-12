using ISILab.AI.Optimization.Populations;
using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateQuestNode : LBSManipulator // where T: LBSNode  // (!) CreateNewNode<T>
{
    //QuestBehaviour quest;
    public GrammarTerminal actionToSet;
    LBSQuestGraph quest;

    public LBSQuestGraph Quest
    {
        get => quest; 
        set => quest = value;
    }


    private string prefix = "";
    public CreateQuestNode() : base()
    {
    }

    public override void Init(LBSLayer layer, object owner)
    {
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
    {
        //throw new NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
    {
        if(actionToSet == null)
        {
            Debug.LogWarning("No tienen nada seleccionado, asegurate de seleccionar" +
                "una gramatica y una palabra para que funcione.");
            return;
        }

        var name = "";
        var loop = true;
        var v = 0;
        do
        {
            name = prefix + actionToSet.ID + " (" + v + ")";

            loop = quest.QuestNodes.Any(n => n.ID.Equals(name));
            v++;
        } while (loop);

        quest.AddNode(new QuestNode(name, EndPosition, actionToSet.ID));
        
    }
}
