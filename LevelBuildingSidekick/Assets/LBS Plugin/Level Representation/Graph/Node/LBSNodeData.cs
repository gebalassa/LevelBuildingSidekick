using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;
using UnityEditor;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = ("LevelBuildingSidekick/Level Represetation/Graph Representation/Node"))]
    public class LBSNodeData : Data
    {
        public RoomCharacteristics room;
        public int x;
        public int y;
        public int radius;
        public string label = "";

        [JsonIgnore] 
        public Texture2D sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSNodeController);

        [JsonIgnore]
        public Rect Rect
        {
            get
            {
                return new Rect(new Vector2(x - radius, y - radius), Vector2.one * radius * 2);
            }
        }

        [JsonIgnore]
        public Func<string, bool> Exist { get; internal set; }

        public LBSNodeData()
        {

        }

        public LBSNodeData(string label, Vector2 position, int radius)
        {
            this.label = label;
            x = (int)position.x;
            y = (int)position.y;
            this.radius = radius;
        }

        public Vector2Int Centroid
        {
            get
            {
                return (Position + (Vector2Int.one * Radius));
            }
        }

        public Vector2Int Position
        {
            get
            {
                return new Vector2Int(x,y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }
        public void UnitySelect()
        {
            var s = ScriptableObject.CreateInstance<InspectorDrawer<LBSNodeData>>();
            s.data = this;
            Selection.SetActiveObjectWithContext(s, s);
        }
    }

    public class InspectorDrawer<T> : ScriptableObject  where T : Data
    {
        [SerializeField]
        internal LBSNodeData data;
    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

