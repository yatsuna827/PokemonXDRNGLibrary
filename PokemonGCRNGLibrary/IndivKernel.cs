using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class IndivKernel
    {
        public uint Lv;
        public uint PID;
        public uint[] IVs;
        internal IndivKernel(uint PID, uint[] IVs, uint Lv = 50) { this.Lv = Lv; this.PID = PID; this.IVs = IVs; }
    }

    public class GCIndivKernel : IndivKernel
    {
        public uint XDAbilityIndex;
        internal GCIndivKernel(uint PID, uint[] IVs, uint XDAbilityIndex, uint Lv=50) : base(PID, IVs, Lv)
        {
            this.XDAbilityIndex = XDAbilityIndex;
        }
    }
}
