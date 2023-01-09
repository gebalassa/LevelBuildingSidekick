using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalNode : GrammarNode
{
    string terminal;
    public string Text { get { return terminal; } set { terminal = value; } }


    public TerminalNode(string text)
    {
        ID = text;
        terminal = text;
    }

    public override string GetText()
    {
        return terminal;
    }

    public override List<GrammarNode> GetTerminals()
    {
        return new List<GrammarNode>() {this};
    }

    public override List<string> GetExpansionsText()
    {
        return new List<string>() { ID };
    }

    public override List<GrammarNode> GetExpansion(int index)
    {
        return new List<GrammarNode>() { this }; 
    }
}