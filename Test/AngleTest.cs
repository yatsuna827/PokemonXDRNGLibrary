namespace Test
{
    public class FirstAngleTest
    {
        private static readonly FirstCameraAngleGenerator _generator = new();
        private static readonly FirstAngleReverser _reverser = new();

        [Property]
        public bool 生成後のseedの逆算結果には元のseedが含まれること(uint seed)
        {
            return _reverser.Reverse(seed.NextSeed(_generator)).Contains(seed);
        }

        [Property]
        public bool 逆算結果のseedから生成を開始すると元のseedに戻ること(uint seed)
        {
            return _reverser.Reverse(seed).All(_ => _.NextSeed(_generator) == seed);
        }
    }
}
