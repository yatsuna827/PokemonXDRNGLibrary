using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary.CommonExtension;
using PokemonXDRNGLibrary;
using PokemonXDRNGLibrary.AdvanceSource;
using PokemonStandardLibrary;
using static System.Console;
using System.Diagnostics;
using PokemonStandardLibrary.Gen3;

while (true)
{
    Hoge();
}

static void Hoge()
{
    var tama = XDRNGSystem.GetDarkPokemon("ボーマンダ");
    //foreach (var d in tama.PreGeneratePokemons.OfType<PreGenerateDarkPokemon>()) d.isFixed = true;

    for (uint c = 0; c < 32; c++)
    {
        foreach (var result in tama.CalcBack(30, 30, 30, c, 30, 31).Where(_ => _.targetIndividual.HiddenPower == 70))
        {
            var id = result.ConditionedTSV == null ? $"{string.Join(",", result.ContraindicatedTSVs)}以外" : $"{result.ConditionedTSV}";
            WriteLine($"{result.representativeSeed:X8} {string.Join("-", result.targetIndividual.IVs)} {result.targetIndividual.Nature.ToJapanese()} TSV: {id}");
            foreach (var seed in result.generatableSeeds) WriteLine($"{seed:X8}");
        }
    }

    WriteLine("おしり");
    ReadKey();
}

class Class1 : IGeneratableEffectful<uint, uint>
{
    private readonly uint _genderRatio;
    private readonly bool _hoge;
    private readonly bool _fixedGenderIsFemale;
    private readonly uint _fixedNature;
    public uint Generate(ref uint seed, uint tsv)
    {
        while (true)
        {
            var h16 = seed.GetRand();
            var l16 = seed.GetRand();
            var pid = (h16 << 16) | l16;

            if (_fixedNature != 25 && pid % 25 != _fixedNature) continue;
            if (_hoge && ((pid & 0xFF) < _genderRatio) != _fixedGenderIsFemale) continue;

            if ((h16 ^ l16 ^ tsv) < 8) continue;

            return pid;
        }
    }

    public Class1(Nature fixedNature, Pokemon.Species species, Gender fixedGender)
    {
        _genderRatio = (uint)species.GenderRatio;
        _fixedGenderIsFemale = fixedGender == Gender.Female;
        _fixedNature = (uint)fixedNature;

        _hoge = !((species.GenderRatio.IsFixed() || fixedGender == Gender.Genderless));
    }
}

class Class2 : IGeneratableEffectful<uint, uint>
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

    public Class2(Nature fixedNature, Pokemon.Species species, Gender fixedGender)
    {
        _genderRatio = (uint)species.GenderRatio;
        _fixedGenderIsFemale = fixedGender == Gender.Female;
        _fixedNature = (uint)fixedNature;
    }

    public static IGeneratableEffectful<uint, uint> Create(Nature fixedNature, Pokemon.Species species, Gender fixedGender)
    {
        if (species.GenderRatio.IsFixed() || fixedGender == Gender.Genderless)
            return new NatureConditionalPIDGenerator(fixedNature);

        return new Class2(fixedNature, species, fixedGender);
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
