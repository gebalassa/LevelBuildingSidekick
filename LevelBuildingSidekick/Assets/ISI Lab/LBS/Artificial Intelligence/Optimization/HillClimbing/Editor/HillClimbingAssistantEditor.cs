using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
using LBS;
using LBS.Components;
using LBS.Settings;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [LBSCustomEditor("HillClimbingAssistant", typeof(HillClimbingAssistant))]
    public class HillClimbingAssistantEditor : LBSCustomEditor, IToolProvider
    {
        private readonly UnityEngine.Color AssColor = LBSSettings.Instance.view.assitantsColor;

        private HillClimbingAssistant hillClimbing;

        private Foldout foldout;
        private Button revert;
        private Button execute;
        private Toggle toggle;
        private Button executeOne;
        private Toggle toggleTimer;

        private Button recalculate;

        private LBSLayer tempLayer;

        // Manipulators
        private SetZoneConnection setZoneConnection;
        private RemoveAreaConnection removeAreaConnection;

        public HillClimbingAssistantEditor(object target) : base(target)
        {
            hillClimbing = target as HillClimbingAssistant;

            CreateVisualElement();

            var wnd = EditorWindow.GetWindow<LBSMainWindow>();

            hillClimbing.OnTermination += wnd.Repaint;
        }

        public override void Repaint()
        {
            var moduleConstr = hillClimbing.Owner.GetModule<ConstrainsZonesModule>();
            foldout.Clear();
            foreach (var constraint in moduleConstr.Constraints)
            {
                var view = new ConstraintView();
                view.SetData(constraint);
                foldout.Add(view);
            }
        }

        public override void SetInfo(object target)
        {
            hillClimbing = target as HillClimbingAssistant;
        }

        public void SetTools(ToolKit toolKit)
        {
            Texture2D icon;

            toolKit.AddSeparator();

            // Add Zone connection
            icon = Resources.Load<Texture2D>("Icons/Tools/Node_connection");
            setZoneConnection = new SetZoneConnection();
            var t1 = new LBSTool(icon, "Add zone connection", setZoneConnection);
            t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Assistants");
            t1.Init(hillClimbing.Owner, hillClimbing);
            toolKit.AddTool(t1);

            // Remove zone connections
            icon = Resources.Load<Texture2D>("Icons/Tools/Delete_node_connection");
            removeAreaConnection = new RemoveAreaConnection();
            var t2 = new LBSTool(icon, "Remove zone connection", removeAreaConnection);
            t2.Init(hillClimbing.Owner, hillClimbing);
            toolKit.AddTool(t2);
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("HillClimbingEditor");
            visualTree.CloneTree(this);

            var moduleConstr = hillClimbing.Owner.GetModule<ConstrainsZonesModule>();

            // Foldout
            foldout = this.Q<Foldout>();
            foreach (var constraint in moduleConstr.Constraints)
            {
                var view = new ConstraintView();
                view.SetData(constraint);
                foldout.Add(view);
            }

            // Print Timers
            toggleTimer = this.Q<Toggle>("ShowTimerToggle");
            toggleTimer.RegisterCallback<ChangeEvent<bool>>(x =>
            {
                hillClimbing.printClocks = x.newValue;
            });

            // Revert
            revert = this.Q<Button>("Revert");
            revert.clicked += Revert;

            // Execute
            execute = this.Q<Button>("Execute");
            execute.clicked += Execute;

            // Execute 1 step
            executeOne = this.Q<Button>("ExecuteOneStep");
            executeOne.clicked += ExecuteOneStep;

            // Show Constraint
            toggle = this.Q<Toggle>("ShowConstraintToggle");
            toggle.value = hillClimbing.visibleConstraints;
            toggle.RegisterCallback<ChangeEvent<bool>>(x =>
            {
                hillClimbing.visibleConstraints = x.newValue;
                DrawManager.ReDraw();
            });

            recalculate = new Button();
            recalculate.text = "Recalculate Constraints";
            recalculate.clicked += () =>
            {
                hillClimbing.RecalculateConstraint();
                DrawManager.ReDraw();
                Paint();
            };

            Add(recalculate);

            return this;
        }

        private void Paint()
        {
            Clear();
            CreateVisualElement();
        }

        private void ExecuteOneStep()
        {
            hillClimbing.ExecuteOneStep();
        }

        private void Execute()
        {
            hillClimbing.Execute();
        }

        private void Revert()
        {
            if (tempLayer == null)
            {
                Debug.Log("No existe version para revertir.");
                return;
            }

            var lvl = hillClimbing.Owner.Parent;
            lvl.ReplaceLayer(hillClimbing.Owner, tempLayer);
        }
    }
}