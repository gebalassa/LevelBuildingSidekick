using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using System.Linq;

namespace LBS.VisualElements
{
    public class PathOSTagPallete : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<PathOSTagPallete, VisualElement.UxmlTraits> { }
        #endregion

        private PathOSOptionView[] optionViews;
        private object[] options;
        private object selected;
        private Action<PathOSOptionView, object> onSetView;

        #region FIELS VIEW
        private VisualElement content;
        private DropdownField dropdownGroup;
        private VisualElement icon;
        private Label nameLabel;
        private Button noElement;
        private Button addButton;
        private Button removeButton;
        #endregion

        #region EVENTS
        public event Action<ChangeEvent<string>> OnChangeGroup;
        public event Action<object> OnSelectOption;
        public event Action<object> OnRemoveOption;
        public event Action OnAddOption;
        public event Action OnRepaint;
        public event Func<object,string> OnSetTooltip;
        #endregion

        #region PROPERTIES
        public object Selected
        {
            get => selected;
            set => selected = value;
        }

        public object[] Options
        {
            get => options;
            set => options = value;
        }

        public bool ShowGroups
        {
            set => this.dropdownGroup.SetDisplay(value);
        }

        public bool ShowRemoveButton
        {
            set => this.removeButton.SetDisplay(value);
        }

        public bool ShowAddButton
        {
            set => this.addButton.SetDisplay(value);
        }
        #endregion


        #region CONSTRUCTORS
        public PathOSTagPallete()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTagPallete");
            visualTree.CloneTree(this);

            // Content
            this.content = this.Q<VisualElement>("Content");

            // Change Group
            this.dropdownGroup = this.Q<DropdownField>("DropdownGroup");
            dropdownGroup.RegisterCallback<ChangeEvent<string>>(evt => OnChangeGroup?.Invoke(evt));

            // NameLabel
            this.nameLabel = this.Q<Label>("NameLabel");

            // AddButton
            this.addButton = this.Q<Button>("AddButton");
            addButton.clicked += () => OnAddOption?.Invoke();

            // removeButton
            this.removeButton = this.Q<Button>("DeleteButton");
            removeButton.clicked += () => OnRemoveOption?.Invoke(selected);

            // NoElement
            this.noElement = this.Q<Button>("NoElement");

            // Icon
            this.icon = this.Q<VisualElement>("IconPallete");
        }
        #endregion

        #region METHODS
        private void OnInternalSelectOption(object obj)
        {

            foreach (var optV in optionViews)
            {
                optV.SetSelected(false);
            }
            selected = obj;
            OnSelectOption?.Invoke(obj);

        }

        public void SetOptions(object[] options, Action<PathOSOptionView, object> onSetView)
        {
            this.options = options;
            this.onSetView = onSetView;
        }

        public void SetIcon(Texture2D icon, Color color)
        {
            this.icon.style.backgroundImage = icon;
            this.icon.style.unityBackgroundImageTintColor = color;
        }

        public void SetName(string name)
        {
            this.nameLabel.text = name;
        }

        public void Repaint()
        {
            OnRepaint?.Invoke();

            content.Clear();

            if (options.Length > 0)
            {
                this.optionViews = new PathOSOptionView[options.Length];

                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    var view = new PathOSOptionView(option, OnInternalSelectOption, onSetView);
                    view.tooltip = OnSetTooltip?.Invoke(option);
                    optionViews[i] = view;
                    content.Add(view);
                }
            }
            else
            {
                content.Add(noElement);
            }

            if(selected != null)
            {
                var ov = optionViews.ToList().Find(o => o.target.Equals(selected));
                if(ov != null)
                    ov.SetSelected(true);
            }
        }
        #endregion
    }

}