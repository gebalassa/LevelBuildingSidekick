using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Graph;

public class EdgeController : Controller
{
    public NodeController Node1 { get; set; }
    public NodeController Node2 { get; set; }
    public float Thickness
    {
        get
        {
            return (Data as EdgeData).thikness;
        }
    }

    public EdgeController(Data data) : base(data)
    {
        View = new EdgeView(this);
    }

    public override void LoadData()
    {
    }

    public override void Update()
    {
    }

    internal bool DoesConnect(NodeController n1, NodeController n2)
    {
        EdgeData d = Data as EdgeData;
        return ((n1.Data.Equals(d.firstNode) && n2.Data.Equals(d.secondNode)) || (n2.Data.Equals(d.firstNode) && n1.Data.Equals(d.secondNode)));
    }

    internal bool Contains(NodeController node)
    {
        EdgeData d = Data as EdgeData;
        return (node.Data.Equals(d.firstNode) || node.Data.Equals(d.secondNode));
    }
}
