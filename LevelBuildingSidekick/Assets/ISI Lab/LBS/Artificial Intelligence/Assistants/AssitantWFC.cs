using LBS.Assisstants;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class AssitantWFC : LBSAssistantAI
{
    public override object Clone()
    {
        return new AssitantWFC();
    }

    public override void Execute()
    {
        Debug.Log("No implementado WFC 'Execute()'");
    }

    public override void OnAdd(LBSLayer layer)
    {
        Debug.Log("No implementado WFC 'Init(layer)'");
    }
}