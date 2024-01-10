using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonXDRNGLibrary
{
    public class RNGTarget
    {
        public uint[] generatableSeeds;
        public GCIndividual targetIndividual;
        public readonly uint representativeSeed;

        public uint? ConditionedTSV { get; }
        public uint[] ContraindicatedTSVs { get; }

        public RNGTarget(uint repSeed, GCIndividual indiv, uint[] genSeeds, uint? conditionedTSV = null, uint[] contraindicatedTSVs = null)
        {
            representativeSeed = repSeed;
            targetIndividual = indiv;
            generatableSeeds = genSeeds;
            ConditionedTSV = conditionedTSV;
            ContraindicatedTSVs = contraindicatedTSVs ?? Array.Empty<uint>();
        }
    }
}
