using LBS;
using LBS.Behaviours;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace LBS
{
    public class LBSTool
    {
        #region FIELDS
        private Texture2D icon;
        private string name;
        private LBSManipulator manipulator;
        #endregion

        #region PROPERTIES
        public Texture2D Icon => icon;
        public string Name => name;
        public LBSManipulator Manipulator => manipulator;
        #endregion

        #region EVENTS
        public event Action OnSelect;
        public event Action OnDeselect;

        public event Action<LBSLayer> OnStart;
        public event Action<LBSLayer> OnPressed;
        public event Action<LBSLayer> OnEnd;
        #endregion

        #region CONSTRUCTORS
        public LBSTool(Texture2D icon, string name, LBSManipulator manipulator,
            Feedback feedback = null,
            Action OnStart = null,
            Action OnUpdate = null,
            Action OnEnd = null) // (!) el feedback y los eventos no se estan usando 
        {
            this.icon = icon;
            this.name = name;
            this.manipulator = manipulator;
        }
        #endregion

        #region METHODS
        public virtual void Init(LBSLayer layer, object behaviour)
        {
            manipulator.OnManipulationStart += () => { OnStart?.Invoke(layer); };
            manipulator.OnManipulationUpdate += () => { OnPressed?.Invoke(layer); };
            manipulator.OnManipulationEnd += () => { OnEnd?.Invoke(layer); };

            manipulator.Init(layer, behaviour);
        }

        public void BindButton(ToolButton button)
        {
            var canvas = MainView.Instance;

            button.OnFocusEvent += () =>
            {
                canvas.AddManipulator(this.manipulator);
                OnSelect?.Invoke();
            };
            button.OnBlurEvent += () =>
            {
                canvas.RemoveManipulator(this.manipulator);
                OnDeselect?.Invoke();
            };
        }
        #endregion;
    }
}