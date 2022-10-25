using System;
using System.Collections.Generic;
using System.Linq;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;

namespace PokemonGCRNGLibrary
{
    public class CODarkPokemon
    {
        public readonly GCSlot darkPokemon;
        public readonly IReadOnlyList<PreGenerateSlot> PreGeneratePokemons = new PreGenerateSlot[0];

        internal CODarkPokemon(string name, uint lv)
        {
            darkPokemon = new GCSlot(name, lv);
        }
        internal CODarkPokemon(string name, uint lv, PreGenerateSlot[] preGenerate)
        {
            darkPokemon = new GCSlot(name, lv);
            PreGeneratePokemons = preGenerate;
        }

        public virtual GCIndividual Generate(uint seed)
        {
            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed, DummyTSV);
            }
            return darkPokemon.Generate(seed, out seed, DummyTSV);
        }
        public virtual GCIndividual Generate(uint seed, Criteria criteria)
        {
            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed, DummyTSV);
            }
            return darkPokemon.Generate(seed, DummyTSV, criteria);
        }

        public IReadOnlyList<RNGTarget> CalcBack(uint H, uint A, uint B, uint C, uint D, uint S)
        {
            var preList = PreGeneratePokemons.Reverse().ToArray();
            var resList = new List<RNGTarget>();

            var generatingSeedList = SeedFinder.FindGeneratingSeed(H, A, B, C, D, S, false);
            foreach (var seed in generatingSeedList)
            {
                var PSV = (seed.NextSeed(6) >> 16) ^ (seed.NextSeed(7) >> 16); // ダークポケモンのPSV
                var cells = new List<CalcBackCell>() { new CalcBackCell(seed) };

                // 事前生成されるポケモンの分
                foreach (var pre in preList)
                {
                    var list = new List<CalcBackCell>();
                    foreach (var cell in cells) list.AddRange(pre.CalcBack(cell));

                    cells = list;
                }

                // TSV生成の部分.
                var genSeedList1 = new List<uint>(); // 色回避無し個体
                var genSeedList2 = new List<uint>(); // 色回避有り個体
                foreach (var cell in cells)
                {
                    var res = cell.GetGeneratableSeed();
                    if (res.flag)
                    {
                        // tsvによって振り分けるようにする; tsv == psvならスキップ個体になる.
                        if (res.TSV == PSV) genSeedList2.Add(res.seed); else genSeedList1.Add(res.seed);
                    }
                }

                if (genSeedList1.Count > 0) resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, out uint finSeed), genSeedList1.ToArray()));
                if (genSeedList2.Count > 0) resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, out uint finSeed, PSV), genSeedList2.ToArray()));
            }

            return resList;
        }

    }

    public class XDDarkPokemon
    {
        private readonly uint lvBonus;
        public readonly GCSlot darkPokemon;
        public readonly IReadOnlyList<PreGenerateSlot> PreGeneratePokemons = new PreGenerateSlot[0];
        internal XDDarkPokemon(string name, uint lv, uint bonus = 0)
        {
            darkPokemon = new GCSlot(name, lv);
            lvBonus = bonus;
        }
        internal XDDarkPokemon(string name, uint lv, PreGenerateSlot[] preGenerate, uint bonus = 0)
        {
            darkPokemon = new GCSlot(name, lv);
            PreGeneratePokemons = preGenerate;
            lvBonus = bonus;
        }

        public virtual GCIndividual Generate(uint seed)
        {
            uint r;
            do { r = seed.GetRand(10); } while (r == 3); // トレーナーによっては他の値でもスキップ?
            if (r == 8) seed.Advance(5); else if (r < 5) seed.Advance(); else seed.Advance(2);

            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed);
            }
            return darkPokemon.Generate(seed, out seed);
        }
        public virtual GCIndividual Generate(uint seed, uint pTSV)
        {
            uint r;
            do { r = seed.GetRand(10); } while (r == 3); // トレーナーによっては他の値でもスキップ?
            if (r == 8) seed.Advance(5); else if (r < 5) seed.Advance(); else seed.Advance(2);

            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed, pTSV);
            }
            return darkPokemon.Generate(seed, out seed, pTSV);
        }

        public virtual GCIndividual Generate(uint seed, Criteria criteria)
        {
            uint r;
            do { r = seed.GetRand(10); } while (r == 3); // トレーナーによっては他の値でもスキップ?
            if (r == 8) seed.Advance(5); else if (r < 5) seed.Advance(); else seed.Advance(2);

            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed);
            }
            return darkPokemon.Generate(seed, criteria);
        }
        public virtual GCIndividual Generate(uint seed, uint pTSV, Criteria criteria)
        {
            uint r;
            do { r = seed.GetRand() % 10; } while (r == 3); // トレーナーによっては他の値でもスキップ?
            if (r == 8) seed.Advance(5); else if (r < 5) seed.Advance(); else seed.Advance(2);

            uint DummyTSV = seed.GetRand() ^ seed.GetRand();
            for (int i = 0; i < PreGeneratePokemons.Count; i++)
            {
                PreGeneratePokemons[i].Generate(seed, out seed, pTSV);
            }
            return darkPokemon.Generate(seed, pTSV, criteria);
        }

        public IReadOnlyList<RNGTarget> SearchTarget(uint H, uint A, uint B, uint C, uint D, uint S, RNGTargetCriteria criteria)
        {
            var seedList = SeedFinder.FindGeneratingSeed(H, A, B, C, D, S, false).ToArray();
            var resList = seedList.Select(_ => { var s = _; return new RNGTarget(s, darkPokemon.Generate(s, out s)); }).ToArray();

            for (int k = 0; k < seedList.Length; k++)
            {
                if (criteria.HiddenPowerType != PokeType.None && resList[k].targetIndividual.HiddenPowerType != criteria.HiddenPowerType) continue;
                if (resList[k].targetIndividual.HiddenPower < criteria.MinHiddenPower) continue;
                if (criteria.ability != "" && resList[k].targetIndividual.Ability != criteria.ability) continue;
                if (criteria.gender != Gender.Genderless && resList[k].targetIndividual.Gender != criteria.gender) continue;
                if (!criteria.nature[(int)resList[k].targetIndividual.Nature]) continue;
                var seed = seedList[k].PrevSeed(1024);
                var genSeedList = new List<uint>();
                for (int i = 0; i <= 1024; i++)
                {
                    var res = Generate(seed);
                    if (res.RepresentativeSeed == seedList[k]) genSeedList.Add(seed);
                    seed.Advance();
                }
                resList[k].generatableSeeds = genSeedList.ToArray();
            }

            return resList.Where(_=>_.generatableSeeds.Length != 0).ToArray();
        }

        public IReadOnlyList<RNGTarget> CalcBack(uint H, uint A, uint B, uint C, uint D, uint S, uint TSV = 0x10000)
        {
            var preList = PreGeneratePokemons.Reverse().ToArray();
            var resList = new List<RNGTarget>();

            var generatingSeedList = SeedFinder.FindGeneratingSeed(H, A, B, C, D, S, false);
            foreach (var seed in generatingSeedList)
            {
                var PSV = (seed.NextSeed(6) >> 16) ^ (seed.NextSeed(7) >> 16);
                var cells = new List<CalcBackCell>() { new CalcBackCell(seed) };

                // 事前生成されるポケモンの分
                foreach (var pre in preList)
                {
                    var list = new List<CalcBackCell>();
                    foreach (var cell in cells) list.AddRange(pre.CalcBack(cell));

                    cells = list;
                }

                // TSV生成の部分.
                var genSeedList1 = new List<uint>(); // 色回避無し個体
                var genSeedList2 = new List<uint>(); // 色回避有り個体
                foreach (var cell in cells)
                {
                    if (TSV != 0x10000 && cell.preGeneratedPSVList.Any(_ => (_ ^ TSV) < 8)) continue; // 色回避が発生してしまうのでダメ~~~~~~~~
                    if (cell.TSV != 0x10000 && ((TSV ^ cell.TSV) >= 8) || TSV == 0x10000) continue; // TSV条件を満たさないのでダメ~~~~~~~~

                    var seeds = CalcBackAngle(cell.seed);
                    if (TSV == PSV) genSeedList2.AddRange(seeds); else genSeedList1.AddRange(seeds);
                }

                // TSV条件をreturnするのはまた今度にします...
                if (genSeedList1.Count > 0) resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, out uint finSeed), genSeedList1.ToArray()));
                if (genSeedList2.Count > 0) resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, out uint finSeed, PSV), genSeedList2.ToArray()));
            }

            return resList;
        }

        protected virtual IEnumerable<uint> CalcBackAngle(uint seed)
        {
            seed.Back(2); // 敵のTSV生成の分.

            {
                var _seed = seed;
                var r = _seed >> 16;
                if (r < 5 && r != 3)
                {
                    yield return _seed.Back();

                    while(_seed >> 16 == 3)
                    {
                        yield return _seed.Back();
                    }
                }
            }

            {
                var _seed = seed;
                var r = _seed.Back() >> 16;
                if (r >= 5 && r != 8)
                {
                    yield return _seed.Back();

                    while (_seed >> 16 == 3)
                    {
                        yield return _seed.Back();
                    }
                }
            }

            {
                var _seed = seed;
                if ((_seed.Back(4) >> 16) == 8)
                {
                    yield return _seed.Back();

                    while ((_seed >> 16) == 3)
                    {
                        yield return _seed.Back();
                    }
                }
            }
        }
    }

    class XDTogepii : XDDarkPokemon
    {
        internal XDTogepii() : base("トゲピー", 25) { }


        public override GCIndividual Generate(uint seed)
        {
            return darkPokemon.Generate(seed, out seed);
        }
        public override GCIndividual Generate(uint seed, uint pTSV)
        {
            return darkPokemon.Generate(seed, out seed, pTSV);
        }

        public override GCIndividual Generate(uint seed, Criteria criteria)
        {
            return darkPokemon.Generate(seed, 0x10000, criteria);
        }
        public override GCIndividual Generate(uint seed, uint pTSV, Criteria criteria)
        {
            return darkPokemon.Generate(seed, pTSV, criteria);
        }
    }
}