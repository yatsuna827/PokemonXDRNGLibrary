using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public static class XDStarter
    {
        private static readonly GCSlot EEVEE = new GCSlot("イーブイ", 10);
        public static XDStarterResult GenerateStarter(uint seed)
        {
            seed.Advance(1000);
            uint TID = seed.GetRand();
            uint SID = seed.GetRand();
            var e = EEVEE.Generate(seed, out seed);

            return new XDStarterResult(TID, SID, e);
        }
        public static XDStarterResult GenerateStarter(uint seed, XDStarterCriteria criteria)
        {
            seed.Advance(1000);
            uint TID = seed.GetRand();
            if (criteria.CheckTID && !criteria.targetTID.Contains(TID)) return XDStarterResult.Empty;
            uint SID = seed.GetRand();
            if (criteria.CheckTSV && !criteria.targetTSV.Contains((TID ^ SID) >> 3)) return XDStarterResult.Empty;

            criteria.TSV = TID ^ SID;
            var e = EEVEE.Generate(seed, 0x10000, criteria);
            if (e == GCIndividual.Empty) return XDStarterResult.Empty;

            return new XDStarterResult(TID, SID, e);
        }

        public class XDStarterResult
        {
            public uint TID;
            public uint SID;
            public GCIndividual Eevee;

            public static readonly XDStarterResult Empty = new XDStarterResult(0x10000, 0x10000, GCIndividual.Empty);

            internal XDStarterResult(uint tid, uint sid, GCIndividual eevee)
            {
                TID = tid;
                SID = sid;
                Eevee = eevee;
            }
        }
    }
}
