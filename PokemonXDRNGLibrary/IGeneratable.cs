using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    interface IGeneratable<TResult>
    {
        TResult Generate(uint seed);
        TResult Generate(uint seed, out uint finishingSeed);
    }
}
