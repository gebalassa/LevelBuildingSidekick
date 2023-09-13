using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("MAPElitesPresset", typeof(MAPElitesPresset))]
public class MAPElitesPressetVE : LBSCustomEditor
{
    Vector2IntField samples;

    ClassDropDown evaluatorX;
    Vector2Field thresholdX;
    VisualElement contentX;

    ClassDropDown evaluatorY;
    Vector2Field thresholdY;
    VisualElement contentY;

    ClassDropDown optimizer;
    VisualElement contentO;

    public MAPElitesPressetVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        var presset = target as MAPElitesPresset;
        samples.value = presset.SampleCount;

        if (presset.XEvaluator != null)
        {
            evaluatorX.value = presset.XEvaluator.GetType().Name;
            LoadEditor(contentX, presset.XEvaluator);
        }
        thresholdX.value = presset.XThreshold;


        if (presset.YEvaluator != null)
        {
            evaluatorY.value = presset.YEvaluator.GetType().Name;
            LoadEditor(contentY, presset.YEvaluator);
        }
        thresholdY.value = presset.YThreshold;

        if (presset.Optimizer != null)
        {
            optimizer.value = presset.Optimizer.GetType().Name;
            LoadEditor(contentO, presset.Optimizer);
        }
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var vt = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MAPElitesPresset");
        vt.CloneTree(ve);

        var presset = target as MAPElitesPresset;

        samples = ve.Q<Vector2IntField>(name: "Samples");
        samples.RegisterValueChangedCallback(
             evt =>
             {
                 presset.SampleCount = evt.newValue;
             });

        evaluatorX = ve.Q<ClassDropDown>(name: "XDropdown");
        thresholdX = ve.Q<Vector2Field>(name: "XThreshold");
        thresholdX.RegisterValueChangedCallback(
             evt =>
             {
                 presset.XThreshold = evt.newValue;
             });
        contentX = ve.Q<VisualElement>(name: "XContent");

        evaluatorY = ve.Q<ClassDropDown>(name: "YDropdown");
        thresholdY = ve.Q<Vector2Field>(name: "YThreshold");
        thresholdY.RegisterValueChangedCallback(
             evt =>
             {
                 presset.YThreshold = evt.newValue;
             });
        contentY = ve.Q<VisualElement>(name: "YContent");

        optimizer = ve.Q<ClassDropDown>(name: "ODropdown");
        contentO = ve.Q<VisualElement>(name: "OContent");

        evaluatorX.Type = typeof(IRangedEvaluator);
        evaluatorY.Type = typeof(IRangedEvaluator);
        optimizer.Type = typeof(BaseOptimizer);

        evaluatorX.RegisterValueChangedCallback(
            evt => 
            {
                var obj = evaluatorX.GetChoiceInstance();
                presset.XEvaluator = obj as IRangedEvaluator;
                LoadEditor(contentX, obj);
            });

        evaluatorY.RegisterValueChangedCallback(
            evt =>
            {
                var obj = evaluatorY.GetChoiceInstance();
                presset.YEvaluator = obj as IRangedEvaluator;
                LoadEditor(contentY, obj);
            });

        optimizer.RegisterValueChangedCallback(
            evt =>
            {
                var obj = optimizer.GetChoiceInstance();
                presset.Optimizer = obj as BaseOptimizer;
                LoadEditor(contentO, obj);
            });


        InitialValues();


        return ve;
    }

    private void InitialValues()
    {
        

    }


    private void LoadEditor(VisualElement container, object target)
    {
        container.Clear();

        var prosp = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>();

        if (prosp.Count <= 0)
        {
            return;
        }

        var ves = prosp.Where(t => t.Item2.Any(v => v.type == target.GetType()));

        if(ves.Count() <= 0)
        {
            return;
        }

        var ve = Activator.CreateInstance(ves.First().Item1, new object [] {target}) as VisualElement;

        container.Add(ve);
    }


    public void Print()
    {

    }

}
