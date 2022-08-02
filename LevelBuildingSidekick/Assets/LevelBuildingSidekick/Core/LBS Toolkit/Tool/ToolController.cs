using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using LevelBuildingSidekick;
using UnityEngine;

public abstract class ToolController : Controller
{
    public Vector2 CurrentPos { get; set; } // podria ser una variable solo de metodo y no global
    protected ToolkitController Toolkit { get; set; }
    public bool IsActive { get; set; }
    protected ToolController(Data data, ToolkitController toolkit) : base(data)
    {
        Toolkit = toolkit;
    }

    public virtual void Switch()
    {
        if(Toolkit != null)
        {
            Toolkit.Switch(this);
        }
    }

    public abstract void Action(LevelRepresentationController level);

    internal void InternalUpdate()
    {
        if (IsActive)
        {
            Event e = Event.current;
            CurrentPos = e.mousePosition;
            if (e.button == 0 && e.type.Equals(EventType.MouseDown))
            {
                OnMouseDown(CurrentPos);
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseDrag))
            {
                OnMouseDrag(CurrentPos);
            }
            if (e.button == 0 && e.type.Equals(EventType.MouseUp))
            {
                OnMouseUp(CurrentPos);
            }
        }
    }

    public override void Update() { }

    public virtual void OnMouseDown(Vector2 position) { }
    public virtual void OnMouseUp(Vector2 position) { }
    public virtual void OnMouseDrag(Vector2 position) { }

    
}
