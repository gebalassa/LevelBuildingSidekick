using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class LBSManipulator : MouseManipulator, IManipulatorLBS
{
    protected Feedback feedback;

    public Action OnManipulationStart;
    public Action OnManipulationUpdate;
    public Action OnManipulationEnd;

    private MainView mainView;
    protected MainView MainView
    {
        get => mainView;
        set => mainView = value;
    }

    private bool started = false;
    private bool ended = false;

    private Vector2Int startClickPosition = Vector2Int.zero;
    private Vector2Int moveClickPosition = Vector2Int.zero;
    private Vector2Int endClickPosition = Vector2Int.zero;

    public Vector2Int StartPosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
    {
        get
        {
            if (started)
            {
                return startClickPosition;
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                return default(Vector2Int);
            }
        }
    }

    public Vector2Int MovePosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
    {
        get
        {
            if (started)
            {
                return moveClickPosition;
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                return default(Vector2Int);
            }
        }
    }

    public Vector2Int EndPosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
    {
        get
        {
            if (ended)
            {
                return endClickPosition;
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                return default(Vector2Int);
            }
        }
    }

    public LBSManipulator()
    {
    }

    public abstract void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnInternalMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnInternalMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnInternalMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnInternalMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnInternalMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnInternalMouseUp);
    }

    private void StartFeedback()
    {
        if (feedback == null)
            return;

        MainView.AddElement(feedback);
        feedback.ActualizePositions(startClickPosition, startClickPosition);
    }

    private void UpdateFeedback()
    {
        if (feedback == null)
            return;

        if (!started)
            return;

        feedback.ActualizePositions(startClickPosition, moveClickPosition);
    }

    private void EndFeedback()
    {
        if (feedback == null)
            return;

        MainView.RemoveElement(feedback);
    }

    protected void OnInternalMouseDown(MouseDownEvent e)
    {
        if (e.button != 0)
            return;

        this.started = true;
        this.startClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        StartFeedback();

        OnManipulationStart?.Invoke();
        OnMouseDown(e.target as VisualElement, startClickPosition, e);
    }

    protected void OnInternalMouseMove(MouseMoveEvent e)
    {
        if (e.button != 0)
            return;

        this.moveClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        UpdateFeedback();

        OnMouseMove(e.target as VisualElement, moveClickPosition, e);
        OnManipulationUpdate?.Invoke();
    }

    protected void OnInternalMouseUp(MouseUpEvent e)
    {
        if (e.button != 0)
            return;

        this.ended = true;
        this.endClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        EndFeedback();

        OnMouseUp(e.target as VisualElement, endClickPosition, e);
        OnManipulationEnd?.Invoke();

        this.ended = this.started = false;
    }

    protected abstract void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e);

    protected abstract void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e);

    protected abstract void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e);

    public void AddManipulationStart(Action action)
    {
        OnManipulationStart += action;
    }

    public void AddManipulationUpdate(Action action)
    {
        OnManipulationUpdate += action;
    }

    public void AddManipulationEnd(Action action)
    {
        OnManipulationEnd += action;
    }

    public void RemoveManipulationStart(Action action)
    {
        OnManipulationStart -= action;
    }

    public void RemoveManipulationUpdate(Action action)
    {
        OnManipulationUpdate -= action;
    }

    public void RemoveManipulationEnd(Action action)
    {
        OnManipulationEnd -= action;
    }
}


public interface IManipulatorLBS
{
    public void AddManipulationStart(Action action);
    public void AddManipulationUpdate(Action action);
    public void AddManipulationEnd(Action action);

    public void RemoveManipulationStart(Action action);
    public void RemoveManipulationUpdate(Action action);
    public void RemoveManipulationEnd(Action action);

    public void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);
}