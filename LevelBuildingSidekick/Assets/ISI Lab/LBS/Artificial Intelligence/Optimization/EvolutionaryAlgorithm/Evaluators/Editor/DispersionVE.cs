using Commons.Optimization.Evaluator;
using ISILab.LBS.AI.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DispersionVE : EvaluatorVE
{
    IntegerField intField;

    public DispersionVE(IEvaluator evaluator) : base(evaluator)
    {
        intField = new IntegerField("Cluster Count");

        intField.value = (evaluator as Dispersion).ClusterCount;

        intField.RegisterValueChangedCallback(e => (evaluator as Dispersion).ClusterCount = e.newValue);

        this.Add(intField);
    }

    public override void Init()
    {
    }
}
