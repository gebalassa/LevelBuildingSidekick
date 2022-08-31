﻿using LevelBuildingSidekick;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSViewController : Controller
{
    public List<ContextAction> contextActions = new List<ContextAction>();

    protected LBSViewController(Data data) : base(data) { }

    public abstract void PopulateView(GraphView view);

    public override void LoadData()
    {
        //throw new System.NotImplementedException();
    }

    public override void Update()
    {
        //throw new System.NotImplementedException();
    }
}

public struct ContextAction
{
    public Action<DropdownMenuAction,LBSBaseView, ContextualMenuPopulateEvent> action;
    public string name;

    public ContextAction(string name, Action<DropdownMenuAction, LBSBaseView, ContextualMenuPopulateEvent> action)
    {
        this.action = action;
        this.name = name;
    }
}