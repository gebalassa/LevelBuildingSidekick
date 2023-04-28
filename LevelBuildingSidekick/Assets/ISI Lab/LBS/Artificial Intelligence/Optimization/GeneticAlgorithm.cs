﻿using Commons.Optimization;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GeneticAlgorithm : BaseOptimizer
{
    #region FIELDS

    [SerializeField, SerializeReference]
    ICrossover crossover;
    [SerializeField, SerializeReference]
    IMutation mutation;

    /// <summary>
    /// The default crossover probability.
    /// </summary>
    [SerializeField, SerializeReference]
    public float crossoverProbability = 0.75f;

    /// <summary>
    /// The default mutation probability.
    /// </summary>
    [SerializeField, SerializeReference]
    public float mutationProbability = 0.25f;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Gets or sets the task executor which will be used to execute fitness evaluation.
    /// </summary>
    public ITaskExecutor TaskExecutor { get; set; }

    /// <summary>
    /// Gets or sets the reinsertion operator.
    /// </summary>
    public IReinsertion Reinsertion { get; set; }

    /// <summary>
    /// Gets the operators strategy
    /// </summary>
    public IOperatorsStrategy OperatorsStrategy { get; set; }

    /// <summary>
    /// Gets or sets the crossover operator.
    /// </summary>
    /// <value>The crossover.</value>
    public ICrossover Crossover
    {
        get => crossover;
        set => crossover = value;
    }

    /// <summary>
    /// Gets or sets the mutation operator.
    /// </summary>
    public IMutation Mutation
    {
        get => mutation;
        set => mutation = value;
    }

    /// <summary>
    /// Gets or sets the crossover probability.
    /// </summary>
    public float CrossoverProbability
    {
        get => crossoverProbability;
        set => crossoverProbability = value;
    }

    /// <summary>
    /// Gets or sets the mutation probability.
    /// </summary>
    public float MutationProbability { get; set; }

    public new IOptimizable Adam
    {
        get => adam;
        set
        {
            adam = value;
            Population.Adam = value;
        }
    }

    #endregion

    public GeneticAlgorithm()
    {
        Reinsertion = new ElitistReinsertion();
        State = Op_State.NotStarted;
        TaskExecutor = new LinearTaskExecutor();
        OperatorsStrategy = new DefaultOperatorsStrategy();

        Selection = new RankSelection();
        Crossover = new AreaCrossover();
        //Crossover = new UniformCrossover();

        Mutation = new RoulleteWheelMutation(new List<Tuple<MutationBase, float>>()
        { 
            new Tuple<MutationBase, float>(new ExhaustiveAddGene(), 25),
            new Tuple<MutationBase, float>(new ExhaustiveRemoveGene(), 25),
            new Tuple<MutationBase, float>(new ExhaustiveSwapGene(), 25),
            new Tuple<MutationBase, float>(new ExhaustiveRanged2DSwap(5), 25)
        });

        Population = new Population();
        Termination = new GenerationNumberTermination();
        Evaluator = null; //(!!!) FILL UP With new wvaluator
    }

    public override void RunOnce()
    {
        var parents = SelectParents();
        var p = parents.Select(p => p as ChromosomeBase).ToList();
        var offspring = Cross(p);
        Mutate(offspring);
        var children = offspring.Select(p => p as IOptimizable).ToList();
        EvaluateFitness(children);
        var newGenerationChromosomes = Reinsert(children, parents);
        Population.CreateNewGeneration(newGenerationChromosomes);
        EndCurrentGeneration();
    }

    /// <summary>
    /// Reinsert the specified offspring and parents.
    /// </summary>
    /// <param name="offspring">The offspring chromosomes.</param>
    /// <param name="parents">The parents chromosomes.</param>
    /// <returns>
    /// The reinserted chromosomes.
    /// </returns>
    private IList<IOptimizable> Reinsert(IList<IOptimizable> offspring, IList<IOptimizable> parents)
    {
        return Reinsertion.SelectChromosomes(Population, offspring, parents);
    }


    /// <summary>
    /// Ends the current generation.
    /// </summary>
    /// <returns><c>true</c>, if current generation was ended, <c>false</c> otherwise.</returns>
    private bool EndCurrentGeneration()
    {
        Population.EndCurrentGeneration();

        OnGenerationRan?.Invoke();
        
        if (Termination.HasReached(this))
        {
            State = Op_State.TerminationReached;
            OnTerminationReached?.Invoke();
            return true;
        }

        if (stopRequested)
        {
            TaskExecutor.Stop();
            State = Op_State.Stopped;
        }
        

        return false;
    }

    /// <summary>
    /// Crosses the specified parents.
    /// </summary>
    /// <param name="parents">The parents.</param>
    /// <returns>The result chromosomes.</returns>
    private IList<ChromosomeBase> Cross(IList<ChromosomeBase> parents)
    {
        return OperatorsStrategy.Cross(Population, Crossover, CrossoverProbability, parents);
    }

    /// <summary>
    /// Mutate the specified chromosomes.
    /// </summary>
    /// <param name="chromosomes">The chromosomes.</param>
    private void Mutate(IList<ChromosomeBase> chromosomes)
    {
        OperatorsStrategy.Mutate(Mutation, MutationProbability, chromosomes);
    }

    /// <summary>
    /// Selects the parents.
    /// </summary>
    /// <returns>The parents.</returns>
    private IList<IOptimizable> SelectParents()
    {
        return Selection.SelectEvaluables(Population.MinSize, Population.CurrentGeneration);
    }

    /// <summary>
    /// Evaluates the fitness.
    /// </summary>
    public override void EvaluateFitness(IList<IOptimizable> optimizables)
    {
        try
        {

            for (int i = 0; i < optimizables.Count; i++)
            {
                var c = optimizables[i];

                TaskExecutor.Add(() =>
                {
                    RunEvaluateFitness(c);
                });
            }

            if (!TaskExecutor.Start())
            {
                throw new TimeoutException("The fitness evaluation reached the {0} timeout.".With(TaskExecutor.Timeout));
            }
        }
        finally
        {
            TaskExecutor.Stop();
            TaskExecutor.Clear();
        }

        optimizables = optimizables.OrderByDescending(c => c.Fitness).ToList();
    }

    /// <summary>
    /// Runs the evaluate fitness.
    /// </summary>
    /// <param name="chromosome">The chromosome.</param>
    private void RunEvaluateFitness(IOptimizable chromosome)
    {
        chromosome.Fitness = Evaluator.Evaluate(chromosome);
    }
}