using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class Result
    {
        public Individual Individual { get; internal set; }

        public uint StartingSeed { get; internal set; }
        public uint FinishingSeed { get; internal set; }
        public uint DommyTSV { get; internal set; }
        public bool AvoidedShiny { get; internal set; }

    }

    public class RentalBattleResult
    {
        static internal readonly string[] PlayerNameList = { "レオ", "ユータ", "タツキ" };
        public string PlayerName { get; set; }
        public List<Individual> PlayerParty { get; set; }
        public List<Individual> EnemyParty { get; set; } 
        public uint StartingSeed { get; set; }
        public uint FinishingSeed { get; set; }
    }

    internal static class Generator
    {
        static internal IndivKernel GenerateIndividual(ref this uint seed)
        {
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID = seed.GetPID();
            return new IndivKernel(PID, IVs);
        }
        static internal IndivKernel GenerateIndividual(ref this uint seed, Gender WantedGender = Gender.Genderless, GenderRatio GenderRatio = GenderRatio.Genderless, Nature WantedNature = Nature.other)
        {
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID = seed.GetPID(_ => (WantedGender == Gender.Genderless || _.GetGender(GenderRatio) == WantedGender) && (WantedNature == Nature.other || (Nature)(_ % 25) == WantedNature));
            return new IndivKernel(PID, IVs);
        }
        static internal IndivKernel GenerateIndividual(ref this uint seed, uint TSV)
        {
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID;
            bool AvoidedShiny = false;
            while(true) {
                PID = seed.GetPID();
                if (!PID.isShiny(TSV)) break;
                AvoidedShiny = true;
            }

            return new GCIndivKernel(PID, IVs, AbilityIndex);
        }
        static internal IndivKernel GenerateIndividual(ref this uint seed, uint TSV, Gender WantedGender=Gender.Genderless, GenderRatio GenderRatio= GenderRatio.Genderless, Nature WantedNature = Nature.other)
        {
            uint[] IVs = seed.GetIVs();
            uint AbilityIndex = seed.GetRand(2);
            uint PID;
            bool AvoidedShiny = false;
            while (true)
            {
                PID = seed.GetPID(_ => (WantedGender == Gender.Genderless || _.GetGender(GenderRatio) == WantedGender) && (WantedNature == Nature.other || (Nature)(_ % 25) == WantedNature));
                if (!PID.isShiny(TSV)) break;
                AvoidedShiny = true;
            }
            return new GCIndivKernel(PID, IVs, AbilityIndex);
        }

    }

    public static class CORNGSystem
    {
        static public uint CalcOffset(uint seed)
        {
            seed.Advance(1000);
            uint TSV = seed.GetRand() ^ seed.GetRand();

            seed.Advance(2);
            seed.GenerateIndividual(Gender.Male, GenderRatio.M7F1);
            seed.Advance(2);
            seed.GenerateIndividual(Gender.Male, GenderRatio.M7F1);
            seed.Advance(2);

            return seed;
        }

        static public void AdvanceOffset(ref this uint seed)
        {
            seed.Advance(1000);
            uint TSV = seed.GetRand() ^ seed.GetRand();

            seed.Advance(2);
            seed.GenerateIndividual(Gender.Male, GenderRatio.M7F1);
            seed.Advance(2);
            seed.GenerateIndividual(Gender.Male, GenderRatio.M7F1);
            seed.Advance(2);
        }

        static public uint GenerateDommyParty(uint seed)
        {
            seed.Advance(118);
            for (int i = 0; i < 4; i++)
            {
                uint DommyTSV = seed.GetRand() ^ seed.GetRand();
                RentalParty Party = RentalParty.DommyParty[i];
                for (int k = 0; k < 6; k++)
                {
                    seed.Advance(2);
                    seed.GenerateIndividual(Party[k].FixedGender, Party[k].GenderRatio);
                }
            }
            seed.Advance(2);
            seed.GenerateIndividual();

            return seed;
        }

        static public RentalBattleResult GenerateBattleTeam(uint seed)
        {
            uint StartingSeed = seed;
            uint EnemyTeamIndex = seed.GetRand(8);
            uint PlayerTeamIndex;
            do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            RentalParty EnemyParty = RentalParty.UltimateParty[EnemyTeamIndex];
            List<Individual> EList = new List<Individual>();
            for (int i = 0; i < 6; i++)
            {
                seed.Advance(2);
                EList.Add(EnemyParty[i].GetIndividual(seed.GenerateIndividual(EnemyTSV, EnemyParty[i].FixedGender, EnemyParty[i].GenderRatio, EnemyParty[i].FixedNature)));
            }
            uint PlayerNameIndex = seed.GetRand(3);

            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            RentalParty PlayerParty = RentalParty.UltimateParty[PlayerTeamIndex];
            List<Individual> PList = new List<Individual>();
            for (int i = 0; i < 6; i++)
            {
                seed.Advance(2);
                PList.Add(PlayerParty[i].GetIndividual(seed.GenerateIndividual(PlayerTSV, PlayerParty[i].FixedGender, PlayerParty[i].GenderRatio, PlayerParty[i].FixedNature)));
            }

            return new RentalBattleResult()
            {
                StartingSeed = StartingSeed,
                FinishingSeed = seed,
                PlayerName = RentalBattleResult.PlayerNameList[PlayerNameIndex],
                EnemyParty = EList,
                PlayerParty = PList
            };
        }



        static public void RollRNGNamingScreenNext(ref this uint seed)
        {
            if (seed.GetRand() < 0x199A) seed.Advance(4);
        }
    }

    internal class GCPokemon
    {
        internal readonly Pokemon Pokemon;
        internal readonly Gender FixedGender;
        internal readonly Nature FixedNature;
        internal GenderRatio GenderRatio { get { return Pokemon.GenderRatio; } }
        internal Individual GetIndividual(IndivKernel Kernel)
        {
            return Pokemon.GetIndividual(Kernel);
        }
        internal GCPokemon(Pokemon p, Gender g = Gender.Genderless, Nature n = Nature.other)
        {
            Pokemon = p;
            FixedGender = g;
            FixedNature = n;
        }
    }

    internal class RentalParty
    {
        private GCPokemon[] Party { get; set; }
        internal GCPokemon this[int index] { get { return Party[index]; } }
        internal GCPokemon this[uint index] { get { return Party[index]; } }
        private RentalParty() { }

        static RentalParty Dommy1 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.FemaleOnly), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M3F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Male),
            }
        };
        static RentalParty Dommy2 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Male),
            }
        };
        static RentalParty Dommy3 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F3), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M7F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
            }
        };
        static RentalParty Dommy4 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.FemaleOnly), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Female),
                new GCPokemon(Pokemon.GetDommyPokemon(GenderRatio.M1F1), Gender.Male),
            }
        };
        static internal RentalParty[] DommyParty = new RentalParty[] { Dommy1, Dommy2, Dommy3, Dommy4 };

        static RentalParty Ultimate1 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("バシャーモ"), Gender.Male, Nature.Sassy),
                new GCPokemon(Pokemon.GetPokemon("ラフレシア"), Gender.Female, Nature.Gentle),
                new GCPokemon(Pokemon.GetPokemon("ランターン"), Gender.Female, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("オニゴーリ"), Gender.Male, Nature.Rash),
                new GCPokemon(Pokemon.GetPokemon("グランブル"), Gender.Male, Nature.Naughty),
                new GCPokemon(Pokemon.GetPokemon("ジュペッタ"), Gender.Female, Nature.Naughty),
            }
        };
        static RentalParty Ultimate2 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("エンテイ"), Gender.Genderless, Nature.Hasty),
                new GCPokemon(Pokemon.GetPokemon("ゴローニャ"), Gender.Female, Nature.Impish),
                new GCPokemon(Pokemon.GetPokemon("ベトベトン"), Gender.Male, Nature.Lonely),
                new GCPokemon(Pokemon.GetPokemon("コータス"), Gender.Male, Nature.Mild),
                new GCPokemon(Pokemon.GetPokemon("ライボルト"), Gender.Female, Nature.Mild),
                new GCPokemon(Pokemon.GetPokemon("ドククラゲ"), Gender.Male, Nature.Serious),
            }
        };
        static RentalParty Ultimate3 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("ラグラージ"), Gender.Male, Nature.Brave),
                new GCPokemon(Pokemon.GetPokemon("フーディン"), Gender.Female, Nature.Mild),
                new GCPokemon(Pokemon.GetPokemon("ルンパッパ"), Gender.Male, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("トドゼルガ"), Gender.Female, Nature.Bashful),
                new GCPokemon(Pokemon.GetPokemon("ゴルダック"), Gender.Male, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("バクオング"), Gender.Female, Nature.Adamant),
            }
        };
        static RentalParty Ultimate4 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("ライコウ"), Gender.Genderless, Nature.Mild),
                new GCPokemon(Pokemon.GetPokemon("キュウコン"), Gender.Female, Nature.Rash),
                new GCPokemon(Pokemon.GetPokemon("マタドガス"), Gender.Female, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ツボツボ"), Gender.Female, Nature.Sassy),
                new GCPokemon(Pokemon.GetPokemon("アーマルド"), Gender.Male, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ネイティオ"), Gender.Male, Nature.Quirky),
            }
        };
        static RentalParty Ultimate5 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("メガニウム"), Gender.Male, Nature.Quiet),
                new GCPokemon(Pokemon.GetPokemon("バクフーン"), Gender.Male, Nature.Mild),
                new GCPokemon(Pokemon.GetPokemon("オーダイル"), Gender.Male, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("エーフィ"), Gender.Male, Nature.Rash),
                new GCPokemon(Pokemon.GetPokemon("ブラッキー"), Gender.Male, Nature.Bold),
                new GCPokemon(Pokemon.GetPokemon("カイロス"), Gender.Female, Nature.Naughty),
            }
        };
        static RentalParty Ultimate6 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("スイクン"), Gender.Genderless, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("デンリュウ"), Gender.Female, Nature.Quiet),
                new GCPokemon(Pokemon.GetPokemon("ネンドール"), Gender.Genderless, Nature.Lonely),
                new GCPokemon(Pokemon.GetPokemon("オドシシ"), Gender.Male, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ポリゴン2"), Gender.Genderless, Nature.Rash),
                new GCPokemon(Pokemon.GetPokemon("ドンファン"), Gender.Female, Nature.Adamant),
            }
        };
        static RentalParty Ultimate7 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("メタグロス"), Gender.Genderless, Nature.Lonely),
                new GCPokemon(Pokemon.GetPokemon("ユレイドル"), Gender.Male, Nature.Impish),
                new GCPokemon(Pokemon.GetPokemon("カイリキー"), Gender.Male, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("エアームド"), Gender.Female, Nature.Lonely),
                new GCPokemon(Pokemon.GetPokemon("サイドン"), Gender.Female, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ハリテヤマ"), Gender.Male, Nature.Adamant),
            }
        };
        static RentalParty Ultimate8 = new RentalParty()
        {
            Party = new GCPokemon[] {
                new GCPokemon(Pokemon.GetPokemon("ヘラクロス"), Gender.Female, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ソーナンス"), Gender.Male, Nature.Timid),
                new GCPokemon(Pokemon.GetPokemon("ミロカロス"), Gender.Female, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("ドードリオ"), Gender.Male, Nature.Adamant),
                new GCPokemon(Pokemon.GetPokemon("ノクタス"), Gender.Female, Nature.Modest),
                new GCPokemon(Pokemon.GetPokemon("ヤミラミ"), Gender.Male, Nature.Adamant),
            }
        };

        static internal RentalParty[] UltimateParty = new RentalParty[] { Ultimate1, Ultimate2, Ultimate3, Ultimate4, Ultimate5, Ultimate6, Ultimate7, Ultimate8 };
    }

    public class DarkPokemon
    {
        internal GCPokemon TargetPokemon;
        internal GCPokemon[] PreGeneratePokemons = new GCPokemon[0];
        internal DarkPokemon(GCPokemon TargetDarkPokemon) { TargetPokemon = TargetDarkPokemon; }
        public Individual Generate(uint seed)
        {
            uint DommyTSV = seed.GetRand() ^ seed.GetRand();
            seed.Advance(2);
            foreach(var p in PreGeneratePokemons)
            {
                seed.GenerateIndividual(DommyTSV, p.FixedGender, p.GenderRatio, p.FixedNature);
            }
            return TargetPokemon.GetIndividual(seed.GenerateIndividual(DommyTSV));
        }
        static public string[] GetCODarkPokemonList()
        {
            return CODarkPokemons.Select(_ => _.TargetPokemon.Pokemon.Name).ToArray();
        }
        static public List<DarkPokemon> CODarkPokemons { get; private set; }
        static DarkPokemon()
        {
            CODarkPokemons = new List<DarkPokemon>();
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("マクノシタ")))
            {
                PreGeneratePokemons = new GCPokemon[] {
                    new GCPokemon(Pokemon.GetPokemon("ヨマワル"), Gender.Male, Nature.Quirky),
                    new GCPokemon(Pokemon.GetPokemon("イトマル"), Gender.Female, Nature.Hardy)
                }
            });
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ベイリーフ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("マグマラシ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("アリゲイツ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヨルノズク"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("モココ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ポポッコ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヌオー"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ムウマ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("マグマッグ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("オオタチ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヤンヤンマ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("テッポウオ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("マンタイン"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ハリーセン"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("アサナン"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ノコッチ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("チルット"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ウソッキー"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("カポエラー"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("エンテイ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("レディアン"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("スイクン"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("グライガー")))
            {
                PreGeneratePokemons = new GCPokemon[] {
                    new GCPokemon(Pokemon.GetPokemon("ヒメグマ"), Gender.Male, Nature.Serious),
                    new GCPokemon(Pokemon.GetPokemon("プリン"), Gender.Female, Nature.Docile),
                    new GCPokemon(Pokemon.GetPokemon("キノココ"), Gender.Male, Nature.Bashful)
                }
            });
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("オドシシ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("イノムー"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ニューラ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("エイパム"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヤミカラス")))
            {
                PreGeneratePokemons = new GCPokemon[] {
                    new GCPokemon(Pokemon.GetPokemon("キバニア"), Gender.Male, Nature.Docile),
                    new GCPokemon(Pokemon.GetPokemon("コノハナ"), Gender.Female, Nature.Serious),
                    new GCPokemon(Pokemon.GetPokemon("デルビル"), Gender.Male, Nature.Bashful)
                }
            });
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("フォレトス"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("アリアドス"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("グランブル"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ビブラーバ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ライコウ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("キマワリ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("デリバード"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヘラクロス")))
            {
                PreGeneratePokemons = new GCPokemon[] {
                    new GCPokemon(Pokemon.GetPokemon("アメモース"), Gender.Male, Nature.Hardy),
                    new GCPokemon(Pokemon.GetPokemon("アリアドス"), Gender.Female, Nature.Hardy),
                }
            });
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("エアームド"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ミルタンク"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("アブソル"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ヘルガー"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("トロピウス"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("メタグロス"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("バンギラス"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ドーブル"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("リングマ")))
            {
                PreGeneratePokemons = new GCPokemon[] {
                    new GCPokemon(Pokemon.GetPokemon("ゴーリキー"), Gender.Female, Nature.Calm),
                    new GCPokemon(Pokemon.GetPokemon("ヌマクロー"), Gender.Male, Nature.Mild),
                    new GCPokemon(Pokemon.GetPokemon("ダーテング"), Gender.Female, Nature.Gentle)
                }
            });
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("ツボツボ"))));
            CODarkPokemons.Add(new DarkPokemon(new GCPokemon(Pokemon.GetPokemon("トゲチック"))));
        }
    }

    public static class GenerateModules
    {
        public static uint GetPID(ref this uint seed)
        {
            return (seed.GetRand() << 16) | seed.GetRand();
        }
        public static uint GetPID(ref this uint seed, Func<uint, bool> condition)
        {
            uint PID;
            do { PID = (seed.GetRand() << 16) | seed.GetRand(); } while (!condition(PID));
            return PID;
        }
        public static uint[] GetIVs(ref this uint seed)
        {
            uint HAB = seed.GetRand();
            uint SCD = seed.GetRand();
            return new uint[6] {
                HAB & 0x1f,
                (HAB >> 5) & 0x1f,
                (HAB >> 10) & 0x1f,
                (SCD >> 5) & 0x1f,
                (SCD >> 10) & 0x1f,
                SCD & 0x1f
            };
        }
        public static bool isShiny(this uint PID, uint TSV) { return ((PID & 0xFFFF) ^ (PID >> 16) ^ TSV) < 8; }
        public static Gender GetGender(this uint PID, GenderRatio ratio)
        {
            if (ratio == GenderRatio.Genderless) return Gender.Genderless;
            return (PID & 0xFF) < (uint)ratio ? Gender.Female : Gender.Male;
        }
    }

}
