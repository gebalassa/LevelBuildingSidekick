using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    public abstract class LBSChromosome : ChromosomeBase2D
    {
        protected LBSChromosome(Rect rect, int[] immutables = null) : base(rect, immutables) { }

        protected LBSChromosome() : base() { }
    }
}