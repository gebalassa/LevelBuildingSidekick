using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DrawSimpleGraph : Drawer
{
    public Color color1 = new Color(244f / 255f, 129f / 255f, 88f / 255f);
    public Color color2 = new Color(108f / 255f, 137f / 255f, 215f / 255f);

    public DrawSimpleGraph() : base() { }

    public override void Draw(ref LBSLayer layer, MainView view)
    {
        var graph = layer.GetModule<GraphModule<RoomNode>>();

        var pixelSize = layer.TileSize * LBSSettings.Instance.TileSize;

        var nViews = new List<LBSNodeView<RoomNode>>();
        foreach (var node in graph.GetNodes())
        {
            // Node
            var size = new Vector2(120, 120);
            var element = new LBSNodeView<RoomNode>(node, node.Position - (size / 2f), size);

            element.SetText(node.ID);
            element.SetColor(node.Room.Color);
            element.OnGVC += (mgc) => { DrawArea(mgc, element.GetPosition().size, node.Room.Size, pixelSize); };
            
            /*
            // Slider Width
            var sliderW = new SliderInt(1, 32);
            sliderW.style.flexGrow = 1f;
            sliderW.style.height = 16;
            var labelW = new Label();
            sliderW.RegisterCallback<ChangeEvent<int>>(e => {
                labelW.text = "W: "+ e.newValue.ToString();
                node.Room.Width = e.newValue;
                element.MarkDirtyRepaint();
            });
            sliderW.value = node.Room.Width;
            labelW.text = "W: " + node.Room.Width.ToString();
            var contW = new VisualElement();
            contW.Add(labelW);
            contW.Add(sliderW);
            contW.style.flexDirection = FlexDirection.Row;
            contW.style.height = 16;

            // Slider Height
            var sliderH = new SliderInt(1, 32);
            sliderH.style.flexGrow = 1f;
            sliderH.style.height = 16;

            var labelH = new Label();
            sliderH.RegisterCallback<ChangeEvent<int>>(e => {
                labelH.text = "H: " + e.newValue.ToString();
                node.Room.Height = e.newValue;
                element.MarkDirtyRepaint();
            });
            sliderH.value = node.Room.Height;
            labelH.text = "H: " + node.Room.Height.ToString();
            var contH = new VisualElement();
            contH.Add(labelH);
            contH.Add(sliderH);
            contH.style.flexDirection = FlexDirection.Row;
            contH.style.height = 16;
            */

            nViews.Add(element);

            var mainCont = new VisualElement();
            mainCont.style.minHeight = 32f;
            mainCont.style.marginLeft = mainCont.style.marginRight = 10;
            //mainCont.Add(contW);
            //mainCont.Add(contH);
            element.Add(mainCont);
            view.AddElement(element);
        }

        foreach (var edge in graph.GetEdges())
        {
            var n1 = nViews.Find(v => v.Data.Equals(edge.FirstNode));
            var n2 = nViews.Find(v => v.Data.Equals(edge.SecondNode));
            var element = new LBSEdgeView<LBSEdge, RoomNode>(edge, n1, n2, 10, 3);
            view.AddElement(element);
            element.SendToBack();
        }
    }

    public override void Draw(LBSBehaviour behaviour, MainView view)
    {
        throw new System.NotImplementedException();
    }

    private void DrawArea(MeshGenerationContext mgc,Vector2 nodeSize,Vector2 tileSize,Vector2 pixelSize)
    {
        var painter = mgc.painter2D;
        var pos1 = -((tileSize * pixelSize) / 2f) + nodeSize / 2f;
        var pos2 = ((tileSize * pixelSize) / 2f) + nodeSize / 2f;

        painter.DrawDottedBox(pos1, pos2, color1, 4);
    }
}
