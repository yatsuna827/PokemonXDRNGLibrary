using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary.CommonExtension;
using PokemonXDRNGLibrary;
using PokemonXDRNGLibrary.AdvanceSource;
using PokemonStandardLibrary;
using static System.Console;
using System.Diagnostics;
using PokemonStandardLibrary.Gen3;

var seed = 0xE3DFC811u;
var generator = new TeamGenerator(TeamGenerator.Greevil);
var firstAngleGenerator = new FirstCameraAngleGenerator();
for (int i=0; i< 10; i++)
{
    var _seed = seed.NextSeed(4);
    var angle = firstAngleGenerator.Generate(ref _seed, new AngleHistory(0, 6));
    var result = _seed.Generate(generator);

    WriteLine($"{i} {seed:X8} {angle} {result[0].PID:X8} {string.Join("-", result[0].IVs)}");
    WriteLine($"{i} {seed:X8} {angle} {result[1].PID:X8} {string.Join("-", result[1].IVs)}");

    seed.Advance();
}

static void Hoge()
{
    var tama = XDRNGSystem.GetDarkPokemon("サイドン");
    // foreach (var d in tama.PreGeneratePokemons.Take(4).OfType<PreGenerateDarkPokemon>()) d.isFixed = true;

    for (uint c = 0; c < 1; c++)
    {
        foreach (var result in tama.CalcBack(31, 31, 31, 31, 31, 31))
        {
            var id = result.ConditionedTSV == null ? $"{string.Join(",", result.ContraindicatedTSVs)}以外" : $"{result.ConditionedTSV}";
            WriteLine($"{result.representativeSeed:X8} {string.Join("-", result.targetIndividual.IVs)} {result.targetIndividual.Nature.ToJapanese()} TSV: {id}");

            if (result.generatableSeeds.Length > 0)
                WriteLine($"{result.generatableSeeds.Length} {result.generatableSeeds.Last():X8} {result.generatableSeeds.First():X8}");
            else
                WriteLine("cannot reach");
            
            //foreach (var seed in result.generatableSeeds) WriteLine($"{seed:X8}");
        }
    }

    WriteLine("おしり");
    ReadKey();
}

static uint RandomSeed(uint m, uint n)
{
    var random = new Random();
    var rand = (uint)random.Next(0x10000) / m * m + n;
    var seed = rand << 16 | (uint)random.Next(0x10000);
    return seed.PrevSeed();
}


static GCIndividual[] Generate(uint seed, AngleHistory history)
{
    var firstAngleGenerator = new FirstCameraAngleGenerator();
    var angleGenerator = new CameraAngleGenerator();

    // 1回目のアングル変更
    {
        var r = angleGenerator.Generate(ref seed, history);
        history = history.Next((byte)r);
    }

    // 2回目のアングル変更
    {
        var r = angleGenerator.Generate(ref seed, history);
        history = history.Next((byte)r);
    }

    // 瞬きによる2消費 + マップロードでの9消費 + 戦闘突入時の4消費
    seed.Advance(15);

    WriteLine(history);

    firstAngleGenerator.Generate(ref seed, history);

    var generator = new TeamGenerator(TeamGenerator.Greevil);
    return generator.Generate(seed);
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
