using ISILab.Commons.Utility.Editor;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// GABO TODO: TERMINARRR
public class PathOSTileView : GraphElement
{
    #region FIELDS
    private static VisualTreeAsset view;
    // Icono del tag
    VisualElement background;
    VisualElement elementTag;
    // Event tags
    VisualElement dynamicTagObject;
    VisualElement dynamicTagTrigger;
    VisualElement dynamicObstacleObject;
    VisualElement dynamicObstacleTrigger;
    #endregion

    #region CONSTRUCTORS
    public PathOSTileView(PathOSTile tile)
    {
        if (view == null)
        {
            view = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTileView");
        }
        view.CloneTree(this);

        background = this.Q<VisualElement>(name: "Background");
        elementTag = this.Q<VisualElement>(name: "ElementTag");
        dynamicTagObject = this.Q<VisualElement>(name:"DynamicTagObject");
        dynamicTagTrigger = this.Q<VisualElement>(name:"DynamicTagTrigger");
        dynamicObstacleObject = this.Q<VisualElement>(name:"DynamicObstacleObject");
        dynamicObstacleTrigger = this.Q<VisualElement>(name:"DynamicObstacleTrigger");

        // Set data
        SetColor(tile.Tag.Color);
        SetImage(tile.Tag.Icon);
    }
    #endregion

    #region METHODS
    public void SetColor(Color color)
    {
        background.style.backgroundColor = color;
    }

    public void SetImage(Texture2D image)
    {
        elementTag.style.backgroundImage = image;
    }
    #endregion
}
