using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation.TileMap;
using LBS.Graph;
using System.Linq;
using LBS.Schema;

namespace LBS.Transformers
{
    public class GraphToTileMap : Transformer<LBSGraphData, LBSSchemaData>
    {
        public override LBSSchemaData Transform(LBSGraphData graph)
        {
            if (graph.NodeCount() <= 0)
            {
                Debug.LogWarning("[Error]: 'Graph node' have 0 nodes.");
                return null;
            }

            Queue<LBSNodeData> open = new Queue<LBSNodeData>();
            HashSet<LBSNodeData> closed = new HashSet<LBSNodeData>();

            var parent = graph.GetNodes().OrderByDescending((n) => graph.GetNeighbors(n).Count).First() as RoomCharacteristicsData;
            open.Enqueue(parent);
            //Debug.Log("parent: "+parent.Label);

            var tileMap = new LBSSchemaData();
            int h = Random.Range(parent.RangeHeight.min, parent.RangeHeight.max);
            int w = Random.Range(parent.RangeWidth.min, parent.RangeWidth.max);
            tileMap.AddRoom(Vector2Int.zero, w, h, parent.Label);

            while (open.Count > 0)
            {
                parent = open.Dequeue() as RoomCharacteristicsData;

                var childs = graph.GetNeighbors(parent);
                //var childs = graph.GetNeighbors(parent).OrderBy(n => Utility.MathTools.GetAngleD15(parent.Centroid, n.Centroid)).Select( c => c as RoomCharacteristicsData);

                foreach (var child in childs)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    open.Enqueue(child);

                    var parentH = Random.Range(parent.RangeHeight.min, parent.RangeHeight.max); 
                    var parentW = Random.Range(parent.RangeWidth.min, parent.RangeWidth.max);
                    var childH = Random.Range(child.RangeHeight.min, child.RangeHeight.max);
                    var childW = Random.Range(child.RangeWidth.min, child.RangeWidth.max);

                    var dir = ((Vector2)(child.Centroid - parent.Centroid)).normalized;
                    var posX = dir.x * ((childW + parentW) / 2f) * 1.41f;
                    var posY = dir.y * ((childH + parentH) / 2f) * 1.41f;
                    tileMap.AddRoom(new Vector2Int((int)posX, (int)posY), childW, childH, child.Label);
                }

                closed.Add(parent);
            }

            var tiles = tileMap.GetRooms().SelectMany(r => r.Tiles);

            var x = tiles.Min(t => t.GetPosition().x);
            var y = tiles.Min(t => t.GetPosition().y);

            var dp = new Vector2Int(x,y);

            foreach(var t in tiles)
            {
                t.SetPosition(t.GetPosition() - dp);
            }

            return tileMap;
        }
    }
}
