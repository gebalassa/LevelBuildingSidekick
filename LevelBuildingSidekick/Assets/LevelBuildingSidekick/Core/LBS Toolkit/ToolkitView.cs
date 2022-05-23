using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;

public class ToolkitView : View
{

    public ToolkitView(Controller controller) : base(controller)
    {
        ToolkitOverlay.draw = DrawToolkit;
    }

    public override void Display()
    {
        Draw();
    }

    public void DrawToolkit()
    {
        //GUILayout.Label("Toolkit");
        var controller = Controller as ToolkitController;
        //controller.Update();
        foreach (ToolController t in controller.ToolControllers)
        {
            (t.View as ToolView).DisplayInToolkit();
        }
    }

    public override void Draw()
    {
        var controller = Controller as ToolkitController;
        foreach (ToolController t in controller.ToolControllers)
        {
            t.View.Display();
        }
    }

}
