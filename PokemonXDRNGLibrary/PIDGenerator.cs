using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using PokemonStandardLibrary.Gen3;

namespace PokemonXDRNGLibrary
{
    interface IPIDGenerator : IGeneratableEffectful<uint>, IGeneratableEffectful<uint, uint> { }

    public class BasicPIDGenerator : IPIDGenerator
    {
        public uint Generate(ref uint seed)
            => (seed.GetRand() << 16) | seed.GetRand();
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
    public class NatureConditionalPIDGenerator : IPIDGenerator
    {
        private readonly uint _fixedNature;

        public uint Generate(ref uint seed)
        {
            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();

                if (pid % 25 != _fixedNature) continue;

                return pid;
            }
        }
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
    public class GenderConditionalPIDGenerator : IPIDGenerator
    {
        private readonly uint _genderRatio;
        private readonly bool _fixedGenderIsFemale;

        public uint Generate(ref uint seed)
        {
            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();

                if (((pid & 0xFF) < _genderRatio) != _fixedGenderIsFemale) continue;

                return pid;
            }
        }
        public uint Generate(ref uint seed, uint tsv)
        {
            while (true)
            {
                var h16 = seed.GetRand();
                var l16 = seed.GetRand();
                var pid = (h16 << 16) | l16;

                if (((pid & 0xFF) < _genderRatio) != _fixedGenderIsFemale) continue;
                if ((h16 ^ l16 ^ tsv) < 8) continue;

                return pid;
            }
        }

        public GenderConditionalPIDGenerator(Pokemon.Species species, Gender fixedGender)
        {
            _genderRatio = (uint)species.GenderRatio;
            _fixedGenderIsFemale = fixedGender == Gender.Female;
        }
    }
    public class ConditionalPIDGenerator : IPIDGenerator
    {
        private readonly uint _genderRatio;
        private readonly bool _fixedGenderIsFemale;
        private readonly uint _fixedNature;

        public uint Generate(ref uint seed)
        {
            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();

                if (pid % 25 != _fixedNature) continue;
                if (((pid & 0xFF) < _genderRatio) != _fixedGenderIsFemale) continue;

                return pid;
            }
        }
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

}
