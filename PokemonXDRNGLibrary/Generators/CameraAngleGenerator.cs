using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class FirstCameraAngleGenerator : 
        IGeneratable<uint>, IGeneratableEffectful<uint>, ILcgConsumer,
        IGeneratable<uint, AngleHistory>, IGeneratableEffectful<uint, AngleHistory>, ILcgConsumer<AngleHistory>
    {
        public void Use(ref uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                break;
            }
        }
        public void Use(ref uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3 || history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                break;
            }
        }
        public uint ComputeConsumption(uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                return seed;
            }
        }
        public uint ComputeConsumption(uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3 || history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                return seed;
            }
        }

        public uint Generate(uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r != 3) return r;
            }
        }
        public uint Generate(uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r != 3 && !history.Includes(r)) return r;
            }
        }
        public uint Generate(ref uint seed)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                return r;
            }
        }
        public uint Generate(ref uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(10);
                if (r == 3 || history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else seed.Advance(2);

                return r;
            }
        }
    }

    public class CameraAngleGenerator : IGeneratable<uint, AngleHistory>, IGeneratableEffectful<uint, AngleHistory>, ILcgConsumer<AngleHistory>
    {
        public void Use(ref uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else if (r > 9)
                {
                    seed.Advance(4);
                    if (seed.GetRand(3) == 0)
                        seed.Advance(2);
                    else
                        seed.Advance(3);
                }
                else seed.Advance(2);

                return;
            }
        }
        public void Use(ref uint seed, ref AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else if (r > 9)
                {
                    seed.Advance(4);
                    if (seed.GetRand(3) == 0)
                        seed.Advance(2);
                    else
                        seed.Advance(3);
                }
                else seed.Advance(2);

                history = history.Next((byte)r);

                return;
            }
        }
        public uint ComputeConsumption(uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else if (r > 9)
                {
                    seed.Advance(4);
                    if (seed.GetRand(3) == 0)
                        seed.Advance(2);
                    else
                        seed.Advance(3);
                }
                else seed.Advance(2);

                return seed;
            }
        }

        public uint Generate(uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (!history.Includes(r)) return r;
            }
        }

        public uint Generate(ref uint seed, AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else if (r > 9)
                {
                    seed.Advance(4);
                    if (seed.GetRand(3) == 0)
                        seed.Advance(2);
                    else
                        seed.Advance(3);
                }
                else seed.Advance(2);

                return r;
            }
        }

        public uint Generate(ref uint seed, ref AngleHistory history)
        {
            while (true)
            {
                var r = seed.GetRand(12);
                if (history.Includes(r)) continue;

                if (r == 8) seed.Advance(5);
                else if (r < 5) seed.Advance();
                else if (r > 9)
                {
                    seed.Advance(4);
                    if (seed.GetRand(3) == 0)
                        seed.Advance(2);
                    else
                        seed.Advance(3);
                }
                else seed.Advance(2);

                history = history.Next((byte)r);

                return r;
            }
        }
    }

    public readonly struct AngleHistory
    {
        private readonly byte _current, _previous;

        public AngleHistory(byte current = 12, byte previous = 12)
            => (_current, _previous) = (current, previous);

        public AngleHistory Next(byte value)
            => new AngleHistory(value, _current);

        public bool Includes(uint value)
            => _current == value || _previous == value;

        public override string ToString()
        {
            return $"{_current} {_previous}";
        }
    }

}
