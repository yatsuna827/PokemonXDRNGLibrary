using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonXDRNGLibrary.Algorithm
{
    public static class SearchSubArray
    {
        /// <summary>
        /// tl から、連続する和がblanksに一致する部分列 [l_0, r_0), [l_1=r_0, r_1), ..., [l_n, r_n) を探索し、終端のr_0の値を列挙します。
        /// </summary>
        /// <param name="blanks"></param>
        /// <param name="tl"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static IEnumerable<int> SearchSubArrayWithError(this int[] blanks, int[] tl, int error)
        {
            var ss = tl.ShakutoriWithError(blanks.First(), error).ToArray();
            var q = new Queue<(int, int)>(ss.Select(_ => (_, 1)));
            while (q.Count > 0)
            {
                var (l, idx) = q.Dequeue();

                var seg = blanks[idx];
                for (int r = l, sum = 0; r < tl.Length; sum += tl[r++])
                {
                    if (seg - error <= sum && sum <= seg + error)
                    {
                        if (idx == blanks.Length - 1)
                            yield return r;
                        else
                            q.Enqueue((r, idx + 1));
                    }

                    if (sum + tl[r] > seg + error) break;
                }
            }
        }

        /// <summary>
        /// tl から 合計が n ± error になるような半開区間 [l, r) を探索し、r の値を列挙します
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="n"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static IEnumerable<int> ShakutoriWithError(this int[] tl, int n, int error)
        {
            var r = 0;
            var sum = 0;
            for (int i = 0; i < tl.Length; i++)
            {
                while (r < tl.Length && sum + tl[r] <= n + error)
                {
                    sum += tl[r];
                    r++;

                    if (n - error <= sum && sum <= n + error) yield return r;
                }

                if (r == i) r++;
                else sum -= tl[i];
            }
        }
    }
}
