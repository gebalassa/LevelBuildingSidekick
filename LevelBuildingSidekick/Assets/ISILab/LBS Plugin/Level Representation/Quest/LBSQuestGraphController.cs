using LBS.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSQuestGraphController : LBSGraphController
{
    public GrammarTree GrammarTree;

    List<QuestGraphNode> quests;

    List<QuestGraphNode> openNodes; // change name to something bette (!!!)

    QuestGraphNode currentQuest;
    public QuestGraphNode CurrentQuest
    {
        get
        {
            if(currentQuest == null)
            {
                if(quests == null)
                {
                    quests = new List<QuestGraphNode>();
                }
                if(quests.Count == 0)
                {
                    quests.Add(new QuestGraphNode("Quest1", Vector2.zero));
                }
                CurrentQuest = quests[0];
            }
            return currentQuest;
        }
        set
        {
            if(currentQuest != value)
            {
                currentQuest = value;
                openNodes.Clear();
                OpenNode(currentQuest);
            }
        }
    }

    public LBSQuestGraphController(LBSGraphView view, LBSGraphData data) : base(view, data)
    {
        GrammarTree = GrammarReader.ReadGrammar(Application.dataPath + "/Grammar/Grammar.xml");
        openNodes = new List<QuestGraphNode>();
        OpenNode(CurrentQuest);
    }

    internal override LBSNodeData NewNode(Vector2 position)
    {
        QuestGraphNode g = new QuestGraphNode("Undefined", position);
        return g;
    }

    public LBSNodeData NewNode(Vector2 position, GrammarNode grammarElement)
    {
        QuestGraphNode g = new QuestGraphNode(grammarElement.ID, position);
        return g;
    }

    internal override void AddNode(LBSNodeData node)
    {
        base.AddNode(node);
        openNodes[^1].Children.Add(node as QuestGraphNode);
    }

    internal GrammarNode GetGrammarElement(string grammarKey)
    {
        return GrammarTree.GetGrammarElement(grammarKey);
    }

    public void OpenNode(QuestGraphNode node)
    {
        if(openNodes.Count == 0)
        {
            openNodes.Add(node);
        }
        else if(openNodes[^1].Children.Contains(node))
        {
            CloseNode(openNodes[^1]);
            openNodes.Add(node);
            Debug.Log(node.GrammarKey);
        }

        var graph = data as LBSGraphData;

        Debug.Log(graph.GetNodes().Count);

        graph.Clear();

        Debug.Log(graph.GetNodes().Count);

        if (node.Children.Count != 0)
        {
            Debug.Log("Working");
            data.AddNode(node.Children[0]);

            for (int i = 1; i < node.Children.Count; i++)
            {
                graph.AddNode(node.Children[i]);
                graph.AddEdge(node.Children[i - 1], node.Children[i]);
            }
        }

        Debug.Log(graph.GetNodes().Count);

    }

    public void CloseNode(QuestGraphNode node)
    {
        node.Children.Clear();

        var nodes = data.GetNodes();

        foreach (var n in nodes)
        {
            if(n is QuestGraphNode)
                node.Children.Add(n as QuestGraphNode);
        }
    }
}
