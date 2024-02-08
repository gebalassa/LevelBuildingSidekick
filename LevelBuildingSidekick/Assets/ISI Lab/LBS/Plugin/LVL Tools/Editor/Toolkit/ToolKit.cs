using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using System.Speech.Recognition;
using static UnityEngine.GraphicsBuffer;
using LBS.Settings;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;

namespace LBS.VisualElements
{
    public class ToolKit : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ToolKit, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlColorAttributeDescription m_BaseColor = new UxmlColorAttributeDescription
            {
                name = "base-color",
                defaultValue = new Color(72f / 255f, 72f / 255f, 72f / 255f)
            };

            UxmlColorAttributeDescription m_SelectedColor = new UxmlColorAttributeDescription
            {
                name = "selected-color",
                defaultValue = new Color(215f / 255f, 127f / 255f, 45f / 255f)
            };

            UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
            {
                name = "index",
                defaultValue = 0
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var btn = ve as ToolKit;

                btn.BaseColor = m_BaseColor.GetValueFromBag(bag, cc);
                btn.Index = m_Index.GetValueFromBag(bag, cc);
            }
        }
        #endregion

        #region SINGLETON
        private static ToolKit instance;
        internal static ToolKit Instance 
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region FIELDS
        private List<(LBSTool,ToolButton)> tools = new();
        private (LBSTool,ToolButton) current;

        private Color baseColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private int index = 0;
        private int choiceCount = 0;
        #endregion

        #region FIELDS VIEW
        private VisualElement content;
        #endregion

        #region PROPERTIES
        public Color BaseColor
        {
            get => baseColor;
            set => baseColor = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }

        public int ChoiceCount
        {
            get => choiceCount;
            set => choiceCount = value;
        }
        #endregion

        #region EVENTS
        public event Action<LBSLayer> OnEndAction;
        public event Action<LBSLayer> OnStartAction;
        #endregion

        #region CONSTRUCTORS
        public ToolKit()
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolKit");
            visualTree.CloneTree(this);

            this.content = this.Q<VisualElement>("Content");

            // Singleton
            if (instance != this)
                instance = this;
        }
        #endregion

        #region METHODS
        public void Init(LBSLayer layer)
        {
            InitGeneralTools(layer);
            this.AddSeparator();

            InitBehavioursTools(layer);
            this.AddSeparator();

            InitAssistantsTools(layer);
        }

        private void InitGeneralTools(LBSLayer layer)
        {
            var icon = Resources.Load<Texture2D>("Icons/Select");
            var selectTool = new Select();
            var t1 = new LBSTool(icon, "Select", selectTool);
            t1.Init(layer, this);
            t1.OnSelect += () =>
            {
                LBSInspectorPanel.ShowInspector("Current data");
            };
            this.AddTool(t1);
        }

        private void InitBehavioursTools(LBSLayer layer)
        {

            foreach (var behaviour in layer.Behaviours)
            {
                var type = behaviour.GetType();
                var customEditors = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (customEditors.Count() == 0)
                    return;

                var customEditor = customEditors.First().Item1;
                var i = customEditor.GetInterface(typeof(IToolProvider).Name);

                if (i != null)
                {
                    var ve = LBSInspectorPanel.Instance.behaviours.CustomEditors.First( x => x.GetType() == customEditor);
                    ve.SetInfo(behaviour);
                    ((IToolProvider)ve).SetTools(this);
                }
            }
        }

        private void InitAssistantsTools(LBSLayer layer)
        {
            foreach (var assist in layer.Assitants)
            {
                var type = assist.GetType();
                var customEditors = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (customEditors.Count() == 0)
                    return;

                var customEditor = customEditors.First().Item1;
                var i = customEditor.GetInterface(typeof(IToolProvider).Name);

                if (i != null)
                {
                    var ve = LBSInspectorPanel.Instance.assistants.CustomEditors.First(x => x.GetType() == customEditor);
                    ve.SetInfo(assist);
                    ((IToolProvider)ve).SetTools(this);
                }
            }
        }

        public void SetActive(int index)
        {
            this.index = index;

            if(current != default((LBSTool,ToolButton)))
                current.Item2.OnBlur();
            
            current = tools[index];

            current.Item2.OnFocus();

            var m = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(m);
        }

        public void SetActiveWhithoutNotify(int index)
        {
            this.index = index;
            current = tools[index];
            current.Item2.OnFocusWithoutNotify();

            var m = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(m);
        }

        public void SetActive(string value)
        {
            var index = tools.FindIndex(t => t.Item2.tooltip.Equals(value));
            SetActive(index);
        }

        public void SetActive()
        {
            if (tools.Count <= 0)
                return;
        }

        public void AddSubTools(LBSTool[] tool, int index = -1)
        {
            throw new NotImplementedException();
        }

        public void AddTool(LBSTool tool, int index = -1)
        {
            var button = new ToolButton(tool);
            (LBSTool, ToolButton) t = new(tool, button);
            tool.BindButton(button);

            this.content.Add(button);
            tools.Add(t);

            var i = tools.Count - 1;
            button.AddGroupEvent(() =>
            {
                var index = i;
                SetActive(index);
            });
            button.SetColorGroup(baseColor, LBSSettings.Instance.view.toolkitSelected);

            tool.OnStart += (l) => { OnStartAction?.Invoke(l); };
            tool.OnEnd += (l) => { OnEndAction?.Invoke(l); };

        }

        public void AddSeparator(int height = 10)
        {
            var separator = new VisualElement();
            separator.style.height = height;
            this.content.Add(separator);
        }

        public new void Clear()
        {
            if (tools.Count <= 0)
                return;

            current.Item2?.OnBlur();
            tools.Clear();
            this.content.Clear();
        }
        #endregion
    }
}
