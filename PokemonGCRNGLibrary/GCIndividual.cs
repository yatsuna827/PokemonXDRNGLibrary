using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class GCIndividual : Pokemon.Individual
    {
        public new static readonly GCIndividual Empty = Pokemon.GetPokemon("Dummy").GetIndividual(0, 1, new uint[6], 0);
        public string XDAbility;
        public uint RepresentativeSeed;
        public bool shinySkipped;
        public uint[] EnemyStats { get; internal set; }
        public GCIndividual SetRepSeed(uint seed) { RepresentativeSeed = seed; return this; }
        public GCIndividual SetShinySkipped(bool skipped) { shinySkipped = skipped; return this; }
    }
    public static class GCExtension
    {
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint PID, uint Lv, uint[] IVs, uint ability)
        {
            var temp = species.GetIndividual(PID, Lv, IVs);
            return new GCIndividual()
            {
                Name = temp.Name,
                Form = temp.Form,
                Lv = temp.Lv,
                PID = temp.PID,
                Nature = temp.Nature,
                Gender = temp.Gender,
                Ability = temp.Ability,
                IVs = temp.IVs,
                Stats = temp.Stats,
                HiddenPower = temp.HiddenPower,
                HiddenPowerType = temp.HiddenPowerType,
                XDAbility = species.Ability[ability]
            };

        }
        public static GCIndividual GetIndividual(this Pokemon.Species species, uint PID, uint Lv, uint[] IVs, uint[] EVs, uint ability)
        {
            var temp = species.GetIndividual(PID, Lv, IVs, EVs);
            return new GCIndividual()
            {
                Name = temp.Name,
                Form = temp.Form,
                Lv = temp.Lv,
                PID = temp.PID,
                Nature = temp.Nature,
                Gender = temp.Gender,
                Ability = temp.Ability,
                IVs = temp.IVs,
                EVs = temp.EVs,
                Stats = temp.Stats,
                HiddenPower = temp.HiddenPower,
                HiddenPowerType = temp.HiddenPowerType,
                XDAbility = species.Ability[ability]
            };

        }
    }

}
