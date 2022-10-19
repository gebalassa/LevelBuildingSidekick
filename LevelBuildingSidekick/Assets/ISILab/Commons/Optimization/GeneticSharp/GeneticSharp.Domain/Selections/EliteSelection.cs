using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using UnityEngine.UIElements;

namespace GeneticSharp.Domain.Selections
{
    /// <summary>
    /// Selects the chromosomes with the best fitness.
    /// </summary>
    /// <remarks>
    /// Also know as: Truncation Selection.
    /// </remarks>    
    [DisplayName("Elite")]
    public sealed class EliteSelection : SelectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.EliteSelection"/> class.
        /// </summary>
        public EliteSelection() : base()
        {
        }

        public override VisualElement CIGUI()
        {
            var content = new VisualElement();

            return content;
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IEvaluable> PerformSelectEvaluables(int number, Generation generation)
        {
            var ordered = generation.Evaluables.OrderByDescending(c => c.Fitness);
            return ordered.Take(number).ToList();
        }

        #endregion
    }
}