using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    public interface ISelectable
    {
        public List<object> GetSelected(Vector2Int position);
    }

    [System.Serializable]
    public abstract class LBSModule : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        protected string id;

        //[SerializeField, JsonRequired]
        //protected bool changed;

        [JsonIgnore, HideInInspector]
        private LBSLayer owner;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public LBSLayer Owner
        {
            get => owner;
            set => owner = value;
        }

        [JsonIgnore]
        public string ID
        {
            get => id;
            set => id = value;
        }

        #endregion

        #region EVENTS
        [JsonIgnore]
        public Action<LBSModule, List<object>, List<object>> OnChanged;
        #endregion

        #region CONSTRUCTOR
        public LBSModule() 
        { 
            ID = GetType().Name; 
        }

        public LBSModule(string key) 
        { 
            ID = key; 
        }
        #endregion

        #region METHODS
        public virtual void OnAttach(LBSLayer layer)
        {
            Owner = layer;
        }

        public virtual void OnDetach(LBSLayer layer)
        {
            Owner = null;
        }

        public virtual void Reload(LBSLayer layer)
        {
            Owner = layer;
        }
        #endregion

        #region ABSTRACT METHODS
        /// <summary>
        /// prints by console basic information of 
        /// the representation.
        /// </summary>
        public abstract void Print(); // (?) sto es equivalente a "toString()" ?  

        /// <summary>
        /// Cleans all the information saved in.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Determines whether the representation is empty.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsEmpty();

        /// <summary>
        /// Creates a copy of the representation.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Gets the bounding rectangle of the representation.
        /// </summary>
        /// <returns></returns>
        public abstract Rect GetBounds(); // (?) meter esto a una interfaz IBoundeble ??

        /// <summary>
        /// Rewrites the representation based on another LBSModule.
        /// </summary>
        /// <param name="other"></param>
        public abstract void Rewrite(LBSModule other);
        #endregion
    }
}

