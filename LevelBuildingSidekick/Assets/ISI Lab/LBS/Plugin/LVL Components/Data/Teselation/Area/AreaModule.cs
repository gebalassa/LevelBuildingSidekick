using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LBS.Components.Teselation
{
    [System.Serializable]
    public class AreaModule<T> : TeselationModule where T : Area
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        List<Area> areas;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public int AreaCount => areas.Count;

        #endregion

        #region CONSTRUCTOR

        public AreaModule() : base() { Key = GetType().Name; }

        public AreaModule(List<T> areas, string key) : base(key)
        {
            this.areas = new List<Area>(areas);
        }

        #endregion

        #region METHODS

        public bool AddArea(T area)
        {
            if (areas.Contains(area))
                return false;
            areas.Add(area);
            OnChanged?.Invoke(this);
            return true;
        }

        public T GetArea(int index)
        {
            if (areas.ContainsIndex(index))
                return areas[index] as T;
            return null;
        }

        public T GetArea(string id)
        {
            return areas.Find(a => a.ID.Equals(id)) as T;
        }

        public bool Remove(T area)
        {
            if(areas.Remove(area))
            {
                OnChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        public T RemoveAt(int index)
        {
            if (!areas.ContainsIndex(index))
                return null;
            var a = areas[index];
            areas.Remove(a);
            OnChanged?.Invoke(this);
            return a as T;
        }

        public bool ContainsPoint(Vector2 point)
        {
            foreach(T a in areas)
            {
                if (a.ContainsPoint(point))
                    return true;
            }
            return false;
        }

        public override void Clear()
        {
            areas.Clear();
        }

        public override object Clone()
        {
            var area = new AreaModule<T>();
            area.areas = areas.Select(a => a.Clone() as Area).ToList(); //new List<Area>(areas);
            return area;
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}

