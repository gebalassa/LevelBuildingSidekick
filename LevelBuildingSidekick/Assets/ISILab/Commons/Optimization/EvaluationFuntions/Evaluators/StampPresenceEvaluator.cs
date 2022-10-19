using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

[System.Serializable]
public class StampPresenceEvaluator : IRangedEvaluator
{
    float min = 0;
    float max = 1;
    public float MaxValue => max;

    public float MinValue => min;

    public StampPresset stamp;

    public StampPresenceEvaluator()
    {
        this.stamp = null;
    }

    public StampPresenceEvaluator(StampPresset stamp)
    {
        this.stamp = stamp;
    }

    public float Evaluate(IEvaluable evaluable)
    {
        int presence = 0;

        if (!(evaluable is StampTileMapChromosome))
        {
            return MinValue;
        }

        var stmc = evaluable as StampTileMapChromosome;

        if(!stmc.stamps.Any(s => s.Label == stamp.Label))
        {
            return MinValue;
        }

        var index = stmc.stamps.FindIndex(s => s.Label == stamp.Label);

        var data = stmc.GetDataSquence<int>();
        foreach (var i in data)
        {
            if(index == i)
            {
                presence++;
            }
        }
        return Mathf.Clamp(presence,MinValue,MaxValue);
    }

    public string GetName()
    {
        return "Stamp Presence";
    }

    public VisualElement CIGUI()
    {
        var content = new VisualElement();

        var v2 = new Vector2Field("Fitness threshold");
        v2.value = new Vector2(this.MinValue, this.MaxValue);
        v2.RegisterValueChangedCallback(e => {
            min = e.newValue.x;
            max = e.newValue.y;
        });

        ObjectField of = new ObjectField("Stamp: ");
        of.objectType = typeof(StampPresset);
        of.value = stamp;
        of.RegisterCallback<ChangeEvent<StampPresset>>((e) => stamp = e.newValue);

        content.Add(v2);
        content.Add(of);

        return content;
    }
}
