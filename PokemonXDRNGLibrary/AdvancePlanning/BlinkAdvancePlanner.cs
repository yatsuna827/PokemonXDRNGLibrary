using System.Linq;
using System.Collections.Generic;

using PokemonPRNG.LCG32;
using PokemonXDRNGLibrary.AdvanceSource;

namespace PokemonXDRNGLibrary.AdvancePlanning
{
    public class BlinkAdvancePlanner
    {
        private readonly BlinkObjectEnumeratorHanlder _blinkHandler;
        private readonly ISeedEnumeratorHandler _handler;

        public BlinkAdvancePlanner(BlinkObject blinkObject, ISeedEnumeratorHandler seedEnumeratorHandler)
        {
            _blinkHandler = new BlinkObjectEnumeratorHanlder(blinkObject);
            _handler = seedEnumeratorHandler;
        }

        public IEnumerable<((int Frame, uint Seed) Blink, (int Frame, uint Seed) SecondAdvance)> CalculatePlanning(
            uint current,
            uint target,
            uint minInterval,
            uint maxInterval,
            int minBlinkFrames,
            int maxBlinkFrames,
            int minFrames,
            int maxFrames
        )
            => current.EnumerateActionSequence(_blinkHandler)
                .SkipWhile(_ => minBlinkFrames <= _.Frame)
                .TakeWhile(_ => _.Frame <= maxBlinkFrames)
                .Where(_ => minInterval <= _.Interval && _.Interval <= maxInterval)
                .SelectMany((_) =>
                    _.Seed.EnumerateSeed(_handler).WithIndex()
                        .Skip(minFrames)
                        .Take(maxFrames - minFrames + 1)
                        .Where(__ => __.Element == target)
                        .Select(__ => ((_.Frame, _.Seed), __))
                );
    }
}
