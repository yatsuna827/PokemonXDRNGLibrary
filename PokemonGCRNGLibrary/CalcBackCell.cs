using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    class CalcBackCell
    {
        public readonly uint seed;
        public readonly uint TSV;
        public readonly List<uint> preGeneratedPSVList;

        public (uint seed, uint TSV, bool flag) GetGeneratableSeed()
        {
            var seed = this.seed;
            var tsv = (seed >> 16) ^ (seed.Back() >> 16);

            if (TSV != 0x10000 && (TSV ^ tsv) >= 8) return (seed.PrevSeed(), tsv, false); // TSV条件があり, それを満たさない
            if (preGeneratedPSVList.Any(_ => (_ ^ tsv) < 8)) return (seed.PrevSeed(), tsv, false); // 色回避を発生させてしまう.

            return (seed.PrevSeed(), tsv, true);
        }
        public CalcBackCell(uint seed, uint tsv = 0x10000) { this.seed = seed; TSV = tsv; preGeneratedPSVList = new List<uint>(); }
        public CalcBackCell(uint seed, uint tsv, List<uint> pidList) { this.seed = seed; TSV = tsv; preGeneratedPSVList = pidList; }
    }
}
