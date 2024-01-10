using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonStandardLibrary.Gen3;

namespace PokemonXDRNGLibrary
{
    public class GCIndividual : Pokemon.Individual
    {
        public string GCAbility { get; }
        public bool ShinySkipped { get; }

        // public uint[] EnemyStats { get; }

        internal GCIndividual(Pokemon.Species species, uint pid, uint[] ivs, uint lv, uint gcAbility, bool shinySkipped) : base(species, pid, ivs, lv)
        {
            this.GCAbility = species.Ability[(int)gcAbility];
            this.ShinySkipped = shinySkipped;
        }
        internal GCIndividual(Pokemon.Species species, uint pid, uint[] ivs, uint[] evs, uint lv, uint gcAbility, bool shinySkipped) : base(species, pid, ivs, lv)
        {
            this.GCAbility = species.Ability[(int)gcAbility];
            this.ShinySkipped = shinySkipped;
            // EnemyStatsの計算を入れる.
        }
    }
    public static class GCExtension
    {
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint pid, uint lv, uint[] ivs, uint ability, bool shinySkipped) 
            => new GCIndividual(species, pid, ivs, lv, ability, shinySkipped);
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint pid, uint lv, uint[] ivs, uint[] evs, uint ability, bool shinySkipped)
            => new GCIndividual(species, pid, ivs, evs, lv, ability, shinySkipped);
    }
}
