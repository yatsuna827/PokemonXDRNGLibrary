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
        public GCIndividual[] preGeneratePokemons;
        public GCIndividual targetIndividual;
        public readonly uint representativeSeed;
        public RNGTarget(uint repSeed, GCIndividual indiv)
        {
            representativeSeed = repSeed;
            targetIndividual = indiv;
            generatableSeeds = new uint[0];
        }
        public RNGTarget(uint repSeed, GCIndividual indiv, uint[] genSeeds)
        {
            representativeSeed = repSeed;
            targetIndividual = indiv;
            generatableSeeds = genSeeds;
        }
    }
}
