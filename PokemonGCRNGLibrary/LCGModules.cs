namespace PokemonGCRNGLibrary
{
    public static class LCGModules
    {
        private const uint A = 0x343FD;
        private const uint B = 0x269EC3;
        private const uint C = 0xB9B33155;
        private const uint D = 0xA170F641;

        public static uint GetLCGSeed(uint InitialSeed, uint FirstFrame) { return InitialSeed.Advance(FirstFrame); }
        public static uint NextSeed(this uint seed) { return seed.Advance(); }
        public static uint NextSeed(this uint seed, uint n) { return seed.Advance(n); }
        public static uint PrevSeed(this uint seed) { return seed.Back(); }
        public static uint PrevSeed(this uint seed, uint n) { return seed.Back(n); }
        public static uint Advance(ref this uint seed) { return seed = seed * A + B; }
        public static uint Advance(ref this uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * At[i] + Bt[i];

            return seed;
        }
        public static uint Back(ref this uint seed) { return seed = seed * C + D; }
        public static uint Back(ref this uint seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * Ct[i] + Dt[i];

            return seed;
        }
        public static uint GetRand(ref this uint seed) { return seed.Advance() >> 16; }
        public static uint GetRand(ref this uint seed, uint modulo) { return (seed.Advance() >> 16) % modulo; }

        public static uint GetIndex(this uint seed) { return CalcIndex(seed, A, B, 32); }
        public static uint GetIndex(this uint seed, uint InitialSeed) { return GetIndex(seed) - GetIndex(InitialSeed); }
        private static uint CalcIndex(uint seed, uint A, uint B, uint order)
        {
            if (order == 0) return 0;
            else if ((seed & 1) == 0) return CalcIndex(seed / 2, A * A, (A + 1) * B / 2, order - 1) * 2;
            else return CalcIndex((A * seed + B) / 2, A * A, (A + 1) * B / 2, order - 1) * 2 - 1;
        }

        private static readonly uint[] At;
        private static readonly uint[] Bt;
        private static readonly uint[] Ct;
        private static readonly uint[] Dt;
        static LCGModules()
        {
            At = new uint[32]; Bt = new uint[32]; Ct = new uint[32]; Dt = new uint[32];
            At[0] = A;
            Bt[0] = B;
            Ct[0] = C;
            Dt[0] = D;
            for (int i = 1; i < 32; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }
        }
    }
}
