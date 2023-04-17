using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGene : MutationBase
{
    protected override void PerformMutate(ChromosomeBase chromosome, float probability)
    {
        var r = RandomizationProvider.Current;

        var d = r.GetDouble();

        if (d > probability)
        {
            return;
        }

        var i = r.GetInt(0, chromosome.Length);

        while(i < chromosome.Length && chromosome.GetGene(i) != default)
        {
            i++;
        }

        if(i < chromosome.Length)
        {
            chromosome.ReplaceGene(i, chromosome.GenerateGene());
        }
    }
}
