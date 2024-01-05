using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using PokemonStandardLibrary.Gen3;

namespace PokemonXDRNGLibrary
{
    public class GCSlot : IGeneratable<GCIndividual, uint>, IGeneratableEffectful<GCIndividual, uint>
    {
        public uint Lv { get; }
        public Pokemon.Species Pokemon { get; }
        public Gender FixedGender { get; }
        public Nature FixedNature { get; }

        public virtual GCIndividual Generate(uint seed, uint tsv = 0x10000)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            var ivs = seed.GetIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.GetPID(tsv, Pokemon.GenderRatio, FixedGender, FixedNature);

            return Pokemon.GetIndividual(pid, Lv, ivs, abilityIndex).SetRepSeed(rep);
        }
        public virtual GCIndividual Generate(ref uint seed, uint tsv = 0x10000)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            var ivs = seed.GetIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.GetPID(tsv, Pokemon.GenderRatio, FixedGender, FixedNature);

            return Pokemon.GetIndividual(pid, Lv, ivs, abilityIndex).SetRepSeed(rep);
        }

        public virtual GCIndividual Generate(uint seed, out uint finSeed)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            var ivs = seed.GetIVs();
            var abilityIndex = seed.GetRand(2);

            uint pid;
            while (true)
            {
                pid = (seed.GetRand() << 16) | seed.GetRand();
                if (FixedGender != Gender.Genderless && pid.GetGender(Pokemon.GenderRatio) != FixedGender)
                    continue;
                if (FixedNature != Nature.other && (Nature)(pid % 25) != FixedNature)
                    continue;
                break;
            }

            finSeed = seed;
            return Pokemon.GetIndividual(pid, Lv, ivs, abilityIndex).SetRepSeed(rep);
        }
        public virtual GCIndividual Generate(uint seed, out uint finSeed, uint TSV)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID;
            bool shinySkip = false;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));

                shinySkip |= PID.IsShiny(TSV);
                if (!PID.IsShiny(TSV)) break;
            }

            finSeed = seed;
            return Pokemon.GetIndividual(PID, Lv, IVs, AbilityIndex).SetRepSeed(rep).SetShinySkipped(shinySkip);
        }
        public GCIndividual Generate(uint seed, Criteria criteria)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            uint[] IVs = seed.GetIVs();
            if (!criteria.CheckIVs(IVs)) return GCIndividual.Empty;
            uint AbilityIndex = seed.GetRand(2);
            if (!criteria.CheckAbility(Pokemon.Ability[(int)AbilityIndex])) return GCIndividual.Empty;
            uint PID;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));

                break;
            }
            var indiv = Pokemon.GetIndividual(PID, Lv, IVs, AbilityIndex);
            if (!criteria.CheckGender(indiv.Gender)) return GCIndividual.Empty;
            if (!criteria.CheckNature(indiv.Nature)) return GCIndividual.Empty;
            if (!criteria.CheckShiny(indiv.PID.IsShiny(criteria.TSV))) return GCIndividual.Empty;
            if (!criteria.CheckHiddenPowerType(indiv.HiddenPowerType)) return GCIndividual.Empty;
            if (!criteria.CheckHiddenPowerPower(indiv.HiddenPower)) return GCIndividual.Empty;

            return indiv.SetRepSeed(rep);
        }
        public GCIndividual Generate(uint seed, uint TSV, Criteria criteria)
        {
            var rep = seed;
            seed.Advance(2); // dummyPID
            uint[] IVs = seed.GetIVs();
            if (!criteria.CheckIVs(IVs)) return GCIndividual.Empty;
            uint AbilityIndex = seed.GetRand(2);
            if (!criteria.CheckAbility(Pokemon.Ability[(int)AbilityIndex])) return GCIndividual.Empty;
            uint PID;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));

                if (!PID.IsShiny(TSV)) break;
            }
            var indiv = Pokemon.GetIndividual(PID, Lv, IVs, AbilityIndex);
            if (!criteria.CheckGender(indiv.Gender)) return GCIndividual.Empty;
            if (!criteria.CheckNature(indiv.Nature)) return GCIndividual.Empty;
            if (!criteria.CheckShiny(indiv.PID.IsShiny(criteria.TSV))) return GCIndividual.Empty;
            if (!criteria.CheckHiddenPowerType(indiv.HiddenPowerType)) return GCIndividual.Empty;
            if (!criteria.CheckHiddenPowerPower(indiv.HiddenPower)) return GCIndividual.Empty;

            return indiv.SetRepSeed(rep);
        }

        public GCIndividual GenerateDummy(ref uint seed, uint TSV)
        {
            var rep = seed;
            seed.Advance(2);
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));

                if (!PID.IsShiny(TSV)) break;
            }
            uint[] EVs = seed.GenerateEVs();
            return Pokemon.GetIndividual(PID, 100, IVs, EVs, AbilityIndex).SetRepSeed(rep);
        }
        internal GCSlot(Pokemon.Species p, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Pokemon = p;
            Lv = 50;
            FixedGender = g;
            FixedNature = n;
        }
        internal GCSlot(Pokemon.Species p, uint lv, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Pokemon = p;
            Lv = lv;
            FixedGender = g;
            FixedNature = n;
        }
        internal GCSlot(string name, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Pokemon = PokemonStandardLibrary.Gen3.Pokemon.GetPokemon(name);
            Lv = 50;
            FixedGender = g;
            FixedNature = n;
        }
        internal GCSlot(string name, uint lv, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Pokemon = PokemonStandardLibrary.Gen3.Pokemon.GetPokemon(name);
            Lv = lv;
            FixedGender = g;
            FixedNature = n;
        }
    }

    public class Slot
    {
        private readonly uint _lv;
        private readonly Pokemon.Species _species;
        private readonly Gender _fixedGender;
        private readonly Nature _fixedNature;

        public virtual GCIndividual Generate(uint seed, uint tsv = 0x10000)
        {
            seed.Advance(2); // dummyPID
            var ivs = seed.GetIVs();
            var abilityIndex = seed.GetRand(2);
            var pid = seed.GetPID(tsv, _species.GenderRatio, _fixedGender, _fixedNature);

            return _species.GetIndividual(pid, _lv, ivs, abilityIndex);
        }

    }

    public class BasicPIDGenerator : IGeneratableEffectful<uint, uint>
    {
        public uint Generate(ref uint seed, uint tsv)
        {
            var h16 = seed.GetRand();
            var l16 = seed.GetRand();
            var pid = (h16 << 16) | l16;

            // 高々3回しか呼ばれないので再帰にしてみた
            if ((h16 ^ l16 ^ tsv) < 8) return Generate(ref seed, tsv);

            return pid;
        }
    }
    public class NatureConditionalPIDGenerator : IGeneratableEffectful<uint, uint>
    {
        private readonly uint _fixedNature;
        public uint Generate(ref uint seed, uint tsv)
        {
            while (true)
            {
                var h16 = seed.GetRand();
                var l16 = seed.GetRand();
                var pid = (h16 << 16) | l16;

                if (pid % 25 != _fixedNature) continue;
                if ((h16 ^ l16 ^ tsv) < 8) continue;

                return pid;
            }
        }

        public NatureConditionalPIDGenerator(Nature fixedNature)
            => _fixedNature = (uint)fixedNature;

    }
    public class ConditionalPIDGenerator : IGeneratableEffectful<uint, uint>
    {
        private readonly uint _genderRatio;
        private readonly bool _fixedGenderIsFemale;
        private readonly uint _fixedNature;
        public uint Generate(ref uint seed, uint tsv)
        {
            while (true)
            {
                var h16 = seed.GetRand();
                var l16 = seed.GetRand();
                var pid = (h16 << 16) | l16;

                if (pid % 25 != _fixedNature) continue;
                if (((pid & 0xFF) < _genderRatio) != _fixedGenderIsFemale) continue;
                if ((h16 ^ l16 ^ tsv) < 8) continue;

                return pid;
            }
        }

        public ConditionalPIDGenerator(Nature fixedNature, Pokemon.Species species, Gender fixedGender)
        {
            _genderRatio = (uint)species.GenderRatio;
            _fixedGenderIsFemale = fixedGender == Gender.Female;
            _fixedNature = (uint)fixedNature;
        }
    }

    static class GenerateModules
    {
        public static uint GetPID(ref this uint seed, Func<uint, bool> condition)
        {
            uint PID;
            do { PID = (seed.GetRand() << 16) | seed.GetRand(); } while (!condition(PID));
            return PID;
        }
        public static uint GetPID(ref this uint seed, uint tsv, GenderRatio ratio, Gender fixedGender, Nature fixedNature)
        {
            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();

                if (fixedGender != Gender.Genderless && pid.GetGender(ratio) != fixedGender)
                    continue;
                if (fixedNature != Nature.other && (Nature)(pid % 25) != fixedNature)
                    continue;
                if (pid.IsShiny(tsv))
                    continue;

                return pid;
            }
        }
        public static uint[] GetIVs(ref this uint seed)
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
