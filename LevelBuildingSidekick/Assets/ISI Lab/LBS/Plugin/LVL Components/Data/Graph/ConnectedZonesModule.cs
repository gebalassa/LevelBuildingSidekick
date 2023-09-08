using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConnectedZonesModule : LBSModule
{
    #region FIELDS
    [SerializeField, JsonRequired, SerializeReference]
    List<ZoneEdge> edges = new List<ZoneEdge>();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<ZoneEdge> Edges => new List<ZoneEdge>(edges);
    #endregion

    #region CONSTRUCTORS
    public ConnectedZonesModule() { }
    public ConnectedZonesModule(IEnumerable<ZoneEdge> edges) 
    { 
        foreach(var e in edges)
        {
            AddEdge(e);
        }
    }
    #endregion

    #region METHODS
    public void AddEdge(ZoneEdge edge)
    {
        if (!edges.Contains(edge))
            edges.Add(edge);
    }

    public void AddEdge(Zone first, Zone second)
    {
        //check if already exist (?)
        edges.Add(new ZoneEdge(first, second));
    }

    public ZoneEdge GetEdge(Zone first, Zone second)
    {
        var edge = edges.Find(e => e.First.Equals(first) && e.Second.Equals(second));
        if (edge != null)
            return edge;
        edge = edges.Find(e => e.First.Equals(second) && e.Second.Equals(first));
        return edge;
    }

    public void RemoveEdge(Zone first, Zone second)
    {
        var edge = GetEdge(first, second);
        if (edge != null)
            edges.Remove(edge);
    }

    public void RemoveEdges(Zone zone)
    {
        var toRemove = edges.Where(e => e.First.Equals(zone) || e.Second.Equals(zone));
        foreach(var e in toRemove)
        {
            edges.Remove(e);
        }
    }

    public override void Clear()
    {
        edges.Clear();
    }

    public override object Clone()
    {
        return new ConnectedZonesModule(edges.Select(e => e.Clone()).Cast<ZoneEdge>());
    }

    public override Rect GetBounds()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsEmpty()
    {
        return edges.Count <= 0;
    }

    public override void OnAttach(LBSLayer layer)
    {

    }

    public override void OnDetach(LBSLayer layer)
    {
    }

    public override void Reload(LBSLayer layer)
    {
        //throw new System.NotImplementedException();
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public override void Rewrite(LBSModule module)
    {
        throw new System.NotImplementedException();
    }

    internal ZoneEdge GetEdge(Vector2 position, float delta)
    {
        foreach (var e in edges)
        {
            var dist = position.DistanceToLine(e.First.Pivot, e.Second.Pivot);
            if (dist < delta)
                return e;
        }
        return null;
    }

    internal void RemoveEdge(ZoneEdge edge)
    {
        edges.Remove(edge);
    }
    #endregion
}

[System.Serializable]
public class ZoneEdge : ICloneable
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private Zone first;

    [SerializeField, SerializeReference, JsonRequired]
    private Zone second;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public Zone First
    {
        get => first;
        set => first = value;
    }

    [JsonIgnore]
    public Zone Second
    {
        get => second;
        set => second = value;
    }
    #endregion

    #region CONSTRUCTORS
    public ZoneEdge(Zone first, Zone second)
    {
        this.first = first;
        this.second = second;
    }
    #endregion

    #region METHODS
    public object Clone()
    {
        return new ZoneEdge(first.Clone() as Zone, second.Clone() as Zone);
    }
    #endregion
}