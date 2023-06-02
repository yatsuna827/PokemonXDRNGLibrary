using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;
using PokemonXDRNGLibrary.Algorithm;

namespace PokemonXDRNGLibrary
{
    static public class SeedFinder//(Co->XD) オプショナル引数も含めてそれらしきマジックナンバーを調整しています.
    {
        static readonly List<uint>[] LOWER;
        private static readonly int[] minBlinkableBlank;　// randに対し, 「到達したときに瞬きをするような内部カウンタの値」の最小値.(Co->XD)瞬きからの特定処理用の変数の追加

namespace PokemonXDRNGLibrary
{
    static public class SeedFinder
    {
        static readonly List<uint>[] LOWER;

        static SeedFinder()
        {
            LOWER = Enumerable.Range(0, 0x10000).Select(_ => new List<uint>()).ToArray();
            for (uint y = 0; y < 0x10000; y++) LOWER[y.NextSeed() >> 16].Add(y);

            minBlinkableBlank = Enumerable.Range(0, 0x10000).Select(_ => BlinkConst.blinkThresholds_even.UpperBound((uint)_)).ToArray(); //(Co->XD)瞬きからの特定処理用の変数の追加
        }

        /// <summary>
        /// 指定した個体値の個体を生成するseedを返す. 
        /// </summary>
        public static IReadOnlyList<uint> FindGeneratingSeed(uint H, uint A, uint B, uint C, uint D, uint S, bool generateEnemyTSV = true)
        {
            var offset = generateEnemyTSV ? 5u : 3u;

            var HAB = H | (A << 5) | (B << 10);
            var SCD = S | (C << 5) | (D << 10);

            uint key = (SCD - (0x43FDU * HAB)) & 0xFFFF;
            var resList = new List<uint>();
            foreach (var low16 in LOWER[key])
            {
                uint seed = ((HAB << 16) | low16).PrevSeed(offset);
                resList.Add(seed);
                resList.Add(seed ^ 0x80000000);
            }
            foreach (var low16 in LOWER[key ^ 0x8000])
            {
                uint seed = ((HAB << 16) | low16).PrevSeed(offset);
                resList.Add(seed);
                resList.Add(seed ^ 0x80000000);
            }

            return resList;
        }

        /// <summary>
        /// 瞬きから現在seedを検索し, 条件に一致するものを返します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="minIndex"></param>
        /// <param name="maxIndex"></param>
        /// <param name="blinkInput"></param>
        /// <param name="allowanceLimitOfError"></param>
        /// <param name="coolTime"></param>
        /// <returns></returns>
        public static IEnumerable<uint> FindCurrentSeedByBlink(uint seed, uint minIndex, uint maxIndex, int[] blinkInput, int allowanceLimitOfError = 40, int coolTime = 8)//(Co->XD)メソッドの追加/マジックナンバーを2倍に
        {
            var res = new List<uint>();

            seed.Advance(minIndex);
            blinkInput = blinkInput.Select(_ => _ - (10 + coolTime)).ToArray(); // 瞬き後のクールタイム分を引く. (Co->XD)マジックナンバーの更新

            var n = maxIndex - minIndex + 1;

            // i = maxまで計算できるように, 全て最大間隔で瞬きが行われた場合でも足りるだけ余分に計算しておく.
            var len = (int)n + 170 * (blinkInput.Length + 1); //(Co->XD)マジックナンバーを2倍にしつつ(blinkInput.Length + 2)を(blinkInput.Length + 1)へ

            // 「到達したときに瞬きをするような内部カウンタの値」の最小値に変換する.
            // UpperBoundしていたところは入力が高々65536通りしかないのであらかじめ計算しておくことでO(1)に落とせる.
            // とはいえそこまで劇的に変わるわけではない(UpperBoundの計算量は十分定数に近いので).
            var countList = seed.EnumerateRand().Take(len + 1).Select(_ => minBlinkableBlank[_]).ToArray();

            // 「その位置で瞬きをした場合の, 次の瞬きまでの間隔」.
            // ここが定数倍大きいのでちょっと辛い.
            var blankList = Enumerable.Repeat(171, len).ToArray(); // 171 = INF (Co->XD)マジックナンバーの更新
            for (int i = len - 1; i >= 0; i--) // 後ろから埋めていく. 確率的にcountListは大きい値が多く, 特に85になる確率が高い.
                for (int k = countList[i]; k <= Math.Min(i, 170); k++) // kは『到達したときに瞬きするような内部カウンタの最小値』から170まで(境界を超えないように). (Co->XD)マジックナンバーを2倍に
                    blankList[i - k] = Math.Min(blankList[i - k], k); // kの定義より, i-kで瞬きをしたら, 次は少なくともiで瞬きが発生する.

            // 『iフレーム目に1回目の瞬きが行われた』と仮定してシミュレート.
            for (int i = 0; i < n; i++)
            {
                int idx = i;
                int k;
                for (k = 0; k < blinkInput.Length; k++)
                {
                    // 許容誤差を超えているなら次のフレームへ.
                    if ((blinkInput[k] + allowanceLimitOfError) < blankList[idx] || blankList[idx] < (blinkInput[k] - allowanceLimitOfError)) break;

                    // 間隔ぶんをindexに加算する.
                    idx += blankList[idx] + 1;
                }

                // 入力と全て一致すればresに入れる.
                if (k == blinkInput.Length) res.Add((uint)idx);
            }

            return res.Distinct().Select(_ => seed.NextSeed(_));
        }

        /// <summary>
        /// 戦闘終了時に瞬きから現在seedを検索し, 条件に一致するものを返します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="minIndex"></param>
        /// <param name="maxIndex"></param>
        /// <param name="blinkInput"></param>
        /// <param name="allowanceLimitOfError"></param>
        /// <param name="coolTime"></param>
        /// <returns></returns>
        public static IEnumerable<uint> FindCurrentSeedByBlinkInBattle(uint seed, uint maxFrame, int[] blinkInput, IActionSequenceEnumeratorHandler handler, int allowanceLimitOfError = 40)//(Co->XD)メソッドの追加/マジックナンバーを2倍に
        {
            var source = seed.EnumerateActionSequence(handler).TakeWhile(_ => _.Frame <= maxFrame);
            var timeline = source.Select(_ => _.Interval).ToArray();
            var seeds = source.Select(_ => _.Seed).ToArray();

            return blinkInput.SearchSubArrayWithError(timeline, allowanceLimitOfError).Distinct().Select(_ => seeds[_ - 1]);
        }

        /// <summary>
        /// 瞬きから現在seedを検索し, 条件に一致するものを返します.
        /// 省メモリかつ高速ですが, 瞬きを数回捨てる必要があります.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="minIndex"></param>
        /// <param name="maxIndex"></param>
        /// <param name="blinkInput"></param>
        /// <param name="allowanceLimitOfError"></param>
        /// <param name="coolTime"></param>
        /// <returns></returns>
        public static IEnumerable<uint> FindCurrentSeedByBlinkFaster(uint seed, uint minIndex, uint maxIndex, int[] blinkInput, int allowanceLimitOfError = 40, int coolTime = 8)//(Co->XD)メソッドの追加/マジックナンバーを2倍に
        {
            seed.Advance(minIndex);

            var n = (ulong)maxIndex - minIndex + 1;

            var e = seed.EnumerateBlinkingSeed(coolTime).GetEnumerator();
            var blinkCache = new (int, uint)[256]; // 瞬き間隔をキャッシュしておく配列.

            for (int i = 0; i < blinkInput.Length; i++)
            {
                e.MoveNext();
                blinkCache[i] = (e.Current.interval, e.Current.lcgIndex);
            }

            bool check(int k)
            {
                for (int i = 0; i < blinkInput.Length; i++)
                {
                    var b = blinkCache[(k + i) & 0xFF].Item1;
                    if (blinkInput[i] + allowanceLimitOfError < b || b < blinkInput[i] - allowanceLimitOfError) return false;
                }

                return true;
            };
            int head = 0, tail = blinkInput.Length;
            do
            {
                if (check(head++)) yield return e.Current.seed;
                if (!e.MoveNext()) yield break;
                blinkCache[tail++ & 0xFF] = (e.Current.interval, e.Current.lcgIndex);
            }
            while (blinkCache[head & 0xFF].Item2 <= n);
        }

        /// <summary>
        /// 全範囲から検索します.
        /// Fasterと同様に瞬きを数回捨てる必要があります. 使う意味は薄いです.
        /// </summary>
        /// <param name="blinkInput"></param>
        /// <param name="allowanceLimitOfError"></param>
        /// <param name="coolTime"></param>
        /// <returns></returns>
        public static IEnumerable<uint> FindCurrentSeedByBlink(int[] blinkInput, int allowanceLimitOfError = 40, int coolTime = 8)//メソッドの追加/(Co->XD)マジックナンバーを2倍に
        {
            var e = 0u.EnumerateBlinkingSeed(coolTime).GetEnumerator();
            var blinkCache = new int[256]; // 瞬き間隔をキャッシュしておく配列.

            for (int i = 0; i < blinkInput.Length; i++)
            {
                e.MoveNext();
                blinkCache[i] = e.Current.interval;
            }

            bool check(int k)
            {
                for (int i = 0; i < blinkInput.Length; i++)
                {
                    var b = blinkCache[(k + i) & 0xFF];
                    if (blinkInput[i] + allowanceLimitOfError < b || b < blinkInput[i] - allowanceLimitOfError) return false;
                }

                return true;
            };
            int tail = blinkInput.Length;
            for (int head = 0; e.MoveNext(); head++, blinkCache[tail++ & 0xFF] = e.Current.interval)
            {
                if (!check(head)) continue;
                yield return e.Current.seed;
            }
        }
    }
}
