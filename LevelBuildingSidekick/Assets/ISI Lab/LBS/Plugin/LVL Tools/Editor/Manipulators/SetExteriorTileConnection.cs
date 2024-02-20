using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Components.TileMap;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.VisualElements;
using ISILab.Commons.VisualElements;

namespace ISILab.LBS.Manipulators
{
    public class SetExteriorTileConnection : LBSManipulator
    {
        private List<Vector2Int> Directions => Commons.Directions.Bidimencional.Edges;

        private ExteriorBehaviour exterior;
        private Vector2Int first;

        private ConnectedConrnerLine lineFeedback = new ConnectedConrnerLine();
        private Feedback areaFeedback = new AreaFeedback();

        public LBSIdentifier ToSet
        {
            get => exterior.identifierToSet;
            set => exterior.identifierToSet = value;
        }

        public SetExteriorTileConnection() : base()
        {
            lineFeedback.fixToTeselation = true;
            areaFeedback.fixToTeselation = true;
            feedback = lineFeedback;
        }

        public override void Init(LBSLayer layer, object behaviour)
        {
            exterior = behaviour as ExteriorBehaviour;
            lineFeedback.TeselationSize = layer.TileSize;
            areaFeedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) =>
            {
                lineFeedback.TeselationSize = val;
                areaFeedback.TeselationSize = val;
            };
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
        {
            first = exterior.Owner.ToFixedPosition(position);
        }

        protected override void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e)
        {
            lineFeedback.LeftSide = e.shiftKey;

            if (!e.ctrlKey)
            {
                SetFeedback(lineFeedback);
            }
            else
            {
                SetFeedback(areaFeedback);
            }
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
            if (ToSet == null || ToSet.Label == "")
            {
                Debug.LogWarning("NO tienes ninguna conexion seleccionada");
                return;
            }

            // Get end position
            var end = exterior.Owner.ToFixedPosition(position);

            if (!e.ctrlKey)
            {
                LineEffect(end, e);
            }
            else
            {
                AreaEffect(end, e);
            }
        }

        public void LineEffect(Vector2Int end, MouseUpEvent e)
        {
            // Get corner position
            var corner = e.shiftKey ?
                new Vector2Int(first.x, end.y) :
                new Vector2Int(end.x, first.y);

            List<(LBSTile, Vector2Int, Vector2Int)> path = new();

            // Get first path
            Vector2Int current = first;
            while (!current.Equals(corner))
            {
                var tile = exterior.GetTile(current);
                var dir = ((Vector2)(corner - first)).normalized.ToInt();

                path.Add((tile, new Vector2Int(current.x, current.y), dir));
                current += dir;
            }

            // Get second path
            current = corner;
            while (!current.Equals(end))
            {
                var tile = exterior.GetTile(current);
                var dir = ((Vector2)(end - corner)).normalized.ToInt();

                path.Add((tile, new Vector2Int(current.x, current.y), dir));
                current += dir;
            }

            for (int i = 0; i < path.Count; i++)
            {
                var t1 = path[i].Item1;
                var fDir = Directions.FindIndex(d => d.Equals(path[i].Item3));

                if (t1 != null)
                {
                    exterior.SetConnection(t1, fDir, ToSet.Label, false);
                }

                var t2 = exterior.GetTile(path[i].Item2 + path[i].Item3);
                var dDir = Directions.FindIndex(d => d.Equals(-path[i].Item3));

                if (t2 != null)
                {
                    exterior.SetConnection(t2, dDir, ToSet.Label, false);
                }
            }
        }

        public void AreaEffect(Vector2Int end, MouseUpEvent e)
        {
            var corners = exterior.Owner.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = exterior.GetTile(pos);

                    if (tile == null)
                    {
                        continue;
                    }

                    for (int k = 0; k < Directions.Count; k++)
                    {
                        exterior.SetConnection(tile, k, ToSet.Label, false);

                        var dir = Directions[k];
                        var neig = exterior.GetTile(pos + dir);

                        if (neig != null)
                        {
                            exterior.SetConnection(neig, (k + 2) % 4, ToSet.Label, false);
                        }
                    }
                }
            }
        }
    }
}