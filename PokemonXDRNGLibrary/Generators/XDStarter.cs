using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class XDStarter : IGeneratable<XDStarterResult>
    {
        private static readonly IGeneratableEffectful<GCIndividual> EEVEE = (new GCSlot("イーブイ", 10) as IGeneratableEffectful<GCIndividual, uint>).Apply(0x10000u);
        public XDStarterResult Generate(uint seed)
        {
            seed.Advance(1000);
            var tid = seed.GetRand();
            var sid = seed.GetRand();
            var eevee = seed.Generate(EEVEE);

            return new XDStarterResult(tid, sid, eevee);
        }
    }
    public class XDStarterResult
    {
        public uint TID { get; }
        public uint SID { get; }
        public GCIndividual Eevee { get; }

        internal XDStarterResult(uint tid, uint sid, GCIndividual eevee)
        {
            TID = tid;
            SID = sid;
            Eevee = eevee;
        }
    }
}
