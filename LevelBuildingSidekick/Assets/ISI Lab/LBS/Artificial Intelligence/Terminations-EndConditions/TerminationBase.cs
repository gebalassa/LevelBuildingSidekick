using GeneticSharp.Infrastructure.Framework.Texts;
using GeneticSharp.Infrastructure.Framework.Commons;
using UnityEngine.UIElements;

namespace Commons.Optimization.Terminations
{
    /// <summary>
    /// Base class for ITerminations implementations.
    /// </summary>
    public abstract class TerminationBase : ITermination
    {
        #region Fields
        private bool m_hasReached;

        public abstract VisualElement CIGUI();
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="optimizer">The genetic algorithm.</param>
        public bool HasReached(IOptimizer optimizer)
        {
            ExceptionHelper.ThrowIfNull("geneticAlgorithm", optimizer);

            m_hasReached = PerformHasReached(optimizer);

            return m_hasReached;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="GeneticSharp.Domain.Terminations.LogicalOperatorTerminationBase"/>.</returns>
        public override string ToString()
        {
            return "{0} (HasReached: {1})".With(GetType().Name, m_hasReached);
        }

        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="optimizer">The genetic algorithm.</param>
        protected abstract bool PerformHasReached(IOptimizer optimizer);
        #endregion
    }
}