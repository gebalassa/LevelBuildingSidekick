using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class PSEditorView : View
    {
        GenericWindow Window { get; set; }

        public PSEditorView(Controller controller):base(controller)
        {
            //Window = EditorWindow.GetWindow<GenericWindow>();
            //Window.minSize = Vector2.one* 100;
            //Window.Show();
            //Window.draw = Draw2D;
            //Debug.Log("D: " + (Controller.Data as PSEditorData));

        }

        public override void Draw2D()
        {
            PSEditorController controller = Controller as PSEditorController;

            
            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
            //GUILayout.BeginArea(new Rect(0, 0, 1000, 1000));
            //Debug.Log((Controller as PSEditorController).Level);
            //Debug.Log((Controller as PSEditorController).Level.View);
            //GUILayout.BeginVertical();
            controller.Update();
            controller.LevelRepresentation.View.Draw2D();
            //GUILayout.EndVertical();
            //GUILayout.EndArea();
            //GUILayout.EndScrollView();
            
        }


    }
}

