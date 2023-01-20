using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LBS;
using System;
using LBS.Schema;
using Newtonsoft.Json;
using UnityEditor;

namespace LBS.Components.Graph
{
    [System.Serializable]
    public abstract class LBSNode : ICloneable
    {
        //FIELDS
        [HideInInspector, SerializeField, JsonRequired, SerializeReference]
        private int x, y;

        /// <summary>
        /// Width and Height for node size in window. Porportional to window defined Unit size.
        /// </summary>
        [HideInInspector, SerializeField, JsonRequired, SerializeReference]
        private float width, height;

        [HideInInspector, SerializeField, JsonRequired, SerializeReference]
        private string id = ""; // "ID" or "name"

        //PROPERTIES
        [JsonIgnore]
        public Vector2 Centroid => (Position + (new Vector2(width, height)/2));

        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);

            set
            {
                x = value.x;
                y = value.y;
            }
        }

        [JsonIgnore]
        public float Width
        {
            get => width;
            set => width = value;
        }

        [JsonIgnore]
        public float Height
        {
            get => height;
            set => height = value;
        }

        [JsonIgnore]
        public string ID
        {
            get => id;
            set
            {
                id = value;
                OnChange?.Invoke(this);
            }
        }

        [HideInInspector, JsonIgnore]
        internal Action<LBSNode> OnChange; //  (!!!) implementar


        /// <summary>
        /// Empty constructor, necessary for serialization with json.
        /// </summary>
        public LBSNode() 
        {
            id = "Undefined";
            width = 1;
            height = 1;
            x = 0;
            y = 0;
        }

        /// <summary>
        /// Constructor of the LBSNodeData class, which creates a new node data with 
        /// the given label and position and dimensions 1x1.
        /// </summary>
        /// <param name="label"> Label of the node. </param>
        /// <param name="position"> Position if the node. </param>
        public LBSNode(string label, Vector2 position)
        {
            this.id = label;
            x = (int)position.x;
            y = (int)position.y;
            width = 1;
            height = 1;
        }

        public abstract object Clone();
    }
}

