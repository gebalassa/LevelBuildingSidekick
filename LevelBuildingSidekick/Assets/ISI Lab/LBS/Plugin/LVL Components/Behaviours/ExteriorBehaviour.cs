using LBS.Behaviours;
using LBS.Components;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(TileMapModule), typeof(ConnectedTileMapModule))]
public class ExteriorBehaviour : LBSBehaviour
{
    #region FIELDS
    [JsonRequired, SerializeField]
    private TileMapModule tileMap;
    [JsonRequired, SerializeField]
    private ConnectedTileMapModule connections;
    #endregion

    #region PROPERTIES
    
    #endregion

    #region CONSTRUCTORS
    public ExteriorBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    public override object Clone()
    {
        return new ExteriorBehaviour(this.Icon, this.Name);
    }

    public override void OnAdd(LBSLayer layer)
    {
        Owner = layer;

        tileMap = Owner.GetModule<TileMapModule>();
        connections = Owner.GetModule<ConnectedTileMapModule>();
    }
}
