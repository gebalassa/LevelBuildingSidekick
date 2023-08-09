using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class SimpleConectedBehaviour : LBSBehaviour
{
    public SimpleConectedBehaviour(Texture2D icon, string name) : base(icon, name) { }

    public override object Clone()
    {
        return new SimpleConectedBehaviour(this.Icon, this.Name);
    }

    public override void Init(LBSLayer layer)
    {
        // throw new NotImplementedException();
    }
}
