using LBS.Behaviours;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSTool
{
    #region FIELDS
    private Texture2D icon;
    private string name;
    private LBSManipulator manipulator;
    #endregion

    #region PROPERTIES
    public Texture2D Icon => icon;
    public string Name => name;
    public LBSManipulator Manipulator => manipulator;
    #endregion

    #region EVENTS
    public event Action OnStart;
    public event Action OnPressed;
    public event Action  OnEnd;
    #endregion

    #region CONSTRUCTORS
    public LBSTool(Texture2D icon, string name, LBSManipulator manipulator,
        Feedback feedback = null,
        Action OnStart = null,
        Action OnUpdate = null,
        Action OnEnd = null) // el feedback y los eventos no se estan usando (!)
    {
        this.icon = icon;
        this.name = name;
        this.manipulator = manipulator;
    }
    #endregion

    #region METHODS
    public virtual void Init(LBSLayer layer, LBSBehaviour behaviour)
    {
        manipulator.AddManipulationStart(OnStart);
        manipulator.AddManipulationUpdate(OnPressed);
        manipulator.AddManipulationEnd(OnEnd);

        manipulator.Init(layer, behaviour);

    }

    public void BindButton(ToolButton button)
    {
        var canvas = MainView.Instance;

        button.OnFocusEvent += () => {
            canvas.AddManipulator(this.manipulator);
        };
        button.OnBlurEvent += () => {
            canvas.RemoveManipulator(this.manipulator);
        };
    }
    #endregion;
}
