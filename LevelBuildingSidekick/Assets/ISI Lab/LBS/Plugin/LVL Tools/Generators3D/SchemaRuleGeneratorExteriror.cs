using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LBS.Bundles;
using Newtonsoft.Json;
using ISILab.Commons;
using ISILab.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ISILab.LBS.Generators
{

    [System.Serializable]
    [RequieredModule(typeof(TileMapModule),
        typeof(ConnectedTileMapModule),
        typeof(SectorizedTileMapModule),
        typeof(ConnectedZonesModule))]
    public class SchemaRuleGeneratorExteriror : LBSGeneratorRule
    {
        [JsonRequired]
        private float deltaWall = 1f;

        #region INTERNAL FIELDS
        [JsonIgnore]
        private TileMapModule tilesMod;
        [JsonIgnore]
        private ConnectedTileMapModule connectedTilesMod;
        [JsonIgnore]
        private SectorizedTileMapModule zonesMod;
        [JsonIgnore]
        private Generator3D.Settings settings;
        #endregion

        #region PPROPERTIES
        [JsonIgnore]
        private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;
        [JsonIgnore]
        private List<Vector2Int> DirDiags => Directions.Bidimencional.Diagonals;
        #endregion

        #region CONSTRUCTORS
        public SchemaRuleGeneratorExteriror() { }
        #endregion

        public void Init(LBSLayer layer, Generator3D.Settings settings)
        {
            this.tilesMod = layer.GetModule<TileMapModule>();
            this.connectedTilesMod = layer.GetModule<ConnectedTileMapModule>();
            this.zonesMod = layer.GetModule<SectorizedTileMapModule>();
            this.settings = settings;
        }

        public override List<Message> CheckViability(LBSLayer layer)
        {
            var msgs = new List<Message>();
            var zonesMod = layer.GetModule<SectorizedTileMapModule>();

            return msgs;
        }

        private GameObject GenerateEdges(GameObject pivot, List<Bundle> bundles, List<string> connections, LBSTile tile)
        {
            // Get "Edge" bundles
            var currents = new List<Bundle>();
            foreach (var bundle in bundles)
            {
                currents = bundle.GetChildrenByPositioning(Positioning.Edge);
            }

            for (var i = 0; i < connections.Count; i++)
            {
                var neig = tilesMod.GetTileNeighbor(tile, Dirs[i]);
                if (neig != null)
                    continue;

                // Get random bundle with respctive "connection tag"
                var current = currents.Where(b => b.GetCharacteristics<LBSTagsCharacteristic>()
                .Any(c => c.Value.name == connections[i])).ToList().Random();

                // check if current is valid
                if (current == null)
                {
                    Debug.Log("Los bundles no contienen elemetos con la tag: '" + connections[i] + "'");
                    continue;
                }

                // Get random by weight
                var pref = current.Assets.RandomRullete(a => a.probability).obj;

                // Create part
                var obj = CreateObject(pref, pivot.transform);

                // Set rotation orientation
                if (i % 2 == 0)
                    obj.transform.rotation = Quaternion.Euler(0, (90 * (i - 1)) % 360, 0);
                else
                    obj.transform.rotation = Quaternion.Euler(0, (90 * (i - 3)) % 360, 0);

                // Set delta position
                obj.transform.position = new Vector3(
                    settings.scale.x / 2f * -obj.transform.forward.x,
                    0,
                    settings.scale.y / 2f * -obj.transform.forward.z);

                obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles + new Vector3(0, 90 * 2, 0));
            }

            return pivot;
        }

        private GameObject GenerateCorners(GameObject pivot, List<Bundle> bundles, LBSTile tile)
        {
            var currents = new List<Bundle>();
            foreach (var bundle in bundles)
            {
                currents = bundle.GetChildrenByPositioning(Positioning.Corner);
            }

            var selfConnections = connectedTilesMod.GetConnections(tile);
            for (int i = 0; i < Dirs.Count; i++)
            {
                // Get neigbors 
                var d1 = Dirs[i];
                var neigth = tilesMod.GetTileNeighbor(tile, d1);
                var d2 = Dirs[(i + 1) % Dirs.Count];
                var neigth2 = tilesMod.GetTileNeighbor(tile, d2);
                var neigth3 = tilesMod.GetTileNeighbor(tile, d1 + d2);

                if (neigth == null && neigth2 == null && neigth3 == null)
                {
                    // Get random bundle with respctive "connection tag"
                    var current = currents.Random();

                    // get random by weight
                    var pref = current.Assets.RandomRullete(a => a.probability).obj;
                    var instance = CreateObject(pref, pivot.transform);

                    // Set delta position
                    var dir = d1 + d2;
                    instance.transform.position = new Vector3(
                        settings.scale.x / 2f * dir.x,
                        0,
                        settings.scale.y / 2f * dir.y) * deltaWall;

                    // Set rotation orientation
                    var rot = (i - 1) % Dirs.Count();
                    instance.name = "R:" + rot + " i:" + i;
                    instance.transform.rotation = Quaternion.Euler(0, -90 * rot, 0);
                }
            }

            return pivot;
        }

        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            // Init values
            Init(layer, settings);

            // Get bundles
            var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.IsPresset).ToList();
            var rootBundles = allBundles.Where(b => b.IsRoot()).ToList();

            // Create pivot
            var mainPivot = new GameObject("Schema outside");

            foreach (var tile in tilesMod.Tiles)
            {
                // Get zone
                var zone = zonesMod.GetZone(tile);

                // Get bundle from current tile
                var bundles = zone.GetOutsideBundles();

                if (bundles.Count <= 0)
                {
                    Debug.LogWarning("No se pudo finalizar la generacion de la zona '" + zone.ID + "' " +
                        "ya que no contiene bundles que definan su estilo exterior");

                    continue;
                }

                // Get connections
                var connections = connectedTilesMod.GetConnections(tile);

                //Generate tile
                var tileObj = new GameObject(tile.Position.ToString());

                // Add pref part to pivot
                GenerateEdges(tileObj, bundles, connections, tile);
                GenerateCorners(tileObj, bundles, tile);

                // Set position
                tileObj.transform.position =
                    settings.position +
                    new Vector3(tile.Position.x * settings.scale.x, 0, tile.Position.y * settings.scale.y) +
                    -(new Vector3(settings.scale.x, 0, settings.scale.y) / 2f);

                // Set mainPivot as the parent of tileObj
                tileObj.transform.parent = mainPivot.transform;
            }

            return mainPivot;
        }

        public override object Clone()
        {
            return new SchemaRuleGeneratorExteriror();
        }

        private GameObject CreateObject(GameObject pref, Transform pivot)
        {
#if UNITY_EDITOR
            var obj = PrefabUtility.InstantiatePrefab(pref, pivot) as GameObject;
#else
        var obj =  GameObject.Instantiate(pref, pivot);
#endif
            return obj;
        }

        public override bool Equals(object obj)
        {
            var other = obj as SchemaRuleGeneratorExteriror;

            if (other == null) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}