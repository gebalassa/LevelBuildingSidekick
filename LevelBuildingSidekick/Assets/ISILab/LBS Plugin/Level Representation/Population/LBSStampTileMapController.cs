using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSStampTileMapController : LBSStampController, ITileMap
{
    public static int UnitSize = 100; //esto no dbeer�a estar aca(!!!)

    public float Subdivision { get; set; }

    public LBSStampTileMapController(LBSGraphView view, LBSStampGroupData data) : base(view, data)
    public float TileSize { get { return UnitSize / Subdivision; } }

    {
        Subdivision = 1;
    }

    public override void CreateStamp(ContextualMenuPopulateEvent evt, GraphView view, StampPresset stamp)
    {
        var viewPos = new Vector2(view.viewTransform.position.x, view.viewTransform.position.y);
        var pos = (evt.localMousePosition - viewPos) / view.scale;

        pos = ToTileCoords(pos);

        var newStamp = new StampData(stamp.name, pos);
        data.AddStamp(newStamp);
        view.AddElement(new StampView(newStamp));
    }

    public Vector2 ToTileCoords(Vector2 position)
    {
        int x = (int)((position.x / TileSize) - (position.x % TileSize));
        int y = (int)((position.y / TileSize) - (position.y % TileSize));

        return new Vector2(x, y);
    }

    public Vector2 FromTileCoords(Vector2 position)
    {
        return position * TileSize;
    }
}
