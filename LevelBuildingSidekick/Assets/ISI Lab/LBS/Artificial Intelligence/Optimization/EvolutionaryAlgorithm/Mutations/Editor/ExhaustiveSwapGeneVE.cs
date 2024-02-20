using ISILab.LBS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    //[LBSCustomEditor("ExhaustiveSwapGene", typeof(ExhaustiveSwapGene))]
    public class ExhaustiveSwapGeneVE : LBSCustomEditor
    {

        public ExhaustiveSwapGeneVE(object target) : base(target)
        {
            Add(CreateVisualElement());
            SetInfo(target);
        }

        public override void SetInfo(object target)
        {
            this.target = target;
            var mut = target as ExhaustiveSwapGene;
        }

        protected override VisualElement CreateVisualElement()
        {
            Add(new Label("Exhaustive Swap Gene"));
            return this;
        }
    }
}