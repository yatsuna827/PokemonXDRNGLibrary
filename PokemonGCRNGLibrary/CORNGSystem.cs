using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    public static class CORNGSystem
    {
        private static uint[] mul = { 1, 0x18, 0x240, 0x3600, 0x51000 };
        private static readonly GCSlot DummySlot = new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Male);
        private static readonly IReadOnlyList<IReadOnlyList<GCSlot>> DummyParty = new GCSlot[][]
        {
            new GCSlot[] {
                new GCSlot(Pokemon.GetDummy(GenderRatio.FemaleOnly), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M3F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Male),
            },
            new GCSlot[] {
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
            },
            new GCSlot[] {
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F3), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M7F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
            },
            new GCSlot[] {
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.FemaleOnly), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Female),
                new GCSlot(Pokemon.GetDummy(GenderRatio.M1F1), Gender.Male),
            }
        };

        internal static readonly IReadOnlyList<CODarkPokemon> CODarkPokemonList;
        internal static readonly Dictionary<string, CODarkPokemon> CODarkPokemonDictionary;


        static public uint CalcOffset(uint seed)
        {
            seed.Advance(1000);
            uint TSV = seed.GetRand() ^ seed.GetRand();

            DummySlot.Generate(seed, out seed);
            DummySlot.Generate(seed, out seed);
            seed.Advance(2);

            return seed;
        }
        static public void AdvanceOffset(ref this uint seed)
        {
            seed.Advance(1000);
            uint TSV = seed.GetRand() ^ seed.GetRand();

            DummySlot.Generate(seed, out seed);
            DummySlot.Generate(seed, out seed);
            seed.Advance(2);
        }
        static public uint GenerateDummyParty(uint seed)
        {
            seed.Advance(118);
            for (int i = 0; i < 4; i++)
            {
                uint DummyTSV = seed.GetRand() ^ seed.GetRand();
                var Party = DummyParty[i];
                for (int k = 0; k < 6; k++)
                {
                    Party[k].Generate(seed, out seed);
                }
            }
            seed.Advance(7); // 完全に条件無しのダミーの生成

            return seed;
        }
        static public (uint,uint) GenerateDBCode(uint seed)
        {
            seed.AdvanceOffset();
            seed = GenerateDummyParty(seed);

            uint code = 0;
            for(int i=4; i>=0; i--)
            {
                var res = BattleNow.SingleBattle.Ultimate.GenerateBattleTeam(seed);
                seed = res.FinishingSeed;
                code += (uint)(res.code * mul[i]);
            }
            return (code, seed);
        }
        static public (uint key, uint code, uint seed) GenerateRuntimeDBCode(uint seed)
        {
            uint[] code = new uint[7];
            for (int i = 0; i < 7; i++)
            {
                code[i] = BattleNow.SingleBattle.Ultimate.GenerateCode(seed, out seed);
            }

            return (code[0] + code[1] * 24, code[2] + code[3] * 24 + code[4] * 24 * 24 + code[5] * 24 * 24 * 24 + code[6] * 24 * 24 * 24 * 24, seed);
        }
        static public uint GenerateRuntimeDBCode(uint seed, out uint finseed)
        {
            uint[] code = new uint[5];
            for (int i = 0; i < 5; i++)
            {
                code[i] = BattleNow.SingleBattle.Ultimate.GenerateCode(seed, out seed);
            }

            finseed = seed;
            return code[0] * 24 * 24 * 24 * 24 + code[1] * 24 * 24 * 24 + code[2] * 24 * 24 + code[3] * 24 + code[4];
        }

        static public void RollRNGNamingScreenNext(ref this uint seed)
        {
            if (seed.GetRand_f() < 0.1f) seed.Advance(4);
        }

        static public IReadOnlyList<uint> CalcIrregularAdvanceList(uint seed, int maxFrame)
        {
            var mainCounter = new MainCounter[4]
            {
                new MainCounter(),
                new MainCounter(),
                new MainCounter(),
                new MainCounter(),
            };

            var resList = new List<uint>() { seed };
            for (int i = 0; i < maxFrame; i++)
            {
                var adv = (uint)mainCounter.Sum(_ => _.HeaderAdvance()) + (uint)mainCounter.Sum(_ => _.LazyAdvance1()) + (uint)mainCounter.Sum(_ => _.LazyAdvance2());
                seed.Advance(adv);

                foreach (var c in mainCounter) c.CountUp(ref seed);

                resList.Add(seed);
            }

            return resList;
        }

        static public IReadOnlyList<uint> CalcStandAdvanceList(uint seed, int maxFrame)
        {
            var counter = new StandCounter(ref seed);
            var resList = new List<uint>() { seed };
            for (int i = 0; i < maxFrame; i++)
            {
                counter.CountUp(ref seed);
                resList.Add(seed.NextSeed(4));
            }

            return resList;
        }
        
        static public IReadOnlyList<uint> GetSeedList(uint seed, int maxFrame)
        {
            var resList = new List<uint>() { seed };
            for(int i=0; i< maxFrame; i++)
            {
                seed.Advance();
                resList.Add(seed);
            }
            return resList;
        }

        static public bool Blink(ref this uint seed, int i)
        {
            if (i < 10) return false;
            return seed.GetRand_f() < GetBlinkThreshold(i);
        }

        static public float GetBlinkThreshold(int i)
        {
            if (i >= 180) return 1.0f;
            if (i > 60) i = 60;

            i -= 10;
            var f1 = i / 50.0f;
            var f2 = 2.0f - f1;
            f2 *= f1;
            f2 *= 1 / 60.0f;

            return f2;
        }

        public static CODarkPokemon GetDarkPokemon(int index) { return CODarkPokemonList[index]; }
        public static CODarkPokemon GetDarkPokemon(string name) { return CODarkPokemonDictionary[name]; }
        public static IReadOnlyList<CODarkPokemon> GetCODarkPokemonList() { return CODarkPokemonList; }
        static CORNGSystem()
        {
            var coList = new List<CODarkPokemon>();
            coList.Add(new CODarkPokemon("マクノシタ", 30, new GCSlot[] {
                new GCSlot("ヨマワル", Gender.Male, Nature.Quirky),
                new GCSlot("イトマル", Gender.Female, Nature.Hardy)
            }));
            coList.Add(new CODarkPokemon("ベイリーフ", 30));
            coList.Add(new CODarkPokemon("マグマラシ", 30));
            coList.Add(new CODarkPokemon("アリゲイツ", 30));
            coList.Add(new CODarkPokemon("ヨルノズク", 30));
            coList.Add(new CODarkPokemon("モココ", 30));
            coList.Add(new CODarkPokemon("ポポッコ", 30));
            coList.Add(new CODarkPokemon("ヌオー", 30));
            coList.Add(new CODarkPokemon("ムウマ", 30));
            coList.Add(new CODarkPokemon("マグマッグ", 30));
            coList.Add(new CODarkPokemon("オオタチ", 33));
            coList.Add(new CODarkPokemon("ヤンヤンマ", 33));
            coList.Add(new CODarkPokemon("テッポウオ", 20));
            coList.Add(new CODarkPokemon("マンタイン", 33));
            coList.Add(new CODarkPokemon("ハリーセン", 33));
            coList.Add(new CODarkPokemon("アサナン", 33));
            coList.Add(new CODarkPokemon("ノコッチ", 33));
            coList.Add(new CODarkPokemon("チルット", 33));
            coList.Add(new CODarkPokemon("ウソッキー", 35));
            coList.Add(new CODarkPokemon("カポエラー", 38));
            coList.Add(new CODarkPokemon("エンテイ", 40));
            coList.Add(new CODarkPokemon("レディアン", 40));
            coList.Add(new CODarkPokemon("スイクン", 40));
            coList.Add(new CODarkPokemon("グライガー", 43, new GCSlot[] {
                new GCSlot("ヒメグマ", Gender.Male, Nature.Serious),
                new GCSlot("プリン", Gender.Female, Nature.Docile),
                new GCSlot("キノココ", Gender.Male, Nature.Bashful)
            }));
            coList.Add(new CODarkPokemon("オドシシ", 43));
            coList.Add(new CODarkPokemon("イノムー", 43));
            coList.Add(new CODarkPokemon("ニューラ", 43));
            coList.Add(new CODarkPokemon("エイパム", 43));
            coList.Add(new CODarkPokemon("ヤミカラス", 43, new GCSlot[] {
                new GCSlot("キバニア", Gender.Male, Nature.Docile),
                new GCSlot("コノハナ", Gender.Female, Nature.Serious),
                new GCSlot("デルビル", Gender.Male, Nature.Bashful)
            }));
            coList.Add(new CODarkPokemon("フォレトス", 43));
            coList.Add(new CODarkPokemon("アリアドス", 43));
            coList.Add(new CODarkPokemon("グランブル", 43));
            coList.Add(new CODarkPokemon("ビブラーバ", 43));
            coList.Add(new CODarkPokemon("ライコウ", 40));
            coList.Add(new CODarkPokemon("キマワリ", 45));
            coList.Add(new CODarkPokemon("デリバード", 45));
            coList.Add(new CODarkPokemon("ヘラクロス", 45, new GCSlot[] {
                new GCSlot("アメモース", Gender.Male, Nature.Hardy),
                new GCSlot("アリアドス", Gender.Female, Nature.Hardy),
            }));
            coList.Add(new CODarkPokemon("エアームド", 47));
            coList.Add(new CODarkPokemon("ミルタンク", 48));
            coList.Add(new CODarkPokemon("アブソル", 48));
            coList.Add(new CODarkPokemon("ヘルガー", 48));
            coList.Add(new CODarkPokemon("トロピウス", 49));
            coList.Add(new CODarkPokemon("メタグロス", 50));
            coList.Add(new CODarkPokemon("バンギラス", 55));
            coList.Add(new CODarkPokemon("ドーブル", 45));
            coList.Add(new CODarkPokemon("リングマ", 45, new GCSlot[] {
                new GCSlot("ゴーリキー", Gender.Female, Nature.Calm),
                new GCSlot("ヌマクロー", Gender.Male, Nature.Mild),
                new GCSlot("ダーテング", Gender.Female, Nature.Gentle)
            }));
            coList.Add(new CODarkPokemon("ツボツボ", 45));
            coList.Add(new CODarkPokemon("トゲチック", 20));

            CODarkPokemonList = coList;
            CODarkPokemonDictionary = coList.ToDictionary(_ => _.darkPokemon.pokemon.Name, _ => _);
        }
    }
}
