using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    static class LCGUtils
    {
        public static uint BackRand(ref this uint seed)
        {
            var rand = seed >> 16;
            seed.Back();

            return rand;
        }
        public static uint BackRand(ref this uint seed, uint m)
        {
            var rand = seed >> 16;
            seed.Back();

            return rand % m;
        }
    }
}
