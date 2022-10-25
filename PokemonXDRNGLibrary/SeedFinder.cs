using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    static public class SeedFinder
    {
        static readonly List<uint>[] LOWER;

        static SeedFinder()
        {
            LOWER = Enumerable.Range(0, 0x10000).Select(_ => new List<uint>()).ToArray();
            for (uint y = 0; y < 0x10000; y++) LOWER[y.NextSeed() >> 16].Add(y);
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
    }
}
