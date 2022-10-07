using LBS.Graph;
using LBS.Representation;
using LBS.Representation.TileMap;
using LBS.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class PhysicStepGenerator : Generator
    {
        private static List<Vector2Int> dirs = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        private LBSTileMapData schema;
        private LBSGraphData graph;
        
        public override GameObject Generate()
        {
            var mainPivot = new GameObject("New level 3D");
            foreach (var node in graph.GetNodes())
            {
                var bNames = (node as RoomCharacteristicsData).bundlesNames;
                var bundle = RoomElementBundle.Combine(bNames.Select(n => DirectoryTools.GetScriptable<RoomElementBundle>(n)).ToList());

                var room = schema.GetRoom(node.Label);
                var doors = schema.GetDoors();
                foreach (var tile in room.Tiles)
                {
                    var pivot = new GameObject();
                    pivot.transform.SetParent(mainPivot.transform);
                    pivot.transform.position = new Vector3(tile.GetPosition().x, 0, tile.GetPosition().y); // (* vector de tama�o de tile en mundo) (!)

                    var cBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Center).ToList();
                    var eBundle = bundle.GetCategories().Where(c => c.pivotType == PivotType.Edge).ToList();

                    GenPhysicEdge(eBundle, pivot.transform, tile, schema);
                    /*
                    go = GenPhysicCenter(cat, pivot.transform);

                    foreach (var cat in bundle.GetCategories())
                    {
                        GameObject go;
                        switch (cat.pivotType)
                        {
                            case PivotType.Center:
                                go = GenPhysicCenter(cat, pivot.transform);
                                break;
                            case PivotType.Edge:
                                for (int i = 0; i < dirs.Count; i++) // esto podria cambiar si hay tiles hexagonales o triangulares (?)
                                {
                                    var d = new DoorData("","",tile.GetPosition(), tile.GetPosition() + dirs[i]);
                                    if(doors.Contains(d)) // En esta direccion hay una puerta
                                    {

                                    }
                                    else // Si no hay puerta hay muralla
                                    {

                                    }
                                    go = GenPhysicEdge(cat, pivot.transform,dirs[i]); // implementar construccion de murrallas y puertas (!!!)
                                }
                                break;
                        }
                    }
                    */

                }
            }
            return mainPivot;
        }

        private GameObject GenPhysicCenter(ItemCategory bundle, Transform parent)
        {
            var prefs = bundle.items;
            return SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);
        }

        private GameObject GenPhysicEdge(List<ItemCategory> bundle, Transform parent,TileData tile,LBSTileMapData schema)
        {
            var bWall = bundle.Where(b => b.category == "Wall").ToList();
            var bDoor = bundle.Where(b => b.category == "Door").ToList();

            for (int i = 0; i < dirs.Count; i++)
            {
                var other = schema.GetTile(tile.GetPosition() + dirs[i]);
                if(other == null) // si no hya otro tile pone muralla
                {
                    InstantiateEdge(bWall);
                    continue;
                }

                var doors = schema.GetDoors();
                var tempDoor = new DoorData("", "", tile.GetPosition(), tile.GetPosition() + dirs[i]);
                if (doors.Contains(tempDoor)) // si es una puerta pone puerta
                {
                    InstantiateEdge(bDoor);
                    continue;
                }

                if(!other.GetRoomID().Equals(tile.GetRoomID())) // si son de diferentes habitaciones pone muralla
                {
                    InstantiateEdge(bDoor);
                } // si son de la misma no pone nada
            }

            return null;
        }   

        private void InstantiateEdge(List<ItemCategory> bundles)
        {
            //var prefs = bundle.items;
            //SceneView.Instantiate(prefs[Random.Range(0, prefs.Count)], parent);
        }

        public override void Init(LevelData levelData)
        {
            this.schema = levelData.GetRepresentation<LBSTileMapData>();
            this.graph = levelData.GetRepresentation<LBSGraphData>();
        }
    }
}