
using FsCheck;

namespace Test
{
    public class UnitTest1
    {
        public static object[][] TestCases = new[]
        {
            new object[]{
                new QuickBattleInput(PlayerTeam.デオキシス, EnemyTeam.サンダー, 257, 648, 326, 281),
                new QuickBattleInput(PlayerTeam.ジラーチ, EnemyTeam.サンダー, 349, 325, 336, 313),
                new uint[] { 0x233F7EC1u, 0xF03F7EC1u }
            },
            new object[]{
                new QuickBattleInput(PlayerTeam.デオキシス, EnemyTeam.ファイヤー, 256, 650, 327, 256),
                new QuickBattleInput(PlayerTeam.ミュウツー, EnemyTeam.フリーザー, 362, 349, 320, 388),
                new uint[] { 0x4D1FFF4Du }
            },
            new object[]
            {
                new QuickBattleInput(PlayerTeam.ミュウ, EnemyTeam.フリーザー, 340, 335, 309, 344),
                new QuickBattleInput(PlayerTeam.ミュウ, EnemyTeam.ラティアス, 357, 321, 289, 290),
                new uint[] { 0x9F297767, 0x51297767, 0x03297767 }
            }
        };

        private static readonly XDDBClient client = new();

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Test1(QuickBattleInput first, QuickBattleInput second, IEnumerable<uint> expected)
        {
            var res = client.Search(first, second);
            Assert.Equal(expected.OrderBy(_ => _), res.OrderBy(_ => _));
        }

        [Fact]
        public void Test2()
        {
            var searcher = new QuickBattleSeedSearcher(client);
            var res = searcher.Next(new QuickBattleInput(PlayerTeam.ミュウ, EnemyTeam.フリーザー, 340, 335, 309, 344));
            Assert.Equal(res, Array.Empty<uint>());
            res = searcher.Next(new QuickBattleInput(PlayerTeam.ミュウ, EnemyTeam.ラティアス, 357, 321, 289, 290));
            Assert.Equal(res.OrderBy(_ => _), new uint[] { 0x9F297767, 0x51297767, 0x03297767 }.OrderBy(_ => _));
            res = searcher.Next(new QuickBattleInput(PlayerTeam.レックウザ, EnemyTeam.サンダー, 347, 241, 321, 293));
            Assert.Equal(res.OrderBy(_ => _), new uint[] { 0x1257DEF0 }.OrderBy(_ => _));
        }
    }

    public class GenerationTest
    {
        private static readonly XDDarkPokemon _zapdos, _articuno, _dragonite, _ralts;
        static GenerationTest()
        {
            _zapdos = XDRNGSystem.GetDarkPokemon("サンダー");
            foreach (var pre in _zapdos.PreGeneratePokemons.OfType<PreGenerateDarkPokemon>())
                pre.isFixed = true;

            _articuno = XDRNGSystem.GetDarkPokemon("フリーザー");
            _dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");
            _ralts = XDRNGSystem.GetDarkPokemon("ラルトス");
        }

        [Theory, 
            InlineData(0x604DB56Eu),
            InlineData(0xD4E241C7u),
            InlineData(0x37E1798Au)]
        public void TestGenerateFixedZapdos(uint seed)
        {
            var result = _zapdos.Generate(seed);
            Assert.Equal(0x3F3C705Eu, result.PID);
            Assert.Equal(new uint[] { 30, 31, 31, 30, 31, 31 }, result.IVs);
        }

        [Theory, 
            InlineData(0xE06758FDu),
            InlineData(0xF9B0EF42u),
            InlineData(0x68031769u)]
        public void TestGenerateArticuno(uint seed)
        {
            var result = _articuno.Generate(seed);
            Assert.Equal(0xC351DEF2u, result.PID);
            Assert.Equal(new uint[] { 31, 31, 31, 31, 31, 31 }, result.IVs);
        }

        [Theory,
            InlineData(0x00F402D5u),
            InlineData(0xBBCAB518u),
            InlineData(0x6F0240B9u)]
        public void TestGenerateDragonite(uint seed)
        {
            var result = _dragonite.Generate(seed);
            Assert.Equal(0xB1961329u, result.PID);
            Assert.Equal(new uint[] { 31, 31, 31, 4, 31, 31 }, result.IVs);
        }

        [Theory,
            InlineData(0x0671D11Du)]
        public void TestGenerateShinyBlockedRalts(uint seed)
        {
            var notBlocked = _ralts.Generate(seed);
            Assert.Equal(0x9D8707EBu, notBlocked.PID);
            Assert.Equal(new uint[] { 31, 0, 31, 31, 31, 31 }, notBlocked.IVs);

            var blocked = _ralts.Generate(seed, 39528);
            Assert.Equal(0xED1D5D5Bu, blocked.PID);
            Assert.Equal(new uint[] { 31, 0, 31, 31, 31, 31 }, blocked.IVs);
        }

    }

    public class ReverseTest
    {
        [Fact]
        public void TestReverseDragoniteWithoutShinySkip()
        {
            var dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");

            var tsv = 0u;
            var results = dragonite.CalcBack(31, 31, 31, 4, 31, 31, tsv).ToArray();

            Assert.Single(results);

            var result = results[0];
            var expectedPID = 0xB1961329u;
            Assert.Equal(expectedPID, result.targetIndividual.PID);
            Assert.Equal(new uint[] { 31, 31, 31, 4, 31, 31 }, result.targetIndividual.IVs);
            Assert.Equal(280, result.generatableSeeds.Length);
            Assert.All(result.generatableSeeds, (_) =>
            {
                Assert.Equal(expectedPID, dragonite.Generate(_).PID);
            });
        }

        [Fact]
        public void TestReverseDragoniteWithShinySkip()
        {
            var dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");

            var tsv = 41656u;
            var results = dragonite.CalcBack(31, 31, 31, 4, 31, 31, tsv).ToArray();

            Assert.Single(results);

            var result = results[0];
            var expectedPID = 0x4EEEDFD5u;

            Assert.Equal(expectedPID, result.targetIndividual.PID);
            Assert.Equal(new uint[] { 31, 31, 31, 4, 31, 31 }, result.targetIndividual.IVs);
            Assert.Equal(280, result.generatableSeeds.Length);
            Assert.All(result.generatableSeeds, (_) =>
            {
                Assert.Equal(expectedPID, dragonite.Generate(_, tsv).PID);
            });
        }

        [Fact]
        public void TestReverseDragonite()
        {
            var dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");

            var results = dragonite.CalcBack(31, 31, 31, 4, 31, 31).ToArray();

            Assert.Equal(2, results.Length);

            Assert.All(results, (result) =>
            {
                var expectedPID = result.ConditionedTSV == null ? 0xB1961329u : 0x4EEEDFD5u;
                Assert.Equal(expectedPID, result.targetIndividual.PID);
                Assert.Equal(new uint[] { 31, 31, 31, 4, 31, 31 }, result.targetIndividual.IVs);
                Assert.Equal(280, result.generatableSeeds.Length);

                if (result.ConditionedTSV != null)
                {
                    Assert.Equal(41656u, result.ConditionedTSV);
                    Assert.All(result.generatableSeeds, (_) =>
                    {
                        var pid = dragonite.Generate(_, result.ConditionedTSV.Value).PID;
                        Assert.Equal(expectedPID, pid);
                    });
                }
                else
                {
                    Assert.Equal(new uint[] { 56680, 41656 }, result.ContraindicatedTSVs);
                    Assert.All(result.generatableSeeds, (_) =>
                    {
                        var pid = dragonite.Generate(_).PID;
                        Assert.Equal(expectedPID, pid);
                    });
                }
            });
        }

        [Fact]
        public void TestReverseDragoniteContraindicatedTSVs()
        {
            var dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");

            var results = dragonite.CalcBack(31, 31, 31, 7, 31, 31).Where(_ => _.ConditionedTSV == null).ToArray();
            Assert.Single(results);

            var result = results[0];

            Assert.Equal(new uint[] { 55120, 9184, 40056, 17120, 21216, 36480 }.OrderBy(_ => _), result.ContraindicatedTSVs.OrderBy(_ => _));
        }

        public static IEnumerable<object[]> TestCasesOfTestReverseWeepinbellContraindicatedTSVs()
        {
            static object[] Case(uint c, uint[] expectedTSVs) => new object[] { c, expectedTSVs };

            yield return Case(c: 6, expectedTSVs: new uint[] { 47136, 59448, 49144, 59696 });
            yield return Case(c: 29, expectedTSVs: new uint[] { 40880, 23520, 62120 });
        }
        [Theory]
        [MemberData(nameof(TestCasesOfTestReverseWeepinbellContraindicatedTSVs))]
        public void TestReverseWeepinbellContraindicatedTSVs(uint c, uint[] expectedTSVs)
        {
            var weepinbell = XDRNGSystem.GetDarkPokemon("ウツドン");

            var results = weepinbell.CalcBack(31, 31, 31, c, 31, 31).Where(_ => _.ConditionedTSV == null).ToArray();
            Assert.Single(results);

            var result = results[0];

            Assert.Equal(expectedTSVs.OrderBy(_ => _), result.ContraindicatedTSVs.OrderBy(_ => _));
        }

        [Fact]
        public void TestReverseDragonitePreGenerateShinySkip()
        {
            var dragonite = XDRNGSystem.GetDarkPokemon("カイリュー");

            var results = dragonite.CalcBack(9, 31, 31, 31, 31, 31).OrderBy(_ => _.targetIndividual.PID).ToArray();

            Assert.Equal(2, results.Length);

            {
                var result = results[0];
                var expectedPID = 0x00296B0Du;
                Assert.Equal(new uint[] { 9, 31, 31, 31, 31, 31 }, result.targetIndividual.IVs);
                Assert.Equal(expectedPID, result.targetIndividual.PID);
                Assert.NotNull(result.ConditionedTSV);
                var tsv = result.ConditionedTSV!.Value;
                Assert.Equal(299, result.generatableSeeds.Length);
                Assert.All(result.generatableSeeds, (_) =>
                {
                    Assert.Equal(expectedPID, dragonite.Generate(_, tsv).PID);
                });
            }

            {
                var result = results[1];
                var expectedPID = 0x5B07C9BFu;
                Assert.Equal(new uint[] { 9, 31, 31, 31, 31, 31 }, result.targetIndividual.IVs);
                Assert.Equal(expectedPID, result.targetIndividual.PID);
                Assert.NotNull(result.ConditionedTSV);
                var tsv = result.ConditionedTSV!.Value;
                Assert.Equal(402, result.generatableSeeds.Length);
                Assert.All(result.generatableSeeds, (_) =>
                {
                    Assert.Equal(expectedPID, dragonite.Generate(_, tsv).PID);
                });
            }
        }


    }

}
