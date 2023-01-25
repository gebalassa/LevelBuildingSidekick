using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LBSTool
{
    [SerializeField]
    public Texture2D icon;
    [SerializeField]
    public string name;
    [SerializeField]
    public string manipulator;
    [SerializeField]
    public string inspector;
    [SerializeField]
    public bool UseUnitySelector;

    [NonSerialized]
    protected LBSManipulator _manipulator;
    [NonSerialized]
    protected LBSInspector _inspector;

    public Action OnStartAction;
    public Action OnUpdateAction;
    public Action OnEndAction;

    public LBSTool(Texture2D icon, string name, Type manipulator, Type inspector, bool useUnitySelector = false)
    {
        this.icon = icon;
        this.name = name;
        this.manipulator = manipulator.FullName;
        this.inspector = inspector?.FullName;
        this.UseUnitySelector = useUnitySelector; // (!) esto es un parche, empty deberia ser el unico con este comportamiento
    }

    public virtual ToolButton InitButton(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module) // (!)
    {
        var mType = Type.GetType(this.manipulator);
        _manipulator = Activator.CreateInstance(mType) as LBSManipulator;
        _manipulator.OnManipulationStart += OnStartAction;
        _manipulator.OnManipulationUpdate += OnUpdateAction;
        _manipulator.OnManipulationEnd += OnEndAction;

        _manipulator.Init(ref view,ref level, ref layer, ref module);

        var btn = new BasicToolButton(this.icon, this.name);

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

    public virtual LBSInspector InitInspector(MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        var iType = Type.GetType(this.inspector);
        _inspector = Activator.CreateInstance(iType) as LBSInspector;
        _inspector.Init(new List<LBSManipulator>() { _manipulator }, ref view, ref level, ref layer, ref module);

        return _inspector;
    }

}