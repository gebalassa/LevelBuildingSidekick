using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Drawer
{
    public string modeID;

    public Drawer() { }

    public abstract void Draw(ref LBSLayer layer, MainView view);
}