using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddConnection<T> : ManipulateTeselation<T> where T : LBSTile
{
    public LBSIdentifier tagToSet;

    private List<Vector2Int> dirs = new List<Vector2Int>() // (!) esto deberia estar en un lugar general
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up
    };

    private ConnectedTile first;

    public AddConnection() : base()
    {
        feedback = new ConectedLine();
        feedback.fixToTeselation = true;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        //OnManipulationStart?.Invoke();

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        first = tile.Data;
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        if (first == null)
            return;

        var pos = module.Owner.ToFixedPosition(position);

        var dx = (first.Position.x - pos.x);
        var dy = (first.Position.y - pos.y);
        var fDir = dirs.FindIndex(d => d.Equals(-new Vector2Int(dx, dy)));

        if (fDir < 0 || fDir >= dirs.Count)
            return;


        if (e.target is MainView)
        {
            first.SetConnection(tagToSet.Label, fDir);
            return;
        }

        var tile = e.target as ExteriorTileView;
        if (tile == null)
            return;

        var second = tile.Data;

        if (first == second)
            return;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1f)
            return;

        var tDir = dirs.FindIndex(d => d.Equals(new Vector2Int(dx, dy)));

        first.SetConnection(tagToSet.Label, fDir);
        second.SetConnection(tagToSet.Label, tDir);

    }
}
