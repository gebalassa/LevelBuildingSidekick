using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Assisstants
{
    [System.Serializable]
    public abstract class LBSAssistantAI : ICloneable
    {
        #region FIELDS
        [NonSerialized, HideInInspector, JsonIgnore]
        private LBSLayer owner;
        [NonSerialized, HideInInspector, JsonIgnore]//[SerializeField]
        public Texture2D icon;
        [SerializeField]
        public string name;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public LBSLayer Owner
        {
            get => owner;
            set => owner = value;
        }
        #endregion

        #region EVENTS
        [JsonIgnore]
        public Action OnStart;
        [JsonIgnore]
        public Action OnTermination;
        #endregion

        #region CONSTRUCTORS
        public LBSAssistantAI() { }
        #endregion

        #region METHODS
        public abstract void OnAdd(LBSLayer layer);

        public abstract void Execute();

        public abstract object Clone();

        public List<Type> GetRequieredModules()
        {
            var toR = new List<Type>();
            Type type = this.GetType();

            object[] atts = type.GetCustomAttributes(true);

            foreach (var att in atts)
            {
                if (att is RequieredModuleAttribute)
                {
                    toR.AddRange((att as RequieredModuleAttribute).types);
                }
            }
            return toR;
        }
        #endregion
    }
}
