using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS.Generator
{
    public class ExteriorGenerator : Generator3D //  (!!!) esta clase mescla lo que tiene que hacer la IA de WFC con generar 3d posteriormente
    {
        private Exterior exterior;

        public override GameObject Generate(LBSLayer layer)
        {
            Init(layer);

            var l = layer.GetModule<Exterior>();
            var tiles = Utility.DirectoryTools.GetScriptables<WFCBundle>();

            var mainPivot = new GameObject(objName);
            foreach (var tile in l.Tiles.Select(t => t as ConnectedTile))
            {
                var tuple = tiles.Get(tile.Connections);

                if (tuple == null)
                    continue;

                var pref = tuple.Item1.Pref;
                var go = GameObject.Instantiate(pref, mainPivot.transform);
                go.transform.position = new Vector3(tile.Position.x * scale.x, 0, -tile.Position.y * scale.y);

                var rot = tuple.Item2;
                if (rot % 2 != 0) // parche, debido a mirror en eje Y
                    rot += 2;

                //go.transform.Rotate(new Vector3(0, -1, 0), 90); // (!!!) esto es un �parche por que el WFC empieza con Right a la derecha y estpo empieza con Top a la derecha
                go.transform.rotation = Quaternion.Euler(0, (90 * (rot)) % 360, 0);
                //for (int k = 0; k < rot; k++)
                //    go.transform.Rotate(new Vector3(0, 1, 0), 90); //
            }
            mainPivot.transform.position += position;
            return mainPivot;
        }

        public override void Init(LBSLayer layer)
        {
            exterior = layer.GetModule<Exterior>();
        }
    }
}