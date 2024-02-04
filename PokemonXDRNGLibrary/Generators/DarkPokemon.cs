using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using static System.Net.Mime.MediaTypeNames;

namespace PokemonXDRNGLibrary
{
    public class XDDarkPokemon : IGeneratable<GCIndividual>, IGeneratable<GCIndividual, uint>
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

        private static readonly FirstCameraAngleGenerator _angleGenerator = new FirstCameraAngleGenerator();
        public virtual GCIndividual Generate(uint seed)
        {
            seed.Advance(_angleGenerator);

            seed.Advance(2); // enemyTSV
            foreach (var p in PreGeneratePokemons)
                seed.Advance(p);

            return darkPokemon.Generate(seed);
        }
        public virtual GCIndividual Generate(uint seed, uint pTSV)
        {
            seed.Advance(_angleGenerator);

            seed.Advance(2); // enemyTSV
            foreach (var p in PreGeneratePokemons)
                seed.Advance(p, pTSV);

            return darkPokemon.Generate(seed, tsv: pTSV);
        }

        public virtual GCIndividual Generate(uint seed, Criteria criteria)
        {
            seed.Advance(_angleGenerator);

            seed.Advance(2); // enemyTSV
            foreach (var p in PreGeneratePokemons)
                seed.Advance(p);

            return darkPokemon.Generate(seed, criteria);
        }
        public virtual GCIndividual Generate(uint seed, uint pTSV, Criteria criteria)
        {
            seed.Advance(_angleGenerator);

            seed.Advance(2); // enemyTSV
            foreach (var p in PreGeneratePokemons)
                seed.Advance(p, pTSV);

            return darkPokemon.Generate(seed, pTSV, criteria);
        }

        private static readonly AngleReverser _angleReverser = new AngleReverser();
        public IReadOnlyList<RNGTarget> CalcBack(uint H, uint A, uint B, uint C, uint D, uint S)
        {
            var resList = new List<RNGTarget>();

            foreach (var seed in SeedFinder.FindGeneratingSeed(H, A, B, C, D, S, false))
            {
                // 生成される個体の色回避発生前のSV
                var psv = ((seed.NextSeed(6) >> 16) ^ (seed.NextSeed(7) >> 16)) & 0xFFF8;

                var generalResult = new List<uint>(); // 色回避無し個体
                var rerolledResult = new Dictionary<uint, List<uint>>(); // 色回避有り個体

                var root = new PregenerateNode();

                if (PreGeneratePokemons.Count == 0)
                {
                    var seeds = _angleReverser.Reverse(seed.PrevSeed(2)).ToArray();
                    generalResult.AddRange(seeds);
                }
                else
                {
                    var queue = new Queue<(int Index, CalcBackCell Cell, PregenerateNode Parent)>();
                    queue.Enqueue((PreGeneratePokemons.Count - 1, new CalcBackCell(seed), root));
                    while (queue.Count > 0)
                    {
                        (var index, var cell, var parent) = queue.Dequeue();

                        var pregen = PreGeneratePokemons[index];
                        if (!pregen.CanGeneratedBy(cell.Seed, cell.ConditionedTSV)) continue;

                        var node = pregen.CreateNode(cell, parent);

                        foreach (var _c in pregen.CalcBack(cell))
                        {
                            if (index == 0)
                            {
                                var seeds = _angleReverser.Reverse(_c.Seed.PrevSeed(2)).ToArray();

                                // 到達可能な起点seedでグループ化して返したいのでここではyield returnしない

                                // 位置ずれ前提の場合
                                if (_c.ConditionedTSV != 0x10000)
                                {
                                    if (node.CheckTSV(_c.ConditionedTSV))
                                    {
                                        if (!rerolledResult.ContainsKey(_c.ConditionedTSV))
                                            rerolledResult.Add(_c.ConditionedTSV, new List<uint>());
                                        rerolledResult[_c.ConditionedTSV].AddRange(seeds);
                                    }
                                }
                                else
                                {
                                    node.Feedback();
                                    generalResult.AddRange(seeds);
                                }
                            }
                            else
                            {
                                queue.Enqueue((index - 1, _c, node));
                            }
                        }
                    }
                }

                if (generalResult.Count > 0)
                {
                    var list = root.GetContraindicatedTSVs(PreGeneratePokemons.Count);
                    // 色回避を起こして到達不可能になるTSVと、生成される個体の色回避が発生するTSVが同じであれば、色回避個体は到達不可能
                    if (!list.Contains(psv))
                    {
                        list.Add(psv);
                        resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed), generalResult.ToArray(), contraindicatedTSVs: list.ToArray()));
                        resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, tsv: psv), generalResult.ToArray(), conditionedTSV: psv));
                    }
                    else
                    {
                        resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed), generalResult.ToArray(), contraindicatedTSVs: list.ToArray()));
                    }
                }
                else if (rerolledResult.Count > 0)
                {
                    // 普通では到達不能だが特定のTSVのみPreGenerateSlotの生成で色回避が発生して到達可能になるパターン
                    // いわゆる位置ズレ
                    // 普通に到達可能な場合は取り立てて結果に加える必要は無いと思う
                    // 
                    foreach (var rerolled in rerolledResult)
                    {
                        var tsv = rerolled.Key;
                        var seeds = rerolled.Value.ToArray();
                        if (seeds.Length > 0)
                            resList.Add(new RNGTarget(seed, darkPokemon.Generate(seed, tsv), seeds, tsv));
                    }
                }
            }

            return resList;
        }

        public IEnumerable<RNGTarget> CalcBack(uint H, uint A, uint B, uint C, uint D, uint S, uint tsv)
        {
            foreach (var generationSeed in SeedFinder.FindGeneratingSeed(H, A, B, C, D, S, false))
            {
                var results = new List<uint>(); // 色回避有り個体

                var stack = new Stack<(int Index, uint Seed)>();
                stack.Push((PreGeneratePokemons.Count, generationSeed));
                while (stack.Count > 0)
                {
                    (var index, var seed) = stack.Pop();
                    if (index-- == 0)
                    {
                        // 到達可能な起点seedでグループ化して返したいのでここではyield returnしない
                        results.AddRange(_angleReverser.Reverse(seed.PrevSeed(2)));
                    }
                    else
                    {
                        foreach (var nextSeed in PreGeneratePokemons[index].CalcBack(seed, tsv))
                            stack.Push((index, nextSeed));
                    }
                }

                if (results.Count > 0) yield return new RNGTarget(generationSeed, darkPokemon.Generate(generationSeed, tsv), results.ToArray());
            }
        }

    }

    class XDTogepii : XDDarkPokemon
    {
        internal XDTogepii() : base("トゲピー", 25) { }


        public override GCIndividual Generate(uint seed)
        {
            return darkPokemon.Generate(seed);
        }
        public override GCIndividual Generate(uint seed, uint pTSV)
        {
            return darkPokemon.Generate(seed, pTSV);
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
