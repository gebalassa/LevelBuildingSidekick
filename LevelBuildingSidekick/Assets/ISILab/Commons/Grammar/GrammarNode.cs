using LBS.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrammarNode
{
    public GrammarNode(){ }

    public string ID { get; set; }
    public abstract string GetText();
    public abstract List<GrammarNode> GetTerminals();


}
