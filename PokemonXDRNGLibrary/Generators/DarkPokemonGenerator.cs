using System;
using System.Linq;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class DarkPokemonGenerator : IGeneratable<GCIndividual, uint>
    {
        private readonly GCSlot _slot;
        private readonly ILcgConsumer<uint>[] _preGeneratePokemons;

        private static readonly FirstCameraAngleGenerator _angleGenerator = new FirstCameraAngleGenerator();

        public GCIndividual Generate(uint seed, uint playerTSV = 0x10000)
        {
            seed.Advance(_angleGenerator);

            seed.Advance(2); // enemyTSV
            foreach (var p in _preGeneratePokemons)
                seed.Advance(p, playerTSV);

            return _slot.Generate(seed, tsv: playerTSV);
        }

        public DarkPokemonGenerator(GCSlot slot, ILcgConsumer<uint>[] preGeneratePokemons = null)
        {
            _slot = slot;
            _preGeneratePokemons = preGeneratePokemons?.ToArray() ?? Array.Empty<GCSlot>();
        }
    }
}
