﻿using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using System;
using GeneticSharp.Domain.Populations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using Commons.Optimization;

namespace Commons.Optimization
{
    public abstract class BaseOptimizerMetahuristic <TEvaluable>  where TEvaluable : IEvaluable
    {
        #region Properties

        public int GenerationsNumber { get; }

        public IEvaluable BestCandidate { get; set; }
        public IEvaluable Adam { get; set; }

        public TimeSpan TimeEvolving { get; set; }
        public bool IsRunning { get { return State == Op_State.Running || State == Op_State.Started || State == Op_State.Resumed; } }
        public IPopulation Population { get; set; }

        public ITermination Termination { get; set; }

        public IEvaluator Evaluator { get; set; }

        public Op_State State
        {
            get
            {
                return state;
            }
            set
            {
                var shouldStop = OnStopped != null && state != value && value == Op_State.Stopped;

                state = value;

                if (shouldStop)
                {
                    Stop();
                }
            }
        }

        public IEvaluable[] LastGeneration
        {
            get
            {
                return Population.CurrentGeneration.Evaluables.ToArray();
            }
        }

        

        #endregion

        #region Fields
        protected Op_State state;
        protected Stopwatch clock;
        protected readonly object m_lock;
        protected bool stopRequested;
        protected bool pauseRequested;
        #endregion


        #region Events
        public Action OnGenerationRan { get; set; }
        public Action OnTerminationReached { get; set; }
        public Action OnStopped { get; set; }
        public Action OnResumed { get; set; }
        public Action OnPaused { get; set; }
        public Action OnStarted { get; set; }
        #endregion

        //public BaseOptimizerMetahuristic() { }
        public BaseOptimizerMetahuristic(IEvaluable adam, IEvaluator evaluator, ITermination termination)
        {
            Adam = adam;
            BestCandidate = Adam;
            Evaluator = evaluator;
            Termination = termination;

            State = Op_State.NotStarted;
            m_lock = new object();
            stopRequested = pauseRequested = false;
            GenerationsNumber = 0;
            TimeEvolving = TimeSpan.Zero;
        }

        public virtual void Pause()
        {
            lock (m_lock)
            {
                State = Op_State.Paused;
            }
            OnPaused?.Invoke();
        }

        public virtual void Resume()
        {
            OnResumed?.Invoke();
            lock (m_lock)
            {
                State = Op_State.Resumed;
                pauseRequested = false;
            }

            Run();
        }

        public virtual void Stop()
        {
            lock (m_lock)
            {
                State = Op_State.Stopped;
            }
            OnStopped?.Invoke();
        }

        public virtual void Start()
        {
            OnStarted?.Invoke();
            lock (m_lock)
            {
                stopRequested = false;
                pauseRequested = false;
                State = Op_State.Started;
                clock = Stopwatch.StartNew();
            }

            Run();
        }

        public abstract IEvaluable RunOnce ();
        public abstract IEvaluable Run();

        public abstract List<IEvaluable> GetNeighbors( IEvaluable Adam);

        /*public virtual IEvaluable Run()
        {
            while (!TerminatioReached() && !(State == Op_State.Paused || State == Op_State.Stopped))
            {
                if (stopRequested)
                {
                    Stop();
                    break;
                }
                if (pauseRequested)
                {
                    Pause();
                    break;
                }

                clock.Restart();
                RunOnce();
                OnGenerationRan?.Invoke();
                clock.Stop();
                State = Op_State.Running;
            }

            return BestCandidate;
        }*/

        /// <summary>
        /// Determines if the optimizer has reached a termination condition.
        /// </summary>
        /// <returns>True if the termination condition has been reached, false otherwise.</returns>
        public bool TerminatioReached()
        {
            if (Termination.HasReached(this as BaseOptimizerMetahuristic<IEvaluable>))
            {
                OnTerminationReached?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Requests that the optimization process be stopped.
        /// </summary>
        public void RequestStop()
        {
            lock (m_lock)
            {
                stopRequested = true;
            }
        }

        /// <summary>
        /// Requests that the optimization process be paused.
        /// </summary>
        public void RequestPause()
        {
            lock (m_lock)
            {
                pauseRequested = true;
            }
        }

        public abstract string GetName();
    }

    public enum Op_State
    {
        /// <summary>
        /// The Optimizer has not been started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The Optimizer has been started and is running.
        /// </summary>
        Started,

        /// <summary>
        /// The Optimizer has been stopped and is not running.
        /// </summary>
        Stopped,

        /// <summary>
        /// The Optimizer has been resumed after a stop or termination reach and is running.
        /// </summary>
        Resumed,

        /// <summary>
        /// The Optimizer has not been stopped or reached termination and is still running.
        /// </summary>
        Running,

        /// <summary>
        /// The Optimizer has reach the termination condition and is not running.
        /// </summary>
        TerminationReached,

        /// <summary>
        /// The Optimizer has been paused and is not running.
        /// </summary>
        Paused
    }
}