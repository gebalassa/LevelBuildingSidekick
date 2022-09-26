using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Utility;
using LBS.Representation.TileMap;
using LBS;
using UnityEditor.Overlays;

namespace LBS.Windows
{
    public class LBSSchemaWindow : GenericGraphWindow, ISupportsOverlays
    {
        private LBSSchemaWindow() { }

        [MenuItem("ISILab/LBS plugin/Schema window")]
        [LBSWindow("Schema window")]
        public static void OpenWindow()
        {
            var  wnd = GetWindow<LBSSchemaWindow>();
            wnd.titleContent = new GUIContent("Schema window");
        }

        // este metodo deberia tener parametros tipo (out action, out nextW, out prevW)
        // para boligar a que se immplementen estas coas aqui y no se tenga que intuir. (?)
        public override void OnInitPanel()
        {
            actions.Add(new System.Tuple<string, System.Action>(
                "Generate 3D",
                () => Generate3D.GenerateLevel(LBSController.CurrentLevel)
                ));

            actions.Add(new System.Tuple<string, System.Action>(
                "Optimize",
                () => {
                    // controllers.Find(c => c.GetType() == typeof(LBSTileMapController)) as LBSTileMapController;
                    var c = GetControllerByType<LBSTileMapController>();
                    var schema = c.Optimize();
                    schema = c.RecalculateDoors(schema);
                    LBSController.CurrentLevel.data.AddRepresentation(schema);
                    this.RefreshView();
                }));

            nextWindow = typeof(LBSPopulationWindow);
            prevWindow = typeof(LBSGraphRCWindow);
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)
            var tileData = data.GetRepresentation<LBSTileMapData>();
            AddController(new LBSTileMapController(MainView, tileData));
        }
    }
}