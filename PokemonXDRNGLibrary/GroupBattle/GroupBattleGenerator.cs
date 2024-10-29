using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonXDRNGLibrary.GroupBattle
{
    // see https://sina-poke.hatenablog.com/entry/2022/04/23/021304
    public class GroupBattleGenerator
    {
        private static readonly GCSlot[] dummies = new GCSlot[]
        {
            new GCSlot("Dummy", "Genderless"),
            new GCSlot("Dummy", "M1F1", Gender.Male, Nature.Impish),
            new GCSlot("Dummy", "M3F1", Gender.Male, Nature.Impish),
            new GCSlot("Dummy", "Genderless", Gender.Genderless, Nature.Naive),
            new GCSlot("Dummy", "Genderless", Gender.Genderless, Nature.Relaxed),
            new GCSlot("Dummy", "M1F1", Gender.Male, Nature.Rash)
        };

        private readonly uint _tsv;

        public uint EnterGroupBattle(uint seed)
        {
            seed.Advance(122);

            uint tid = seed.GetRand();
            uint sid = seed.GetRand();
            uint tsv = tid ^ sid;

            for (int i = 0; i < 3; i++)
            {
                dummies[0].Use(ref seed, tsv);
                seed.GenerateEVsDummy();
            }

            dummies[1].Use(ref seed, tsv);
            dummies[1].Use(ref seed, tsv);
            dummies[2].Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            dummies[0].Use(ref seed, tsv);
            seed.GenerateEVsDummy();

            dummies[3].Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            dummies[4].Use(ref seed, tsv);
            dummies[5].Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            for (int i = 0; i < 2; i++)
            {
                dummies[0].Use(ref seed, tsv);
                seed.GenerateEVsDummy();
            }

            return seed;
        }

        public GroupBattleGenerator(uint tsv)
            => _tsv = tsv;
    }
}
