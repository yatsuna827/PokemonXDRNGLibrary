using System;
using System.Collections.Generic;
using System.Text;

using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class FirstAngleReverser
    {
        public IEnumerable<uint> Reverse(uint currentSeed)
        {
            {
                var seed = currentSeed.PrevSeed();
                var r = seed.BackRand(10);
                if (r < 5 && r != 3)
                {
                    yield return seed;
                    while (seed.BackRand(10) == 3)
                        yield return seed;
                }
            }

            {
                var seed = currentSeed.PrevSeed(2);
                var r = seed.BackRand(10);
                if (r >= 5 && r != 8)
                {
                    yield return seed;
                    while (seed.BackRand(10) == 3)
                        yield return seed;
                }
            }

            {
                var seed = currentSeed.PrevSeed(5);
                var r = seed.BackRand(10);
                if (r == 8)
                {
                    yield return seed;
                    while (seed.BackRand(10) == 3)
                        yield return seed;
                }
            }
        }

        public IEnumerable<uint> Reverse(uint currentSeed, AngleHistory history)
        {
            {
                var seed = currentSeed.PrevSeed();
                var r = seed.BackRand(10);
                if (r < 5 && r != 3 && !history.Includes(r))
                {
                    do
                    {
                        yield return seed;
                        r = seed.BackRand(10);
                    }
                    while (r == 3 || history.Includes(r));
                }
            }

            {
                var seed = currentSeed.PrevSeed(2);
                var r = seed.BackRand(10);
                if (r >= 5 && r != 8 && !history.Includes(r))
                {
                    do
                    {
                        yield return seed;
                        r = seed.BackRand(10);
                    }
                    while (r == 3 || history.Includes(r));
                }
            }

            {
                var seed = currentSeed.PrevSeed(5);
                var r = seed.BackRand(10);
                if (r == 8 && !history.Includes(r))
                {
                    do
                    {
                        yield return seed;
                        r = seed.BackRand(10);
                    }
                    while (r == 3 || history.Includes(r));
                }
            }
        }
    }

}
