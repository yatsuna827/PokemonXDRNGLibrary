using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PokemonXDRNGLibrary.GroupBattle
{
    // see https://sina-poke.hatenablog.com/entry/2022/04/23/021304
    public class GroupBattleGenerator
    {
        private static readonly GCSlot dummyGenderless = new GCSlot("Dummy", "Genderless");
        private static readonly GCSlot dummyM1F1MaleImpish = new GCSlot("Dummy", "M1F1", Gender.Male, Nature.Impish);
        private static readonly GCSlot dummyM3F1MaleImpish = new GCSlot("Dummy", "M3F1", Gender.Male, Nature.Impish);
        private static readonly GCSlot dummyGenderlessNaive = new GCSlot("Dummy", "Genderless", Gender.Genderless, Nature.Naive);
        private static readonly GCSlot dummyGenderlessRelaxed = new GCSlot("Dummy", "Genderless", Gender.Genderless, Nature.Relaxed);
        private static readonly GCSlot dummyM1F1MaleRash = new GCSlot("Dummy", "M1F1", Gender.Male, Nature.Rash);

        private readonly uint _tsv;

        public uint EnterGroupBattle(uint seed)
        {
            seed.Advance(122);

            uint tid = seed.GetRand();
            uint sid = seed.GetRand();
            uint tsv = tid ^ sid;

            for (int i = 0; i < 3; i++)
            {
                dummyGenderless.Use(ref seed, tsv);
                seed.GenerateEVsDummy();
            }

            dummyM1F1MaleImpish.Use(ref seed, tsv);
            dummyM1F1MaleImpish.Use(ref seed, tsv);
            dummyM3F1MaleImpish.Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            dummyGenderless.Use(ref seed, tsv);
            seed.GenerateEVsDummy();

            dummyGenderlessNaive.Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            dummyGenderlessRelaxed.Use(ref seed, tsv);
            dummyM1F1MaleRash.Use(ref seed, tsv);

            tid = seed.GetRand();
            sid = seed.GetRand();
            tsv = tid ^ sid;

            for (int i = 0; i < 2; i++)
            {
                dummyGenderless.Use(ref seed, tsv);
                seed.GenerateEVsDummy();
            }

            return seed;
        }

        public GroupBattleGenerator(uint tsv)
            => _tsv = tsv;
    }
}
