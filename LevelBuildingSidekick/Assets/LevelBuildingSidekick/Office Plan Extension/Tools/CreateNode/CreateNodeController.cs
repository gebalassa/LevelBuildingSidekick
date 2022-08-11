using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick.Graph;

public class CreateNodeController : ToolController
{
    public CreateNodeController(Data data, ToolkitController toolkit) : base(data, toolkit)
    {
        View = new CreateNodeView(this);
    }

    public override void Switch()
    {
        base.Switch();
    }

    public override void Action(LevelRepresentationController level)
    {
        LBSGraphController graph = level as LBSGraphController;
        if (graph == null)
        {
            Debug.Log("NULL Graph");
            return;
        }

        LBSNodeData node = new LBSNodeData();

        var xPos = (int)(Event.current.mousePosition.x - node.radius); // node.radius/2f??
        var yPos = (int)(Event.current.mousePosition.y - node.radius); // node.radius/2f??
        node.x = xPos;
        node.y = yPos;
        //new Vector2Int(xPos, yPos);
        node.room = new LevelBuildingSidekick.Schema.RoomCharacteristics();

        int index = graph.Nodes.Count;
        node.label = "Node: " + index.ToString();
        while(!graph.AddNode(node))
        {
            index++;
            node.label = "Node: " + index.ToString();
        }
        //graph.AddNode(node);
    }


    public override void LoadData()
    {
    }

    public override void OnMouseDown(Vector2 position)
    {
    }

    public override void OnMouseUp(Vector2 position)
    {
        Action(Toolkit.Level);
    }

    public override void OnMouseDrag(Vector2 position)
    {
    }

    public override void Update()
    {
    }
}
