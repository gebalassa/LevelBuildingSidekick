using LBS.Bundles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Zone : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected string id = "Zone";
    [SerializeField, JsonRequired, JsonConverter(typeof(ColorConverter))]
    protected Color color;
    [SerializeField, JsonRequired, JsonConverter(typeof(Vector2))]
    protected Vector2 pivot;

    [ScriptableObjectReference(typeof(LBSIdentifier), "Interior Styles")]
    [SerializeField, JsonRequired]
    private List<string> tagsBundles = new List<string>();
    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public string ID
    {
        get => id;
        set => id = value;
    }

    [JsonIgnore]
    public Color Color
    {
        get => color;
        set => color = value;
    }

    [JsonIgnore]
    public Vector2 Pivot
    {
        get => pivot;
        set => pivot = value;
    }

    [JsonIgnore]
    public List<string> TagsBundles
    {
        get => tagsBundles;
        set => tagsBundles = value;
    }
    #endregion

    #region CONSTRUCTORS

    public Zone() { }

    public Zone(string id, Color color)
    {
        this.id = id;
        this.color = color;
    }

    #endregion

    #region METHODS

    public object Clone()
    {
        Zone clone;
        try
        {
            clone = CloneRefs.tryGet(this) as Zone;
        }
        catch (Exception e)
        {
            clone = new Zone(id, color);
            CloneRefs.Add(this, clone);
        }

        return clone;
    }

    #endregion
}

public static class ZoneExtension
{
    public static List<Bundle> GetBundles(this Zone zone)
    {
        var bundles = new List<Bundle>();
        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.IsPresset).ToList();
        foreach (var tags in zone.TagsBundles)
        {
            bundles.Add( allBundles.Find(b => b.ID.Label == tags));
        }
        return bundles;
    }
}