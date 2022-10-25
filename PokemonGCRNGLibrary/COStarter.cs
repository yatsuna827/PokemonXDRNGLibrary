using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    public class COStarter
    {
        private static readonly GCSlot ESPEON = new GCSlot("エーフィ", 25);
        private static readonly GCSlot UMBREON = new GCSlot("ブラッキー", 25);
        public static COStarterResult GenerateStarter(uint seed)
        {
            seed.Advance(1000);
            uint TID = seed.GetRand();
            uint SID = seed.GetRand();
            var e = ESPEON.Generate(seed, out seed, TID ^ SID);
            var u = UMBREON.Generate(seed, out seed, TID ^ SID);
            
            return new COStarterResult(TID, SID, e, u);
        }
        public static COStarterResult GenerateStarter(uint seed, XDStarterCriteria criteria)
        {
            seed.Advance(1000);
            uint TID = seed.GetRand();
            if (criteria.CheckTID && !criteria.targetTID.Contains(TID)) return COStarterResult.Empty;
            uint SID = seed.GetRand();
            if (criteria.CheckTSV && !criteria.targetTSV.Contains((TID ^ SID) >> 3)) return COStarterResult.Empty;

            criteria.TSV = TID ^ SID;
            var e = ESPEON.Generate(seed, out seed, TID ^ SID);
            if (e == GCIndividual.Empty) return COStarterResult.Empty;
            var u = UMBREON.Generate(seed, out seed, TID ^ SID);

            return new COStarterResult(TID, SID, e, u);
        }
        public class COStarterResult
        {
            public uint TID;
            public uint SID;
            public GCIndividual Espeon, Umbreon;

            public static readonly COStarterResult Empty = new COStarterResult(0x10000, 0x10000, GCIndividual.Empty, GCIndividual.Empty);

            internal COStarterResult(uint tid, uint sid, GCIndividual espeon, GCIndividual umbreon)
            {
                TID = tid;
                SID = sid;
                Espeon = espeon;
                Umbreon = umbreon;
            }
        }
    }
}
