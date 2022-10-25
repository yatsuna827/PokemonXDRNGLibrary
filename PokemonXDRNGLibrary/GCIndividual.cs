using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonStandardLibrary.Gen3;

namespace PokemonGCRNGLibrary
{
    public class GCIndividual : Pokemon.Individual
    {
        public new static readonly GCIndividual Empty = Pokemon.GetPokemon("Dummy").GetIndividual(0, 1, new uint[6], 0);
        public string GCAbility { get; }
        public uint RepresentativeSeed { get; private set; }
        public bool ShinySkipped { get; private set; }

        public uint[] EnemyStats { get; internal set; }
        public GCIndividual SetRepSeed(uint seed) { RepresentativeSeed = seed; return this; }
        public GCIndividual SetShinySkipped(bool skipped) { ShinySkipped = skipped; return this; }

        internal GCIndividual(Pokemon.Species species, uint pid, uint[] ivs, uint lv, uint xdability) : base(species, pid, ivs, lv)
        {
            this.GCAbility = species.Ability[(int)xdability];
        }
        internal GCIndividual(Pokemon.Species species, uint pid, uint[] ivs, uint[] evs, uint lv, uint xdability) : base(species, pid, ivs, lv)
        {
            this.GCAbility = species.Ability[(int)xdability];
            // EnemyStatsの計算を入れる.
        }
    }
    public static class GCExtension
    {
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint PID, uint Lv, uint[] IVs, uint ability) 
            => new GCIndividual(species, PID, IVs, Lv, ability);
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint PID, uint Lv, uint[] IVs, uint[] EVs, uint ability)
            => new GCIndividual(species, PID, IVs, EVs, Lv, ability);
    }

}
