using ISILab.Commons.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Components
{
    [CreateAssetMenu(fileName = "NewID", menuName = "ISILab/LBS/PathOSTag")]
    [System.Serializable]
    public class PathOSTag : ScriptableObject
    {
        public enum PathOSCategory
        {
            ElementTag,
            EventTag
        }

        #region FIELDS
        [ReadOnly]
        public string label;
        [SerializeField]
        protected Texture2D icon;
        [SerializeField]
        protected Color color;
        [SerializeField]
        protected PathOSCategory category;
        #endregion

        #region PROPERTIES
        public string Label
        {
            get => label;
            set
            {
                if (label == value)
                    return;

                this.label = value;
                OnChangeText?.Invoke(this);
            }
        }

        public Texture2D Icon
        {
            get => icon;
            set
            {
                if (icon == value)
                    return;

                icon = value;
                OnChangeIcon?.Invoke(this);
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (color == value)
                    return;

                color = value;
                OnChangeColor?.Invoke(this);
            }
        }

        public PathOSCategory Category
        {
            get => category;
            set
            {
                if (category == value) return;
                category = value;
                OnChangeCategory?.Invoke(this);
            }
        }
        #endregion

        #region EVENTS
        public delegate void PathOSTagEvent(PathOSTag tag);
        public PathOSTagEvent OnChangeText;
        public PathOSTagEvent OnChangeColor;
        public PathOSTagEvent OnChangeIcon;
        public PathOSTagEvent OnChangeCategory;
        #endregion

        #region METHODS
        public void Init(string text, Color color, Texture2D icon)
        {
            this.label = text;
            this.color = color;
            this.icon = icon;
        }

        private void OnValidate()
        {
            label = this.name;
        }
        #endregion
    }
}
