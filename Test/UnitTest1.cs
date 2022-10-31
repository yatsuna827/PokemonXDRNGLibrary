
namespace Test
{
    public class UnitTest1
    {
        public static object[][] TestCases = new[]
        {
            new object[]{ 
                new XDQuickBattleArguments(PlayerTeam.デオキシス, EnemyTeam.サンダー, 257, 648, 326, 281), 
                new XDQuickBattleArguments(PlayerTeam.ジラーチ, EnemyTeam.サンダー, 349, 325, 336, 313),
                new uint[] { 0x233F7EC1u, 0xF03F7EC1u }
            },
            new object[]{
                new XDQuickBattleArguments(PlayerTeam.デオキシス, EnemyTeam.ファイヤー, 256, 650, 327, 256),
                new XDQuickBattleArguments(PlayerTeam.ミュウツー, EnemyTeam.フリーザー, 362, 349, 320, 388),
                new uint[] { 0x4D1FFF4Du }
            },
            new object[]
            {
                new XDQuickBattleArguments(PlayerTeam.ミュウ, EnemyTeam.フリーザー, 340, 335, 309, 344),
                new XDQuickBattleArguments(PlayerTeam.ミュウ, EnemyTeam.ラティオス, 357, 321, 289, 290),
                new uint[] { 0x9F297767, 0x51297767, 0x03297767 }
            }
        };

        private static readonly XDDBClient client = new();

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Test1(XDQuickBattleArguments first, XDQuickBattleArguments second, IEnumerable<uint> expected)
        {
            var res = client.Search(first, second);
            Assert.Equal(expected.OrderBy(_ => _), res.OrderBy(_ => _));
        }
    }
}