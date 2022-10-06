using LBS.Windows;
using LBS.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Representation.TileMap;
using LBS.Representation;

namespace LBS.Manipulators
{
    public class AddTileManipulator : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericGraphWindow window;

        private RoomData cRoom;

        public AddTileManipulator(GenericGraphWindow window, LBSTileMapController controller,RoomData cRoom)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.controller = controller;
            this.window = window;
            this.cRoom = cRoom;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if(cRoom == null)
            {
                Debug.Log("No hay habitacion seleccionada");
                return;
            }

            var pos = controller.ViewportMousePosition(e.localMousePosition);
            var tPos = controller.ToTileCoords(pos);
            var schema = LBSController.CurrentLevel.data.GetRepresentation<LBSTileMapData>();
            var tile = new TileData(tPos, cRoom.ID);
            schema.AddTile(tile,cRoom.ID);
            window.RefreshView();
        }
    }
}
