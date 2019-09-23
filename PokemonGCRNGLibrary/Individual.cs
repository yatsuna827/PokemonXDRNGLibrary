using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class Individual
    {
        public string Name;
        public uint Lv;
        public uint PID;
        public Nature Nature;
        public Gender Gender;
        public string Ability;
        public string XDAbility;
        public uint[] IVs;
        public uint[] Stats;
        public bool isShiny(uint TSV) { return (TSV ^ (PID & 0xFFFF) ^ (PID >> 16)) < 8; }
        internal Individual() { }
    }

}
