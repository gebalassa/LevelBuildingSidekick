using Commons.Optimization;
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
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class GeneticAlgorithm : IGeneticAlgorithm , IShowable
{
    #region Constants
    /// <summary>
    /// The default crossover probability.
    /// </summary>
    public const float DefaultCrossoverProbability = 0.75f;

    /// <summary>
    /// The default mutation probability.
    /// </summary>
    public const float DefaultMutationProbability = 0.1f;
    #endregion

    #region Fields
    private bool m_stopRequested;
    private readonly object m_lock = new object();
    private OptimizerState m_state;
    private Stopwatch m_stopwatch;

    [SerializeField, SerializeReference]
    IPopulation population;
    [SerializeField, SerializeReference]
    IEvaluator fitness;
    [SerializeField, SerializeReference]
    ISelection selection;
    [SerializeField, SerializeReference]
    ICrossover crossover;
    [SerializeField, SerializeReference]
    IMutation mutation;
    #endregion

    #region Events
    public Action OnGenerationRan { get; set; }
    public Action OnTerminationReached { get; set; }
    public Action OnStopped { get; set; }
    public Action OnResumed { get; set; }
    public Action OnPaused { get; set; }
    public Action OnStarted { get; set; }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the operators strategy
    /// </summary>
    public IOperatorsStrategy OperatorsStrategy { get; set; }

    public IPopulation Population { get; private set; }

    public IEvaluable[] LastGeneration
    {
        get
        {
            return Population.CurrentGeneration.Evaluables.ToArray();
        }
    }

    /// <summary>
    /// Gets the fitness function.
    /// </summary>
    public IEvaluator Evaluator { get; private set; }

    /// <summary>
    /// Gets or sets the selection operator.
    /// </summary>
    public ISelection Selection { get; set; }

    /// <summary>
    /// Gets or sets the crossover operator.
    /// </summary>
    /// <value>The crossover.</value>
    public ICrossover Crossover { get; set; }

    /// <summary>
    /// Gets or sets the crossover probability.
    /// </summary>
    public float CrossoverProbability { get; set; }

    /// <summary>
    /// Gets or sets the mutation operator.
    /// </summary>
    public IMutation Mutation { get; set; }

    /// <summary>
    /// Gets or sets the mutation probability.
    /// </summary>
    public float MutationProbability { get; set; }

    /// <summary>
    /// Gets or sets the reinsertion operator.
    /// </summary>
    public IReinsertion Reinsertion { get; set; }

    /// <summary>
    /// Gets or sets the termination condition.
    /// </summary>
    public ITermination Termination { get; set; }

    /// <summary>
    /// Gets the generations number.
    /// </summary>
    /// <value>The generations number.</value>
    public int GenerationsNumber
    {
        get
        {
            return Population.GenerationsNumber;
        }
    }

    /// <summary>
    /// Gets the best chromosome.
    /// </summary>
    /// <value>The best chromosome.</value>
    public IChromosome BestChromosome
    {
        get
        {
            return Population.BestCandidate as IChromosome;
        }
    }

    public IEvaluable BestCandidate
    {
        get
        {
            return BestChromosome;
        }
    }

    /// <summary>
    /// Gets the time evolving.
    /// </summary>
    public TimeSpan TimeEvolving { get; private set; }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public OptimizerState State
    {
        get
        {
            return m_state;
        }

        private set
        {
            var shouldStop = OnStopped != null && m_state != value && value == OptimizerState.Stopped;

            m_state = value;

            if (shouldStop)
            {
                OnStopped?.Invoke();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
    public bool IsRunning
    {
        get
        {
            return State == OptimizerState.Started || State == OptimizerState.Resumed;
        }
    }

    /// <summary>
    /// Gets or sets the task executor which will be used to execute fitness evaluation.
    /// </summary>
    public ITaskExecutor TaskExecutor { get; set; }

    #endregion

    public string GetName()
    {
        return "Genetic algorithm";
    }

    public void Pause()
    {
        OnPaused?.Invoke();
    }

    /// <summary>
    /// Resumes the last evolution of the genetic algorithm.
    /// <remarks>
    /// If genetic algorithm was not explicit Stop (calling Stop method), you will need provide a new extended Termination.
    /// </remarks>
    /// </summary>
    public void Resume()
    {
        OnResumed?.Invoke();
        try
        {
            lock (m_lock)
            {
                m_stopRequested = false;
            }

            if (Population.GenerationsNumber == 0)
            {
                throw new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started.");
            }

            if (Population.GenerationsNumber > 1)
            {
                if (Termination.HasReached(this))
                {
                    throw new InvalidOperationException("Attempt to resume a genetic algorithm with a termination ({0}) already reached. Please, specify a new termination or extend the current one.".With(Termination));
                }

                State = OptimizerState.Resumed;
            }

            if (EndCurrentGeneration())
            {
                return;
            }

            bool terminationConditionReached = false;

            do
            {
                if (m_stopRequested)
                {
                    break;
                }

                m_stopwatch.Restart();
                terminationConditionReached = EvolveOneGeneration();
                m_stopwatch.Stop();
                TimeEvolving += m_stopwatch.Elapsed;
            }
            while (!terminationConditionReached);
        }
        catch
        {
            State = OptimizerState.Stopped;
            throw;
        }
    }

    /// <summary>
    /// Evolve one generation.
    /// </summary>
    /// <returns>True if termination has been reached, otherwise false.</returns>
    private bool EvolveOneGeneration()
    {
        var parents = SelectParents();
        var offspring = Cross(parents);
        Mutate(offspring);
        var newGenerationChromosomes = Reinsert(offspring, parents);
        Population.CreateNewGeneration(newGenerationChromosomes);
        return EndCurrentGeneration();
    }

    /// <summary>
    /// Reinsert the specified offspring and parents.
    /// </summary>
    /// <param name="offspring">The offspring chromosomes.</param>
    /// <param name="parents">The parents chromosomes.</param>
    /// <returns>
    /// The reinserted chromosomes.
    /// </returns>
    private IList<IEvaluable> Reinsert(IList<IEvaluable> offspring, IList<IEvaluable> parents)
    {
        return Reinsertion.SelectChromosomes(Population, offspring, parents);
    }

    /// <summary>
    /// Crosses the specified parents.
    /// </summary>
    /// <param name="parents">The parents.</param>
    /// <returns>The result chromosomes.</returns>
    private IList<IEvaluable> Cross(IList<IEvaluable> parents)
    {
        return OperatorsStrategy.Cross(Population, Crossover, CrossoverProbability, parents);
    }

    /// <summary>
    /// Mutate the specified chromosomes.
    /// </summary>
    /// <param name="chromosomes">The chromosomes.</param>
    private void Mutate(IList<IEvaluable> chromosomes)
    {
        OperatorsStrategy.Mutate(Mutation, MutationProbability, chromosomes);
    }

    /// <summary>
    /// Selects the parents.
    /// </summary>
    /// <returns>The parents.</returns>
    private IList<IEvaluable> SelectParents()
    {
        return Selection.SelectEvaluables(Population.MinSize, Population.CurrentGeneration);
    }

    /// <summary>
    /// Ends the current generation.
    /// </summary>
    /// <returns><c>true</c>, if current generation was ended, <c>false</c> otherwise.</returns>
    private bool EndCurrentGeneration()
    {
        EvaluateFitness();
        Population.EndCurrentGeneration();

        OnGenerationRan?.Invoke();

        if (Termination.HasReached(this))
        {
            State = OptimizerState.TerminationReached;
            OnTerminationReached?.Invoke();

            return true;
        }

        if (m_stopRequested)
        {
            TaskExecutor.Stop();
            State = OptimizerState.Stopped;
        }

        return false;
    }

    /// <summary>
    /// Evaluates the fitness.
    /// </summary>
    private void EvaluateFitness()
    {
        try
        {
            var chromosomesWithoutFitness = Population.CurrentGeneration.Evaluables.Where(c => !c.Fitness.HasValue).ToList();

            for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
            {
                var c = chromosomesWithoutFitness[i];

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

        Population.CurrentGeneration.Evaluables = Population.CurrentGeneration.Evaluables.OrderByDescending(c => c.Fitness).ToList();
    }

    /// <summary>
    /// Runs the evaluate fitness.
    /// </summary>
    /// <param name="chromosome">The chromosome.</param>
    private void RunEvaluateFitness(object chromosome)
    {
        var c = chromosome as IChromosome;

        try
        {
            c.Fitness = Evaluator.Evaluate(c);
        }
        catch (Exception ex)
        {
            throw new FitnessException(Evaluator, "Error executing Fitness.Evaluate for chromosome: {0}".With(ex.Message), ex);
        }
    }

    /// <summary>
    /// Starts the genetic algorithm using population, fitness, selection, crossover, mutation and termination configured.
    /// </summary>
    public void Start()
    {
        OnStarted?.Invoke();
        lock (m_lock)
        {
            State = OptimizerState.Started;
            m_stopwatch = Stopwatch.StartNew();
            Population.CreateInitialGeneration();
            m_stopwatch.Stop();
            TimeEvolving = m_stopwatch.Elapsed;
        }
        Resume();
    }

    /// <summary>
    /// Stops the genetic algorithm..
    /// </summary>
    public void Stop()
    {
        if (Population.GenerationsNumber == 0)
        {
            throw new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started.");
        }

        lock (m_lock)
        {
            m_stopRequested = true;
        }
        OnStopped?.Invoke();
    }

    public VisualElement CIGUI()
    {
        var ve = new VisualElement();

        var population = new DropdownField("Population");
        var popuClass = new ClassDropDown(population, typeof(IPopulation), false);
        popuClass.Dropdown.RegisterCallback<ChangeEvent<string>>(v => {
            var value = popuClass.GetChoiceInstance();
            this.population = value as IPopulation;
            if (value is IShowable)
                popuClass.Dropdown.value = v.newValue;
        });
        ve.Add(population);

        var fitness = new DropdownField("Fitness");
        var fitClass = new ClassDropDown(fitness, typeof(IEvaluator), true);
        fitClass.Dropdown.RegisterCallback<ChangeEvent<string>>(v => {
            var value = fitClass.GetChoiceInstance();
            this.fitness = value as IEvaluator;
            if (value is IShowable)
                fitClass.Dropdown.value = v.newValue;
        });
        ve.Add(fitness);

        var selection = new DropdownField("Selection");
        var selClass = new ClassDropDown(selection, typeof(ISelection), true);
        selClass.Dropdown.RegisterCallback<ChangeEvent<string>>(v => {
            var value = selClass.GetChoiceInstance();
            this.selection = value as ISelection;
            if (value is IShowable)
                selClass.Dropdown.value = v.newValue;
        });
        ve.Add(selection);

        var crossover = new DropdownField("Crossover");
        var crosClass = new ClassDropDown(crossover, typeof(ICrossover), true);
        crosClass.Dropdown.RegisterCallback<ChangeEvent<string>>(v => {
            var value = crosClass.GetChoiceInstance();
            this.crossover = value as ICrossover;
            if (value is IShowable)
                crosClass.Dropdown.value = v.newValue;
        });
        ve.Add(crossover);

        var mutation = new DropdownField("Mutation");
        var mutClass = new ClassDropDown(mutation, typeof(IMutation), true);
        mutClass.Dropdown.RegisterCallback<ChangeEvent<string>>(v => {
            var value = mutClass.GetChoiceInstance();
            this.mutation = value as IMutation;
            if (value is IShowable)
                mutClass.Dropdown.value = v.newValue;
        });
        ve.Add(mutation);

        return ve;

        
    }
}
