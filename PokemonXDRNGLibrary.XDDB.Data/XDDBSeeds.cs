using System.IO;
using System.Reflection;

namespace PokemonXDRNGLibrary.XDDB.Data
{
    public static class XDDBSeeds
    {
        public static Stream GetStream()
            => Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "PokemonXDRNGLibrary.XDDB.Data.Resources.seeds.bin");
    }
}
