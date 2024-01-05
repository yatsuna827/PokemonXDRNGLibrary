using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class FirstCameraAngleGenerator : IGeneratable<uint>, IGeneratableEffectful<uint>, ILcgConsumer
    {
        private readonly uint _flags;
        public FirstCameraAngleGenerator(uint previousAngle = 12, uint twoPreviousAngle = 12)
        {
            _flags = (1u << 3);
            if (previousAngle < 12)
                _flags |= (1u << (int)previousAngle);
            if (twoPreviousAngle < 12)
                _flags |= (1u << (int)twoPreviousAngle);
        }

        public void Use(ref uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if ((_flags & (1 << (int)r)) == 0)
                {
                    if (r == 8) seed.Advance(5); 
                    else if (r < 5) seed.Advance(); 
                    else seed.Advance(2);

                    break;
                }
            }
        }
        public uint ComputeConsumption(uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if ((_flags & (1 << (int)r)) == 0)
                {
                    if (r == 8) seed.Advance(5);
                    else if (r < 5) seed.Advance();
                    else seed.Advance(2);

                    return seed;
                }
            }
        }

        public uint Generate(uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if ((_flags & (1 << (int)r)) == 0) return r;
            }
        }

        public uint Generate(ref uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if ((_flags & (1 << (int)r)) == 0)
                {
                    if (r == 8) seed.Advance(5);
                    else if (r < 5) seed.Advance();
                    else seed.Advance(2);

                    return r;
                }
            }
        }

    }
}
