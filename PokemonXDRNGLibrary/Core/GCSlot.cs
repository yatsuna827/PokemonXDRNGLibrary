using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using PokemonStandardLibrary.Gen3;
using PokemonStandardLibrary.CommonExtension;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PokemonXDRNGLibrary
{
    public class GCSlot : IGeneratable<GCIndividual, uint>, IGeneratableEffectful<GCIndividual, uint>, ILcgConsumer, ILcgConsumer<uint>
    {
        public uint Lv { get; }
        public Pokemon.Species Species { get; }
        public Gender FixedGender { get; }
        public Nature FixedNature { get; }

        private protected readonly IPIDGenerator _pidGenerator;

        public virtual GCIndividual Generate(uint seed, uint tsv = 0x10000)
        {
            seed.Advance(2); // dummyPID
            var ivs = seed.GenerateIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.Generate(_pidGenerator, tsv);

            return Species.GetIndividual(pid, Lv, ivs, abilityIndex, false);
        }
        public virtual GCIndividual Generate(ref uint seed, uint tsv = 0x10000)
        {
            seed.Advance(2); // dummyPID
            var ivs = seed.GenerateIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.Generate(_pidGenerator, tsv);

            return Species.GetIndividual(pid, Lv, ivs, abilityIndex, false);
        }

        public virtual void Use(ref uint seed)
        {
            seed.Advance(5); // dummyPID, IVs, ability

            _pidGenerator.Generate(ref seed);
        }
        public virtual uint ComputeConsumption(uint seed)
        {
            seed.Advance(5); // dummyPID, IVs, ability

            _pidGenerator.Generate(ref seed);

            return seed;
        }

        public virtual void Use(ref uint seed, uint tsv)
        {
            seed.Advance(5); // dummyPID, IVs, ability

            _pidGenerator.Generate(ref seed, tsv);
        }
        public virtual uint ComputeConsumption(uint seed, uint tsv)
        {
            seed.Advance(5); // dummyPID, IVs, ability

            _pidGenerator.Generate(ref seed, tsv);

            return seed;
        }

        public GCIndividual Generate(uint seed, Criteria criteria)
        {
            seed.Advance(2); // dummyPID
            var ivs = seed.GenerateIVs();
            if (!criteria.CheckIVs(ivs)) return null;

            uint AbilityIndex = seed.GetRand(2);
            if (!criteria.CheckAbility(Species.Ability[(int)AbilityIndex])) return null;

            var pid = _pidGenerator.Generate(ref seed);

            var indiv = Species.GetIndividual(pid, Lv, ivs, AbilityIndex, false);
            if (!criteria.CheckGender(indiv.Gender)) return null;
            if (!criteria.CheckNature(indiv.Nature)) return null;
            if (!criteria.CheckShiny(indiv.PID.IsShiny(criteria.TSV))) return null;
            if (!criteria.CheckHiddenPowerType(indiv.HiddenPowerType)) return null;
            if (!criteria.CheckHiddenPowerPower(indiv.HiddenPower)) return null;

            return indiv;
        }
        public GCIndividual Generate(uint seed, uint tsv, Criteria criteria)
        {
            seed.Advance(2); // dummyPID
            uint[] IVs = seed.GenerateIVs();
            if (!criteria.CheckIVs(IVs)) return null;
            uint AbilityIndex = seed.GetRand(2);
            if (!criteria.CheckAbility(Species.Ability[(int)AbilityIndex])) return null;

            var pid = _pidGenerator.Generate(ref seed, tsv);
            var indiv = Species.GetIndividual(pid, Lv, IVs, AbilityIndex, false);
            if (!criteria.CheckGender(indiv.Gender)) return null;
            if (!criteria.CheckNature(indiv.Nature)) return null;
            if (!criteria.CheckShiny(indiv.PID.IsShiny(criteria.TSV))) return null;
            if (!criteria.CheckHiddenPowerType(indiv.HiddenPowerType)) return null;
            if (!criteria.CheckHiddenPowerPower(indiv.HiddenPower)) return null;

            return indiv;
        }
         
        public GCIndividual GenerateDummy(ref uint seed, uint tsv)
        {
            seed.Advance(2);
            var ivs = seed.GenerateIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.Generate(_pidGenerator, tsv);
            var evs = seed.GenerateEVs();
            return Species.GetIndividual(pid, 100, ivs, evs, abilityIndex, false);
        }

        internal GCSlot(string name)
        {
            Species = Pokemon.GetPokemon(name);
            Lv = 50;
            FixedGender = Gender.Genderless;
            FixedNature = Nature.other;

            _pidGenerator = new BasicPIDGenerator();
        }
        internal GCSlot(string name, string form)
        {
            Species = Pokemon.GetPokemon(name, form);
            Lv = 50;
            FixedGender = Gender.Genderless;
            FixedNature = Nature.other;

            _pidGenerator = new BasicPIDGenerator();
        }
        internal GCSlot(string name, Gender gender, Nature nature)
        {
            Species = Pokemon.GetPokemon(name);
            Lv = 50;
            FixedGender = gender;
            FixedNature = nature;

            _pidGenerator = GetPIDGenerator(nature, Species, gender);
        }
        internal GCSlot(string name, string form, Gender gender, Nature nature)
        {
            Species = Pokemon.GetPokemon(name, form);
            Lv = 50;
            FixedGender = gender;
            FixedNature = nature;

            _pidGenerator = GetPIDGenerator(nature, Species, gender);
        }
        internal GCSlot(string name, uint lv, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Species = Pokemon.GetPokemon(name);
            Lv = lv;
            FixedGender = g;
            FixedNature = n;

            _pidGenerator = GetPIDGenerator(n, Species, g);
        }

        private static IPIDGenerator GetPIDGenerator(Nature fixedNature, Pokemon.Species species, Gender fixedGender)
        {
            var genderIsFixed = fixedGender != Gender.Genderless && !species.GenderRatio.IsFixed();
            var natureIsFixed = fixedNature != Nature.other;

            if (genderIsFixed && natureIsFixed)
                return new ConditionalPIDGenerator(fixedNature, species, fixedGender);

            if (genderIsFixed)
                return new GenderConditionalPIDGenerator(species, fixedGender);

            if (natureIsFixed)
                return new NatureConditionalPIDGenerator(fixedNature);
            
            return new BasicPIDGenerator();
        }
    }

    static class GenerateModules
    {
        public static uint[] GenerateIVs(ref this uint seed)
        {
            var hab = seed.GetRand();
            var scd = seed.GetRand();
            return new uint[6] {
                hab & 0x1f,
                (hab >> 5) & 0x1f,
                (hab >> 10) & 0x1f,
                (scd >> 5) & 0x1f,
                (scd >> 10) & 0x1f,
                scd & 0x1f
            };
        }
        public static bool IsShiny(this uint PID, uint TSV) { return ((PID & 0xFFFF) ^ (PID >> 16) ^ TSV) < 8; }
        public static Gender GetGender(this uint PID, GenderRatio ratio)
        {
            if (ratio == GenderRatio.Genderless) return Gender.Genderless;
            return (PID & 0xFF) < (uint)ratio ? Gender.Female : Gender.Male;
        }
    }
}
