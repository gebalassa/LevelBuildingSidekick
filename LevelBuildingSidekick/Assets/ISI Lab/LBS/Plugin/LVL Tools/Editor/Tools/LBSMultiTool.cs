using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class LBSMultiTool : LBSTool
{
    [SerializeField]
    private List<string> modes;
    [SerializeField]
    private List<string> manipulators = new List<string>();

    private List<LBSManipulator> _manipulators = new List<LBSManipulator>();

    public LBSMultiTool(Texture2D icon, string name,List<string> modes, List<Type> manipulators, Type inspector, bool useUnitySelector = false) : base(icon, name, manipulators[0], inspector, useUnitySelector)
    {
        this.modes = modes;
        foreach (var manipulator in manipulators)
        {
            this.manipulators.Add(manipulator.FullName);
        }
        this.inspector = inspector?.FullName;
    }

    public override ToolButton InitButton(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        _manipulators = new List<LBSManipulator>();
        base.InitButton(view, ref level, ref layer, ref module); // (?) inecesario?
        foreach (var monipulator in manipulators)
        {
            var mType = Type.GetType(this.manipulator);
            var current = Activator.CreateInstance(mType) as LBSManipulator;
            current.OnManipulationStart += OnStartAction;
            current.OnManipulationUpdate += OnUpdateAction;
            current.OnManipulationEnd += OnEndAction;

            current.Init(ref view, ref level, ref layer, ref module);
            _manipulators.Add(current);

        }

        var btn = new DropdownToolButton(this.icon, this.name, modes);
        btn.OnModeChange += (index, name) => SetManipulator(index, view, btn);

        SetManipulator(0, view, btn);

        btn.OnFocusEvent += () => {
            view.AddManipulator(_manipulator);
            if (UseUnitySelector)
            {
                view.AddManipulator(new ClickSelector());
            }
        };
        btn.OnBlurEvent += () => {
            view.RemoveManipulator(_manipulator);
        };
        return btn;
    }

    public override LBSInspector InitInspector(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        var iType = Type.GetType(this.inspector);
        _inspector = Activator.CreateInstance(iType) as LBSInspector;
        _inspector.Init(new List<LBSManipulator>(_manipulators) { }, ref view, ref level, ref layer, ref module);

        return _inspector;
    }

    private void SetManipulator(int n, MainView view, DropdownToolButton button)
    {
        view.RemoveManipulator(_manipulator);
        _manipulator = _manipulators[n];

        button.OnFocusEvent += () => {
            view.AddManipulator(_manipulator);
            if (UseUnitySelector)
            {
                view.AddManipulator(new ClickSelector());
            }
        };
        button.OnBlurEvent += () => {
            view.RemoveManipulator(_manipulator);
        };
    }
}