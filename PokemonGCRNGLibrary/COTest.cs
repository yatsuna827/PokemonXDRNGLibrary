using PokemonPRNG.LCG32.GCLCG;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class COTest
    {
        private static GCSlot[] preGenerate = new GCSlot[] {
                new GCSlot("アメモース", Gender.Male, Nature.Hardy),
                new GCSlot("アリアドス", Gender.Female, Nature.Hardy),
            };
        public static GCSlot testSlot = new GCSlot("ヘラクロス", 45);
        public static GCIndividual[] generateWithTSV(uint seed)
        {
            var resList = new List<GCIndividual>();
            var tsv = seed.GetRand() ^ seed.GetRand();
            foreach (var slot in preGenerate)
                resList.Add(slot.Generate(seed, out seed, tsv));

            resList.Add(testSlot.Generate(seed, out seed, tsv));
            return resList.ToArray();
        }
        public static GCIndividual[] generateWithoutTSV(uint seed)
        {
            var resList = new List<GCIndividual>();
            seed.Advance(2);
            foreach (var slot in preGenerate)
                resList.Add(slot.Generate(seed, out seed));

            resList.Add(testSlot.Generate(seed, out seed));
            return resList.ToArray();
        }

        public static bool areEqual(GCIndividual a, GCIndividual b)
        {
            var pid = a.PID == b.PID;
            var ivs = Enumerable.Range(0, 6).All(i => a.IVs[i] == b.IVs[i]);

            return pid && ivs;
        }

        public static bool areEqual(GCIndividual[] a, GCIndividual[] b)
        {
            if (a.Length != b.Length) return false;
            var res = true;
            for (int i = 0; i < a.Length; i++)
                res &= areEqual(a[i], b[i]);
            return res;
        }
    }
}
