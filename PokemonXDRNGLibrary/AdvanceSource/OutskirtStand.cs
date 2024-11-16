using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary.AdvanceSource
{
    // 町外れのスタンド屋内の不定消費を計算します
    // 処理はCoのパイラの洞窟と大体同じですが、遅延サブカウンタの生え方が逆になっています
    // また、Coと異なり、マップロード時の消費が割り込みます

    public class OutskirtStandCounter
    {
        private readonly MainCounter[] mainCounters;
        private readonly Queue<CountDownValue> lazyGenerators;
        public OutskirtStandCounter()
        {
            lazyGenerators = new Queue<CountDownValue>();

            void OnCarriedRight()
            {
                lazyGenerators.Enqueue(CountDownValue.CreateValue(20));
                lazyGenerators.Enqueue(CountDownValue.CreateValue(30));
            }
            void OnCarriedLeft()
            {
                lazyGenerators.Enqueue(CountDownValue.CreateValue(10));
            }
            mainCounters = new MainCounter[2]
            {
                new MainCounter(OnCarriedLeft),
                new MainCounter(OnCarriedRight),
            };
        }

        public void CountUp(ref uint seed)
        {
            var c = lazyGenerators.Count; // for内でEnqueueするので、外側で代入しておく必要がある.
            for (int i = 0; i < c; i++)
            {
                var value = lazyGenerators.Dequeue().CountDown();
                if (value.IsZero)
                    mainCounters[0].InterruptChild(new SubCounter(ref seed));
                else
                    lazyGenerators.Enqueue(value);
            }

            foreach (var counter in mainCounters)
                counter.CountUp(ref seed);
        }

        abstract class MapObjectCounter
        {
            public float Value { get; protected set; }
            protected readonly float coefficient;
            protected SubCounter child;

            private const float INITIAL_VALUE = 0.9999999f;
            public MapObjectCounter(float coef, float init = INITIAL_VALUE)
            {
                this.Value = init;
                this.coefficient = coef;
            }

            /// <summary>
            /// カウンタの加算処理を呼びます.
            /// そのまま子カウンタの加算処理も呼び出します.
            /// </summary>
            /// <param name="seed"></param>
            public abstract void CountUp(ref uint seed);

            /// <summary>
            /// 子カウンタを挟みこみます.
            /// </summary>
            /// <param name="newChild"></param>
            public void InterruptChild(SubCounter newChild)
            {
                newChild.child = this.child;
                this.child = newChild;
            }

            /// <summary>
            /// 生きている子カウンタ or nullに行きつくまで子カウンタを退ける.
            /// </summary>
            public void RemoveDeadChilds()
            {
                while (child != null && !child.IsLiving) this.child = child.child;
            }
        }

        class MainCounter : MapObjectCounter
        {
            private readonly Action onCarried;

            public override void CountUp(ref uint seed)
            {
                Value += seed.GetRand_f() * coefficient; // メインカウンタは死ぬことは無い.
                if (Value >= 1.0f)
                {
                    Value -= 1.0f;
                    seed.Advance();
                    InterruptChild(new SubCounter(ref seed));
                    onCarried?.Invoke();
                }

                RemoveDeadChilds();

                child?.CountUp(ref seed);
            }

            public void SimulateCountUp(ref uint seed)
            {
                var v = Value + seed.GetRand_f() * coefficient;
                if (v >= 1.0f)
                {
                    seed.Advance();
                    // サブカウンタが生える.
                    // 初期化 + 直後に加算処理が入り, 繰り上げが発生したらさらに1消費.
                    if (seed.GetRand_f() + seed.GetRand_f() * 0.5f >= 1.0f) seed.Advance();
                }

                child?.SimulateCountUp(ref seed);
            }

            public void SimulateCountUp(ref uint seed, SubCounter[] subordinateCounters)
            {
                var v = Value + seed.GetRand_f() * coefficient;
                if (v >= 1.0f)
                {
                    seed.Advance();
                    // サブカウンタが生える.
                    // 初期化 + 直後に加算処理が入り, 繰り上げが発生したらさらに1消費.
                    if (seed.GetRand_f() + seed.GetRand_f() * 0.5f >= 1.0f) seed.Advance();
                }

                // 遅延で生えたカウンタの加算処理.
                foreach (var sub in subordinateCounters)
                    sub.SimulateCountUp(ref seed);

                child?.SimulateCountUp(ref seed);
            }

            public MainCounter(Action onCarried) : base(0.01f) => this.onCarried = onCarried;
        }

        class SubCounter : MapObjectCounter
        {
            protected int lifetime = -1;
            protected int objectLifetime = 0;
            public bool IsLiving => (lifetime > 0 || objectLifetime > 0);
            public override void CountUp(ref uint seed)
            {
                // lifetimeが0になっているなら加算は行わない.
                if (lifetime == 0) seed.Advance();
                else
                {
                    Value += seed.GetRand_f() * coefficient;
                    if (Value >= 1.0f)
                    {
                        Value -= 1.0f;
                        seed.Advance();
                        objectLifetime = 21;
                    }
                    lifetime--;
                }

                objectLifetime--;

                RemoveDeadChilds();

                child?.CountUp(ref seed);
            }

            public void SimulateCountUp(ref uint seed)
            {
                if (IsLiving)
                {
                    if (lifetime == 0)
                        seed.Advance();
                    else if (Value + seed.GetRand_f() * coefficient >= 1.0f)
                        seed.Advance();
                }

                child?.SimulateCountUp(ref seed);
            }

            public SubCounter(ref uint seed) : base(0.5f, seed.GetRand_f()) => lifetime = 50;
        }
    
        public override string ToString()
        {
            return $"{mainCounters[0].Value} {mainCounters[1].Value}";
        }
    }

    public class OutskirtStand : ISeedEnumeratorHandler
    {
        private uint _frames;
        private OutskirtStandCounter _counter;

        public uint Initialize(uint seed)
        {
            _frames = 0;
            _counter = new OutskirtStandCounter();
            return seed;
        }

        public uint SelectCurrent(uint seed) => seed;

        public uint Advance(uint seed)
        {
            // マップロード消費が決まったタイミングで割り込む
            if (_frames < 6)
            {
                if (_frames == 2) seed.Advance(2);
                if (_frames == 5) seed.Advance(8);
            }

            _frames++;
            
            _counter.CountUp(ref seed);
            return seed;
        }
    }
}
