using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using PokemonStandardLibrary.Gen3;

namespace PokemonXDRNGLibrary
{
    class PokeSpotSlot : IGeneratable<GCIndividual, uint>
    {
        public uint BaseLv { get; }
        public uint VariableLv { get; }
        public Pokemon.Species Species { get; }

        public GCIndividual Generate(uint seed, uint pid)
        {
            seed.Advance(3); // dummyTSV, and ???
            var lv = BaseLv + seed.GetRand(VariableLv);
            seed.Advance(2); // dummyPID
            var ivs = seed.GenerateIVs();
            var abilityIndex = seed.GetRand(2);
            
            return Species.GetIndividual(pid, lv, ivs, abilityIndex, false);
        }

        public PokeSpotSlot(string name, uint baseLv, uint LvRange)
        {
            this.Species = Pokemon.GetPokemon(name);
            this.BaseLv = baseLv;
            this.VariableLv = LvRange;
        }
    }

    public class PokeSpotPID
    {
        private Pokemon.Individual Individual;
        public readonly uint PID;
        internal readonly PokeSpotSlot Slot;
        public virtual string Name { get { return Individual.Name; } }
        public virtual Nature Nature { get { return Individual.Nature; } }
        public virtual string Ability { get { return Individual.Ability; } }
        public virtual Gender Gender { get { return Individual.Gender; } }
        public virtual bool isShiny(uint TSV) { return Individual.PID.IsShiny(TSV); }
        public GCIndividual Generate(uint seed)
        {
            seed.Advance(4);
            uint r;
            do { r = seed.GetRand(10); } while (r == 3);
            if (r == 8) seed.Advance(5); else if (r < 5) seed.Advance(); else seed.Advance(2);
            
            return Slot.Generate(seed, PID);
        }

        public static readonly PokeSpotPID Munchlax = new _Munchlax();
        public static readonly PokeSpotPID Empty = new PokeSpotPID();
        public static readonly PokeSpotPID Bonsly = new _Bonsly();

        public static readonly PokeSpotPID debug = new PokeSpotPID(0, new PokeSpotSlot("サンド", 10, 11));

        class _Munchlax : PokeSpotPID
        {
            public override string Name { get { return "ゴンベ"; } }
            public override Nature Nature { get { return Nature.other; } }
            public override string Ability { get { return "---"; } }
            public override Gender Gender { get { return Gender.Genderless; } }
            public override bool isShiny(uint TSV) { return false; }
        }
        class _Bonsly : PokeSpotPID
        {
            public override string Name { get { return "ウソハチ"; } }
            public override Nature Nature { get { return Nature.other; } }
            public override string Ability { get { return "---"; } }
            public override Gender Gender { get { return Gender.Genderless; } }
            public override bool isShiny(uint TSV) { return false; }
        }

        private PokeSpotPID()
        {
            Individual = Pokemon.Individual.Empty;
        }
        internal PokeSpotPID(uint PID, PokeSpotSlot slot)
        {
            this.PID = PID;
            Slot = slot;
            if(slot != null)
            Individual = slot.Species.GetIndividual(1, new uint[6], PID);
        }
    }

    public enum PokeSpot
    {
        RockGround, Oasis, Cave
    }
    public class PokeSpotGenerator
    {
        private protected PokeSpotSlot[] table;
        static PokeSpotSlot[][] PokeSpotTable = new PokeSpotSlot[][]
        {
            new PokeSpotSlot[] { new PokeSpotSlot("サンド", 10, 14), new PokeSpotSlot("グライガー", 10, 11), new PokeSpotSlot("ナックラー", 10, 11) },
            new PokeSpotSlot[] { new PokeSpotSlot("ハネッコ", 10, 11), new PokeSpotSlot("ゴマゾウ", 10, 11), new PokeSpotSlot("アメタマ", 10, 11) },
            new PokeSpotSlot[] { new PokeSpotSlot("ズバット", 10, 12), new PokeSpotSlot("ココドラ", 10, 12), new PokeSpotSlot("ウパー", 10, 12) }
        };

        public virtual PokeSpotPID Generate(uint seed)
        {
            if (seed.GetRand(3) != 0) return PokeSpotPID.Empty;
            if (seed.GetRand(100) < 10) return PokeSpotPID.Munchlax;

            var s = seed.GetRand(100);
            var index = s < 50 ? 0 : (s < 85 ? 1 : 2);
            var slot = table[index];

            uint PID = (seed.GetRand() << 16) | seed.GetRand();

            return new PokeSpotPID(PID, slot);
        }

        public static PokeSpotGenerator CreateGenerator(PokeSpot pokeSpot, bool USOHACHI = false)
        {
            if (USOHACHI) return new USOHACHIGenerator(pokeSpot);
            else return new PokeSpotGenerator(pokeSpot);
        }

        public static PokeSpotPID CreateIVsGenerator(PokeSpot pokeSpot, int idx, Nature nature)
        {
            return new PokeSpotPID((uint)nature, PokeSpotTable[(int)pokeSpot][idx]);
        }

        private PokeSpotGenerator(PokeSpot pokeSpot)
        {
            this.table = PokeSpotTable[(int)pokeSpot];
        }

        class USOHACHIGenerator : PokeSpotGenerator
        {
            public override PokeSpotPID Generate(uint seed)
            {
                if (seed.GetRand(3) != 0) return PokeSpotPID.Empty;
                seed.Advance();
                var ev = seed.GetRand(100);
                if (ev < 30) return PokeSpotPID.Bonsly;
                if (ev < 40) return PokeSpotPID.Munchlax;

                var s = seed.GetRand(100);
                var index = s < 50 ? 0 : (s < 85 ? 1 : 2);
                var slot = table[index];

                uint PID = (seed.GetRand() << 16) | seed.GetRand();

                return new PokeSpotPID(PID, slot);
            }
            public USOHACHIGenerator(PokeSpot pokeSpot) : base(pokeSpot) { }
        }
    }
}
