using ISILab.Commons.Utility.Editor;
using LBS;
using LBS.Bundles;
using LBS.Settings;
using LBS.VisualElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Internal;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Editor;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("Exteiror Behaviour", typeof(ExteriorBehaviour))]
    public class ExteriorBehaviourEditor : LBSCustomEditor, IToolProvider
    {
        #region FIELDS
        private ExteriorBehaviour exterior;
        private Bundle targetBundle;

        private List<LBSIdentifierBundle> Groups;
        private object[] options;

        private AddEmptyTile addEmptyTile;
        private RemoveTileExterior removeTile;
        private SetExteriorTileConnection setConnection;
        private RemoveConnection removeConection;
        private RemoveConnectionInArea removeConnectionInArea;
        #endregion

        #region VIEW FIELDS
        private SimplePallete connectionPallete;
        private ObjectField bundleField;
        private WarningPanel warningPanel;
        #endregion

        #region PROPERTIES
        private Color BHcolor => LBSSettings.Instance.view.behavioursColor;
        #endregion

        #region CONSTRUCTORS
        public ExteriorBehaviourEditor(object target) : base(target)
        {
            // Set target Behaviour
            exterior = target as ExteriorBehaviour;

            // Get Target bundle
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

            targetBundle = bundles.Find(b => b.Name == exterior.TargetBundle);
            CreateVisualElement();
        }
        #endregion

        #region METHODS
        public override void SetInfo(object target)
        {
            exterior = target as ExteriorBehaviour;

            // Get Target bundle
            var bundles = LBSAssetsStorage.Instance.Get<Bundle>();

            targetBundle = bundles.Find(b => b.Name == exterior.TargetBundle);
        }

        public void SetTools(ToolKit toolKit)
        {
            Texture2D icon;

            // Set empty tile
            icon = Resources.Load<Texture2D>("Icons/Tools/Brush_interior_tile");
            addEmptyTile = new AddEmptyTile();
            var t1 = new LBSTool(icon, "Add tile wihtout connection", addEmptyTile);
            t1.Init(exterior.Owner, exterior);
            toolKit.AddTool(t1);

            // Remove tile
            icon = Resources.Load<Texture2D>("Icons/Tools/Delete_exterior_tile");
            removeTile = new RemoveTileExterior();
            var t2 = new LBSTool(icon, "Remove Tile", removeTile);
            t2.Init(exterior.Owner, exterior);
            toolKit.AddTool(t2);

            toolKit.AddSeparator(10);

            // Set connection
            icon = Resources.Load<Texture2D>("Icons/Tools/Exterior_connection");
            setConnection = new SetExteriorTileConnection();
            var t3 = new LBSTool(icon, "Set connection", setConnection);
            t3.OnSelect += () => LBSInspectorPanel.ShowInspector("Behaviours");
            t3.Init(exterior.Owner, exterior);
            toolKit.AddTool(t3);

            // Remove connection
            icon = Resources.Load<Texture2D>("Icons/Tools/Delete_exterior_connection");
            removeConection = new RemoveConnection();
            var t4 = new LBSTool(icon, "Remove connection", removeConection);
            t4.Init(exterior.Owner, exterior);
            toolKit.AddTool(t4);

        }

        private void OnTargetBundle() // FIX: set a better name
        {
            if (targetBundle == null)
            {
                warningPanel.SetDisplay(true);
                connectionPallete.SetDisplay(false);
            }
            else
            {
                warningPanel.SetDisplay(false);
                connectionPallete.SetDisplay(true);
                SetConnectionPallete(targetBundle);
            }
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ExteriorBehaviourEditor");
            visualTree.CloneTree(this);

            // WarningPanel
            warningPanel = this.Q<WarningPanel>();

            // BundleField
            bundleField = this.Q<ObjectField>("BundleField");
            bundleField.value = targetBundle;
            bundleField.RegisterValueChangedCallback(evt =>
            {
                targetBundle = evt.newValue as Bundle;
                exterior.TargetBundle = targetBundle.Name;
                OnTargetBundle();
                ToolKit.Instance.SetActive("Set connection");
            });

            // Connection Pallete
            connectionPallete = this.Q<SimplePallete>("ConnectionPallete");
            OnTargetBundle();

            return this;
        }

        private void SetConnectionPallete(Bundle bundle)
        {
            // Set init options
            connectionPallete.ShowGroups = false;
            connectionPallete.ShowAddButton = false;
            connectionPallete.ShowRemoveButton = false;

            // Get pallete icon 
            var icon = Resources.Load<Texture2D>("Icons/BrushIcon");

            // Set basic value
            connectionPallete.SetName("Connections");
            connectionPallete.SetIcon(icon, BHcolor);

            // Get odentifiers
            var indtifiers = LBSAssetsStorage.Instance.Get<LBSTag>();

            // Get current option
            var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
            var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
            var idents = tags.Select(s => indtifiers.Find(i => s == i.Label)).ToList().RemoveEmpties();

            // Set Options
            options = new object[idents.Count];
            for (int i = 0; i < idents.Count; i++)
            {
                if (idents[i] == null)
                    continue;

                options[i] = idents[i];
            }

            // Selected option event
            connectionPallete.OnSelectOption += (selected) =>
            {
                exterior.identifierToSet = selected as LBSTag;
                ToolKit.Instance.SetActive("Set connection");
            };

            // Init options
            connectionPallete.SetOptions(options, (optionView, option) =>
            {
                var identifier = option as LBSTag;
                optionView.Label = identifier.Label;
                optionView.Color = identifier.Color;
                optionView.Icon = identifier.Icon;
            });

            connectionPallete.OnRepaint += () => { connectionPallete.Selected = exterior.identifierToSet; };

            connectionPallete.Repaint();
        }
        #endregion
    }
}