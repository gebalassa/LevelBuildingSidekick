﻿using LBS.Graph;
using LBS.Representation.TileMap;
using System.Linq;
using System;
using Commons.Optimization.Evaluator;
using UnityEngine.UIElements;

public class WeighuedEvaluator : IEvaluator
{
    IEvaluator Adjacencies;
    IEvaluator Areas;
    IEvaluator Empty;

    public WeighuedEvaluator()
    {
        Adjacencies = new AdjacenciesEvaluator();
        Areas = new AreasEvaluator();
        Empty = new EmptySpaceEvaluator();
    }

    public VisualElement CIGUI()
    {
        throw new NotImplementedException();
    }

    public float Evaluate(IEvaluable evaluable)
    {
        throw new NotImplementedException();
    }

    public float EvaluateH<u>(IEvaluable schemaData, u graphData)
    {
        var evaluations = new Tuple<IEvaluator , float>[]
        {
                new Tuple<IEvaluator,float>(Adjacencies,0.5f),
                new Tuple<IEvaluator,float>(Areas,0.3f),
                new Tuple<IEvaluator,float>(Empty,0.2f)
            //new Tuple<Func<LBSGraphData, LBSSchemaData, float>, float>(EvaluateRoomDistribution,0.1f)
        };

        var value = 0f;
        for (int i = 0; i < evaluations.Count(); i++)
        {
            var action = evaluations[i].Item1;
            var weight = evaluations[i].Item2;
            value += (float)action.EvaluateH(graphData, schemaData) * weight;
        }

        return value;
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}