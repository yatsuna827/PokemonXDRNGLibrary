using System.Collections.Generic;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public interface ISeedEnumeratorHandler
    {
        uint Initialize(uint seed);
        uint SelectCurrent(uint seed);
        uint Advance(uint seed);
    }
    public interface IActionSequenceEnumeratorHandler
    {
        uint Initialize(uint seed);
        uint SelectCurrent(uint seed);
        bool RollForAction(ref uint seed);
    }

    public static class SeedEnumeratorExtension
    {
        private static IEnumerable<uint> EnumerateSeedNotNull(this uint seed, ISeedEnumeratorHandler handler)
        {
            seed = handler.Initialize(seed);
            while (true)
            {
                yield return handler.SelectCurrent(seed);
                seed = handler.Advance(seed);
            }
        }
        public static IEnumerable<uint> EnumerateSeed(this uint seed, ISeedEnumeratorHandler handler)
            => handler is null ? seed.EnumerateSeed() : seed.EnumerateSeedNotNull(handler);

        public static IEnumerable<(uint Seed, int Frame, int Interval)> EnumerateActionSequence(this uint seed, IActionSequenceEnumeratorHandler handler)
        {
            var lastBlinkedFrame = 0;
            var currentFrame = 0;

            seed = handler.Initialize(seed);
            while (true)
            {
                currentFrame++;
                if (handler.RollForAction(ref seed))
                {
                    yield return (handler.SelectCurrent(seed), currentFrame, currentFrame - lastBlinkedFrame);
                    lastBlinkedFrame = currentFrame;
                }
            }
        }

    }
}
