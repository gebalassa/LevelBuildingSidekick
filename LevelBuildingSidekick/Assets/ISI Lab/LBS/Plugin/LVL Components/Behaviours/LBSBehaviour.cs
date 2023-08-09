using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LBS.Behaviours
{
    [System.Serializable]
    public abstract class LBSBehaviour : ICloneable
    {
        #region FIELDS
        [NonSerialized, HideInInspector, JsonIgnore]
        protected LBSLayer owner;
        [SerializeField]
        private Texture2D icon;
        [SerializeField]
        private string name;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public LBSLayer Owner
        {
            get => owner;
            set => owner = value;
        }

        [JsonIgnore]
        public Texture2D Icon
        {
            get => icon;
        }

        [JsonIgnore]
        public string Name
        {
            get => name;
        }
        #endregion

        #region EVENTS

        #endregion

        #region CONSTRUCTORS
        public LBSBehaviour(Texture2D icon, string name)
        {
            this.icon = icon;
            this.name = name;
        }
        #endregion

        #region METHODS
        public virtual void Init(LBSLayer layer)
        {
            owner = layer;
        }

        public List<Type> GetRequieredModules()
        {
            var toR = new List<Type>();
            Type tipo = this.GetType();

            object[] atts = tipo.GetCustomAttributes(true);

            foreach (var att in atts)
            {
                if (att is RequieredModuleAttribute)
                {
                    toR.AddRange((att as RequieredModuleAttribute).types);
                }
            }
            return toR;
        }

        public abstract object Clone();
        #endregion
    }
}