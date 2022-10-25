using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary.Gen3;

namespace PokemonGCRNGLibrary
{
    class XDTradePokemon
    {
        public const uint Lv = 20;
        public readonly Pokemon.Species pokemon;
        public GCIndividual Generate(uint seed)
        {
            seed.Advance(10);
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID = (seed.GetRand() << 16) | seed.GetRand();
            return pokemon.GetIndividual(PID, Lv, IVs, AbilityIndex);
        }
    }
}
