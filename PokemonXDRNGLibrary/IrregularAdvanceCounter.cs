using System;
using System.Collections.Generic;
using System.Linq;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    class MainCounter
    {
        private float value;
        private readonly SubCounter subCounter;
        private uint nextHeader;
        private const float INITIAL_VALUE = 0.9999999f;

        public MainCounter(float init = INITIAL_VALUE)
        {
            value = init;
            subCounter = new SubCounter();
        }

        public void CountUp(ref uint seed)
        {
            value += seed.GetRand_f() * 0.003f;
            if (value >= 1.0f)
            {
                value -= 1.0f;
                seed.Advance();
                subCounter.Initialize();
            }

            if (subCounter.CountUp(ref seed)) nextHeader = 1; else nextHeader = 0;
        }

        public uint HeaderAdvance() { return nextHeader; }
        public uint LazyAdvance1() { return subCounter.LazyAdvance1(); }
        public uint LazyAdvance2() { return subCounter.LazyAdvance2(); }

    }
    class SubCounter
    {
        private float value;
        private int Life;
        private readonly Queue<uint> lazyAdvance1, lazyAdvance2;
        private const float INITIAL_VALUE = 0.9999999f;
        public SubCounter()
        {
            value = INITIAL_VALUE;
            lazyAdvance1 = new Queue<uint>(Enumerable.Repeat(0, 31).Select(_ => (uint)_));
            lazyAdvance2 = new Queue<uint>(Enumerable.Repeat(0, 41).Select(_ => (uint)_));
        }

        private int counter;
        public bool CountUp(ref uint seed)
        {
            if (Life == 0)
            {
                lazyAdvance1.Enqueue(0);
                lazyAdvance2.Enqueue(0);

                return false;
            }

            value += seed.GetRand_f() * (counter++ < 100 ? 0.5f : 0f);
            if (value >= 1.0f)
            {
                value -= 1.0f;
                Life = 100;
                seed.Advance(6);
                lazyAdvance1.Enqueue(3);
                lazyAdvance2.Enqueue(1);
                return true;
            }

            Life--;
            lazyAdvance1.Enqueue(0);
            lazyAdvance2.Enqueue(0);

            return false;
        }

        public uint LazyAdvance1() { return lazyAdvance1.Dequeue(); }
        public uint LazyAdvance2() { return lazyAdvance2.Dequeue(); }

        public void Initialize()
        {
            Life = 100;
            value = INITIAL_VALUE;
            counter = 0;
        }
    }

    internal delegate void RefAction<T>(ref T seed);
    public class SmokeCalculator
    {
        private readonly Counter root;
        private readonly Counter[] mainCounters;
        private readonly Counter[] subCounters;

        public SmokeCalculator()
        {
            mainCounters = new Counter[2]
            {
                new Counter(0.01f),
                new Counter(0.01f)
            };
            subCounters = new Counter[5]
            {
                new Counter(0.5f),
                new Counter(0.5f),
                new Counter(0.5f),
                new Counter(0.5f),
                new Counter(0.5f)
            };

            mainCounters[0].OnCarry = (ref uint seed) =>
            {
                subCounters[0].Initialize(seed.GetRand_f(), 0.5f, 50);
                mainCounters[0].Connect(subCounters[0]);

                subCounters[1].PrepTime = 10;
            };
            mainCounters[1].OnCarry = (ref uint seed) =>
            {
                subCounters[2].Initialize(seed.GetRand_f(), 0.5f, 50);
                mainCounters[1].Connect(subCounters[2]);

                subCounters[3].PrepTime = 20;
                subCounters[4].PrepTime = 30;
            };

            subCounters[0].OnCountUp = (ref uint seed) =>
            {
                if (subCounters[1].PrepTime > 0) subCounters[1].PrepTime--;
            };
            subCounters[2].OnCountUp = (ref uint seed) =>
            {
                if (subCounters[3].PrepTime > 0) subCounters[3].PrepTime--;
                if (subCounters[4].PrepTime > 0) subCounters[4].PrepTime--;
            };

            root = mainCounters[0];
            root.Next = mainCounters[1];
        }

        public void CountUp(ref uint seed)
        {
            var cur = root;
            if (subCounters[1].PrepTime == 0)
            {
                subCounters[1].Initialize(seed.GetRand_f(), 0.5f, 50);
                subCounters[1].PrepTime = -1;
                root.Connect(subCounters[1]);
            }
            if (subCounters[3].PrepTime == 0)
            {
                subCounters[3].Initialize(seed.GetRand_f(), 0.5f, 50);
                subCounters[3].PrepTime = -1;
                root.Connect(subCounters[3]);
            }
            if (subCounters[4].PrepTime == 0)
            {
                subCounters[4].Initialize(seed.GetRand_f(), 0.5f, 50);
                subCounters[4].PrepTime = -1;
                root.Connect(subCounters[4]);
            }

            while (cur != null)
            {
                cur.CountUp(ref seed);
                cur = cur.Next;
            }
        }

        class Counter
        {
            public Counter Next;
            public float Value;
            public float Coefficient;
            public int Life = -1;
            public int SmokeLife = 0;
            public int PrepTime = -1;
            public RefAction<uint> OnCarry;
            public RefAction<uint> OnCountUp;
            public Counter(float coef, float initValue = 0.9999999f)
            {
                Coefficient = coef;
                Value = initValue;
            }
            public void Initialize(float value, float coef, int life)
            {
                Value = value;
                Coefficient = coef;
                Life = life;
            }
            public void Connect(Counter c)
            {
                var temp = Next;
                Next = c;
                c.Next = temp;
            }
            public void CountUp(ref uint seed)
            {
                Value += seed.GetRand_f() * Coefficient;
                OnCountUp?.Invoke(ref seed);
                if (Value >= 1.0f)
                {
                    Value -= 1.0f;
                    seed.Advance();

                    if (Life != -1) SmokeLife = 21;

                    OnCarry?.Invoke(ref seed);
                }
                if (Life > 0) Life--;
                if (Life == 0) Coefficient = 0.0f;
                if (SmokeLife > 0) SmokeLife--;

                while (Next != null && Next.Life == 0 && Next.SmokeLife == 0) Next = Next.Next;
            }
        }
    }

    public class StandCounter
    {
        private float value;
        public StandCounter(ref uint seed)
        {
            value = seed.GetRand_f();
        }
        public void CountUp(ref uint seed)
        {
            value += seed.GetRand_f() * 0.8f;
            if (value >= 1.0f)
            {
                value -= 1.0f;
                seed.Advance();
            }
        }
        public float GetCounter() { return value; }
    }
}
