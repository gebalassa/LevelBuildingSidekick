using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.Commons.VisualElements
{
    public class ConnectionFeedback : GraphElement
    {
        private Color color = Color.white;
        private Vector2Int pos1 = new Vector2Int();
        private Vector2Int pos2 = new Vector2Int();

        public ConnectionFeedback()
        {
            focusable = false;
            SetPosition(new Rect(Vector2.zero, new Vector2(10, 10)));
            generateVisualContent += OnGenerateVisualContent;
        }

        public void Actualize(Color color, Vector2Int pos1, Vector2Int pos2)
        {
            this.color = color;
            this.pos1 = pos1;
            this.pos2 = pos2;
            MarkDirtyRepaint();
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;

            var fPos1 = Vector2.zero;
            var fPos2 = pos2 - pos1;
            painter.DrawLine(pos1, pos2, color, 3);
            painter.DrawCircle(pos1, 10f, color);
            painter.DrawCircle(pos2, 10f, color);
        }
    }
}