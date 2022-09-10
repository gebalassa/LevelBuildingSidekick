﻿using LBS.Representation.TileMap;
using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Windows
{
    public class PopulationWindow : GenericGraphWindow, ISupportsOverlays
    {
        private PopulationWindow() { }

        [MenuItem("ISILab/LBS plugin/Population Window")]
        [LBSWindow("Population Window")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<PopulationWindow>();
            wnd.titleContent = new GUIContent("Population window");
        }

        public override void OnInitPanel()
        {
            actions.Add(new System.Tuple<string, System.Action>(
                "Generate 3D",
                () => Debug.Log("[Implementar]")
                ));

            nextWindow = typeof(LBSQuestWindow);
            prevWindow = typeof(LBSSchemaWindow);
        }

        public override void OnLoadControllers()
        {
            var data = LBSController.CurrentLevel.data; // peligroso buscar otra forma (!)

            controllers.Add(new LBSTileMapController(MainView, data.GetRepresentation<LBSTileMapData>()));
            controllers.Add(new LBSStampController(MainView, data.GetRepresentation<LBSStampGroupData>()));
        }

    }
}
