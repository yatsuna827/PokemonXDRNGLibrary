using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    public class TeamGenerator : IGeneratable<GCIndividual[]>
    {
        private readonly GCSlot[] _team;
        private readonly FirstCameraAngleGenerator _angle;
        public TeamGenerator(GCSlot[] team, FirstCameraAngleGenerator angleGenerator)
            => (_team, _angle) = (team.Take(6).ToArray(), angleGenerator);

        public GCIndividual[] Generate(uint seed)
        {
            seed.Used(_angle);

            var dummyTSV = seed.GetRand() ^ seed.GetRand();
            return _team.Select(_ => _.Generate(ref seed)).ToArray();
        }

        public static GCSlot[] Greevil
        {
            get => new[] {
                new GCSlot("サイドン", 47),
                new GCSlot("ファイヤー", 50),
                new GCSlot("ナッシー", 47),
                new GCSlot("ケンタロス", 47),
                new GCSlot("フリーザー", 50),
                new GCSlot("サンダー", 50),
            };
        }
    }
}
