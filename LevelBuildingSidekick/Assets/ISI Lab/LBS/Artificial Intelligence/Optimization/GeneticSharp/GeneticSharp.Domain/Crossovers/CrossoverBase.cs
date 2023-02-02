﻿using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using System.Linq;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Crossovers
{
    /// <summary>
    /// A base class for crossovers.
    /// </summary>
    public abstract class CrossoverBase : ICrossover
    {
        #region Constructors
        protected CrossoverBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CrossoverBase"/> class.
        /// </summary>
        /// <param name="parentsNumber">The number of parents need for cross.</param>
        /// <param name="childrenNumber">The number of children generated by cross.</param>
        protected CrossoverBase(int parentsNumber, int childrenNumber) : this(parentsNumber, childrenNumber, 2)
        {
            ParentsNumber = parentsNumber;
            ChildrenNumber = childrenNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CrossoverBase"/> class.
        /// </summary>
        /// <param name="parentsNumber">The number of parents need for cross.</param>
        /// <param name="childrenNumber">The number of children generated by cross.</param>
        /// <param name="minChromosomeLength">The minimum length of the chromosome supported by the crossover.</param>
        protected CrossoverBase(int parentsNumber, int childrenNumber, int minChromosomeLength)
        {
            ParentsNumber = parentsNumber;
            ChildrenNumber = childrenNumber;
            MinLength = minChromosomeLength;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the operator is ordered (if can keep the chromosome order).
        /// </summary>
        public bool IsOrdered { get; protected set; }

        /// <summary>
        /// Gets the number of parents need for cross.
        /// </summary>
        /// <value>The parents number.</value>
        public int ParentsNumber { get; protected set; }

        /// <summary>
        /// Gets the number of children generated by cross.
        /// </summary>
        /// <value>The children number.</value>
        public int ChildrenNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum length of the chromosome supported by the crossover.
        /// </summary>
        /// <value>The minimum length of the chromosome.</value>
        public int MinLength { get; protected set; }

        public abstract VisualElement CIGUI();
        #endregion

        #region Methods        
        /// <summary>
        /// Cross the specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        public IList<IOptimizable> Cross(IList<IOptimizable> parents)
        {
            ExceptionHelper.ThrowIfNull("parents", parents);


            if (parents.Count != ParentsNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(parents), "The number of parents should be the same of ParentsNumber.");
            }

            var firstParent = parents[0].GetDataSquence<object>();

            if(firstParent == null)
            {
                throw new NullReferenceException("IEvaluable is not IChromosome");
            }

            if (firstParent.Length < MinLength)
            {
                throw new CrossoverException(
                    this, "A chromosome should have, at least, {0} genes. {1} has only {2} gene.".With(MinLength, firstParent.GetType().Name, firstParent.Length));
            }

            return PerformCross(parents);
        }

        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected abstract IList<IOptimizable> PerformCross(IList<IOptimizable> parents);
        #endregion        
    }
}
