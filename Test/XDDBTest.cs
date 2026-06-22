namespace Test
{
    public class XDDBTest
    {
        private static readonly XDDBClient client = new(PokemonXDRNGLibrary.XDDB.Data.XDDBSeeds.GetStream());

        [Theory]
        [MemberData(nameof(UnitTest1.TestCases), MemberType = typeof(UnitTest1))]
        public void SearchReturnsSameResultAsExpected(QuickBattleInput first, QuickBattleInput second, uint[] expected)
        {
            var res = client.Search(first, second);
            Assert.Equal(expected.OrderBy(_ => _), res.OrderBy(_ => _));
        }

        // indexと対応しないseedsデータが渡された場合、コンストラクタで検出して例外を投げること
        [Fact]
        public void ConstructorThrowsForMismatchedSeeds()
        {
            using var ms = new System.IO.MemoryStream();
            PokemonXDRNGLibrary.XDDB.Data.XDDBSeeds.GetStream().CopyTo(ms);
            var bytes = ms.ToArray();
            bytes[0] ^= 0xFF;

            Assert.Throws<System.IO.InvalidDataException>(() => new XDDBClient(new System.IO.MemoryStream(bytes)));
        }

        // 実際には存在しないコードが渡されても、検算で弾かれて空を返すこと
        [Fact]
        public void SearchReturnsEmptyForUnknownInput()
        {
            var res = client.Search(
                new QuickBattleInput(PlayerTeam.Mewtwo, EnemyTeam.Articuno, 322, 340, 290, 310),
                new QuickBattleInput(PlayerTeam.Mewtwo, EnemyTeam.Articuno, 322, 340, 290, 310));
            Assert.Empty(res);
        }
    }
}
