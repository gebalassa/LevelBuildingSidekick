using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : GraphView
{
    #region UXML_FACTORY
    public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }
    #endregion

    #region FIELDS
    
    private ExternalBounds visualBound;
    private List<Manipulator> manipulators = new List<Manipulator>();
    private Vector2 tileSize = new Vector2(100, 100);
    
    #endregion

    #region PROPERTIES
    public Vector2 TileSize => tileSize;
    #endregion

    #region EVENTS

    public Action<ContextualMenuPopulateEvent> OnBuild;
    public Action OnClearSelection;

    #endregion

    #region CONSTRUCTORS

    public MainView()
    {
        Insert(0, new GridBackground());
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MainViewUSS");
        styleSheets.Add(styleSheet);
        style.flexGrow = 1;

        SetBasicManipulators();
        InitBound(4000,10000);

        AddElement(visualBound);

    }

    #endregion

    #region METHODS

    public void SetBasicManipulators() // necesario aqui (?)
    {
        var manis = new List<Manipulator>() {
                new ContentZoomer(),
                new ContentDragger(),
                new SelectionDragger(),
            };

        SetManipulators(manis);
    }

    public override void ClearSelection() // (?)
    {
        base.ClearSelection();
        if (selection.Count == 0)
        {
            OnClearSelection?.Invoke();
        }
    }

    public void SetManipulator(Manipulator current)
    {
        ClearManipulators();
        this.AddManipulator(current);
    }

    public void SetManipulators(List<Manipulator> manipulators)
    {
        ClearManipulators();
        AddManipulators(manipulators);
    }

    public void ClearManipulators()
    {
        foreach (var m in this.manipulators)
        {
            this.RemoveManipulator(m as IManipulator);
        }
        this.manipulators.Clear();
    }

    public void RemoveManipulator(Manipulator manipulator)
    {
        this.manipulators.Remove(manipulator);
        this.RemoveManipulator(manipulator as IManipulator);
    }

    public void RemoveManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            this.manipulators.Remove(m);
            this.RemoveManipulator(m as IManipulator);
        }
    }

    public void AddManipulator(Manipulator manipulator)
    {
        this.manipulators.Add(manipulator);
        this.AddManipulator(manipulator as IManipulator);
    }

    public void AddManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            if (!this.manipulators.Contains(m))
            {
                this.manipulators.Add(m);
                this.AddManipulator(m as IManipulator);
            }
        }
    }

    public void ClearView()
    {
        this.graphElements.ForEach(e => this.RemoveElement(e));
        AddElement(visualBound);
    }

    public new void AddElement(GraphElement element)
    {
        base.AddElement(element);
    }

    public Vector2 FixPos(Vector2 v) // (?) esto deberia estar aqui? 
    {
        var t = new Vector2(this.viewTransform.position.x, this.viewTransform.position.y);
        var vv = (v - t) / this.scale;
        return vv;
    }

    public Vector2Int ToTileCords(Vector2 vec)
    {
        var nPos = new Vector2Int((int)(vec.x / tileSize.x), (int)(vec.y / tileSize.y));

        if (vec.x < 0)
            nPos.x -= 1;

        if (vec.y < 0)
            nPos.y -= 1;

        return nPos;
    }

    private void InitBound(int interior, int exterior)
    {
        var dif = exterior - interior;
        this.visualBound = new ExternalBounds(
            new Rect(
                new Vector2(-interior, -interior),
                new Vector2(interior, interior)
                ),
            new Rect(
                new Vector2(-exterior, -exterior),
                new Vector2(exterior, exterior)
                )
            );
    }

    #endregion
}
