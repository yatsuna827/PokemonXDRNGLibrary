using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class Result
    {
        public GCIndividual Individual { get; internal set; }

        public uint StartingSeed { get; internal set; }
        public uint FinishingSeed { get; internal set; }
        public uint DummyTSV { get; internal set; }
        public bool AvoidedShiny { get; internal set; }
    }

}
