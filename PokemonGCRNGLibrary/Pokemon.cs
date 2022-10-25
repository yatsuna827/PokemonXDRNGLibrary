using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class Pokemon
    {
        public class Species
        {
            public readonly string Name;
            public readonly uint[] BS;
            public readonly string[] Ability;
            public readonly PokeType[] Type;
            public readonly GenderRatio GenderRatio;
            public readonly bool Hatchable;
            public readonly string FormName;
            public string GetFullName() { return Name + FormName; }

            public Individual GetIndividual(uint PID, uint Lv, uint[] IVs)
            {
                return new Individual()
                {
                    Species = this,
                    Name = Name,
                    Form = FormName,
                    Lv = Lv,
                    PID = PID,
                    Stats = GetStats(IVs, (Nature)(PID % 25), Lv),
                    IVs = IVs,
                    EVs = new uint[6],
                    Nature = (Nature)(PID % 25),
                    Ability = Ability[PID & 1],
                    Gender = PID.GetGender(GenderRatio),
                    HiddenPower = CalcHiddenPower(IVs),
                    HiddenPowerType = CalcHiddenPowerType(IVs)
                };
            }
            public Individual GetIndividual(uint PID, uint Lv, uint[] IVs, uint[] EVs)
            {
                return new Individual()
                {
                    Species = this,
                    Name = Name,
                    Form = FormName,
                    Lv = Lv,
                    PID = PID,
                    Stats = GetStats(IVs, EVs, (Nature)(PID % 25), Lv),
                    IVs = IVs,
                    EVs = EVs,
                    Nature = (Nature)(PID % 25),
                    Ability = Ability[PID & 1],
                    Gender = PID.GetGender(GenderRatio),
                    HiddenPower = CalcHiddenPower(IVs),
                    HiddenPowerType = CalcHiddenPowerType(IVs)
                };
            }

            public (uint[] min, uint[] max) CalcIVsRange(uint[] stats, uint lv, Nature nature)
            {
                double[] mag = nature.ToMagnification();
                var ivs = Enumerable.Range(0, 32).Select(_ => (uint)_);

                var preimageIVs = new uint[][]
                {
                    ivs.Where(iv => (iv + BS[0] * 2) * lv / 100 + 10 + lv == stats[0]).ToArray(),
                    ivs.Where(iv => (uint)(((iv + BS[1] * 2) * lv / 100 + 5) * mag[1]) == stats[1]).ToArray(),
                    ivs.Where(iv => (uint)(((iv + BS[2] * 2) * lv / 100 + 5) * mag[2]) == stats[2]).ToArray(),
                    ivs.Where(iv => (uint)(((iv + BS[3] * 2) * lv / 100 + 5) * mag[3]) == stats[3]).ToArray(),
                    ivs.Where(iv => (uint)(((iv + BS[4] * 2) * lv / 100 + 5) * mag[4]) == stats[4]).ToArray(),
                    ivs.Where(iv => (uint)(((iv + BS[5] * 2) * lv / 100 + 5) * mag[5]) == stats[5]).ToArray(),
                };

                var minIVs = preimageIVs.Select(_ => _.Length == 0 ? 32u : _.Min()).ToArray();
                var maxIVs = preimageIVs.Select(_ => _.Length == 0 ? 32u : _.Max()).ToArray();


                return (minIVs, maxIVs);
            }
            private uint[] GetStats(uint[] IVs, Nature Nature = Nature.Hardy, uint Lv = 50)
            {
                uint[] stats = new uint[6];
                double[] mag = Nature.ToMagnification();

                stats[0] = (IVs[0] + BS[0] * 2) * Lv / 100 + 10 + Lv;
                if (Name == "ヌケニン") stats[0] = 1;
                for (int i = 1; i < 6; i++)
                    stats[i] = (uint)(((IVs[i] + BS[i] * 2) * Lv / 100 + 5) * mag[i]);

                return stats;
            }
            private uint[] GetStats(uint[] IVs, uint[] EVs, Nature Nature = Nature.Hardy, uint Lv = 50)
            {
                uint[] stats = new uint[6];
                double[] mag = Nature.ToMagnification();

                stats[0] = (IVs[0] + EVs[0] / 4 + BS[0] * 2) * Lv / 100 + 10 + Lv;
                if (Name == "ヌケニン") stats[0] = 1;
                for (int i = 1; i < 6; i++)
                    stats[i] = (uint)(((IVs[i] + EVs[i] / 4 + BS[i] * 2) * Lv / 100 + 5) * mag[i]);

                return stats;
            }

            private static uint CalcHiddenPower(uint[] IVs)
            {
                uint num = ((IVs[0] >> 1) & 1) + 2 * ((IVs[1] >> 1) & 1) + 4 * ((IVs[2] >> 1) & 1) + 8 * ((IVs[5] >> 1) & 1) + 16 * ((IVs[3] >> 1) & 1) + 32 * ((IVs[4] >> 1) & 1);

                return num * 40 / 63 + 30;
            }

            private readonly PokeType[] HiddenPowerTypes = new PokeType[]
            {
                PokeType.Fighting, PokeType.Flying, PokeType.Poison, PokeType.Ground, PokeType.Rock, PokeType.Bug, PokeType.Ghost, PokeType.Steel,
                PokeType.Fire, PokeType.Water, PokeType.Grass, PokeType.Electric, PokeType.Psychic, PokeType.Ice, PokeType.Dragon, PokeType.Dark
            };
            private PokeType CalcHiddenPowerType(uint[] IVs)
            {
                // 格闘, 飛行, 毒, 地面, 岩, 虫, 霊, 鋼, 炎, 水, 草, 電気, 超, 氷, 龍, 悪
                uint num = (IVs[0] & 1) + 2 * (IVs[1] & 1) + 4 * (IVs[2] & 1) + 8 * (IVs[5] & 1) + 16 * (IVs[3] & 1) + 32 * (IVs[4] & 1);
                return HiddenPowerTypes[(num * 15 / 63)];
            }


            internal Species(string name, uint[] bs, PokeType[] type, string[] ability, bool hatchable, GenderRatio ratio)
            {
                Name = name;
                BS = bs;
                Ability = ability;
                Type = type;
                GenderRatio = ratio;
                Hatchable = hatchable;
                FormName = "";
            }
            internal Species(string name, string FormName, uint[] bs, PokeType[] type, string[] ability, bool hatchable, GenderRatio ratio)
            {
                Name = name;
                BS = bs;
                Ability = ability;
                Type = type;
                GenderRatio = ratio;
                Hatchable = hatchable;
                this.FormName = FormName;
            }

        }

        public class Individual
        {
            public Species Species { get; internal set; }
            public string Name { get; internal set; }
            public string Form { get; internal set; }
            public uint Lv { get; internal set; }
            public uint PID { get; internal set; }
            public Nature Nature { get; internal set; }
            public Gender Gender { get; internal set; }
            public string Ability { get; internal set; }
            public uint[] IVs { get; internal set; }
            public uint[] EVs { get; internal set; }
            public uint[] Stats { get; internal set; }

            public uint HiddenPower;
            public PokeType HiddenPowerType;

            public uint[] SimulateStats(uint Lv)
            {
                uint[] stats = new uint[6];
                double[] mag = Nature.ToMagnification();

                stats[0] = (IVs[0] + Species.BS[0] * 2) * Lv / 100 + 10 + Lv;
                if (Name == "ヌケニン") stats[0] = 1;
                for (int i = 1; i < 6; i++)
                    stats[i] = (uint)(((IVs[i] + Species.BS[i] * 2) * Lv / 100 + 5) * mag[i]);

                return stats;
            }

            public bool isShiny(uint TSV) { return (TSV ^ (PID & 0xFFFF) ^ (PID >> 16)) < 8; }
            internal Individual() { }

            public static Individual Empty = GetPokemon("Dummy").GetIndividual(0, 1, new uint[6]);
        }

        private static readonly List<Species> DexData;
        private static readonly List<Species> UniqueData;
        private static readonly Dictionary<string, Species> UniqueDex;
        private static readonly Dictionary<string, Species> DexDictionary;

        public static Species[] AllPokemonList { get { return DexData.ToArray(); } }

        private Pokemon() { }
        public static Species GetPokemon(int index) { return UniqueData[index]; }
        public static Species GetPokemon(string Name) { return UniqueDex[Name]; }
        public static Species GetPokemon(string Name, string Form)
        {
            return DexDictionary[Name + Form];
        }
        public static Species GetDummy(GenderRatio ratio)
        {
            return new Species("Dummy", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[2], false, ratio);
        }

        static Pokemon()
        {
            DexData = new List<Species>();
            DexData.Add(new Species("Dummy", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Non, PokeType.Non }, new string[] { "特性1", "特性2" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("フシギダネ", new uint[] { 45, 49, 49, 65, 65, 45 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "しんりょく" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("フシギソウ", new uint[] { 60, 62, 63, 80, 80, 60 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("フシギバナ", new uint[] { 80, 82, 83, 100, 100, 80 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ヒトカゲ", new uint[] { 39, 52, 43, 60, 50, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("リザード", new uint[] { 58, 64, 58, 80, 65, 80 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("リザードン", new uint[] { 78, 84, 78, 109, 85, 100 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ゼニガメ", new uint[] { 44, 48, 65, 50, 64, 43 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("カメール", new uint[] { 59, 63, 80, 65, 80, 58 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("カメックス", new uint[] { 79, 83, 100, 85, 105, 78 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("キャタピー", new uint[] { 45, 30, 35, 20, 20, 45 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "りんぷん", "りんぷん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("トランセル", new uint[] { 50, 20, 55, 25, 25, 30 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("バタフリー", new uint[] { 60, 45, 50, 80, 80, 70 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "ふくがん", "ふくがん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ビードル", new uint[] { 40, 35, 30, 20, 20, 50 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "りんぷん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("コクーン", new uint[] { 45, 25, 50, 25, 25, 35 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("スピアー", new uint[] { 65, 80, 40, 45, 80, 75 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "むしのしらせ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ポッポ", new uint[] { 40, 45, 40, 35, 35, 56 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ピジョン", new uint[] { 63, 60, 55, 50, 50, 71 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ピジョット", new uint[] { 83, 80, 75, 70, 70, 91 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("コラッタ", new uint[] { 30, 56, 35, 25, 35, 72 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ラッタ", new uint[] { 55, 81, 60, 50, 70, 97 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "こんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("オニスズメ", new uint[] { 40, 60, 30, 31, 31, 70 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オニドリル", new uint[] { 65, 90, 65, 61, 61, 100 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("アーボ", new uint[] { 35, 60, 44, 40, 54, 55 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "いかく", "だっぴ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アーボック", new uint[] { 60, 85, 69, 65, 79, 80 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "いかく", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ピカチュウ", new uint[] { 35, 55, 30, 50, 40, 90 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ライチュウ", new uint[] { 60, 90, 55, 90, 80, 100 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サンド", new uint[] { 50, 75, 85, 20, 30, 40 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "すながくれ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("サンドパン", new uint[] { 75, 100, 110, 45, 55, 65 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "すながくれ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ニドラン♀", new uint[] { 55, 47, 52, 40, 40, 41 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ニドリーナ", new uint[] { 70, 62, 67, 55, 55, 56 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ニドクイン", new uint[] { 90, 82, 87, 75, 85, 76 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ニドラン♂", new uint[] { 46, 57, 40, 40, 40, 50 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Species("ニドリーノ", new uint[] { 61, 72, 57, 55, 55, 65 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("ニドキング", new uint[] { 81, 92, 77, 85, 75, 85 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("ピッピ", new uint[] { 70, 45, 48, 60, 65, 35 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("ピクシー", new uint[] { 95, 70, 73, 85, 90, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("ロコン", new uint[] { 38, 41, 40, 50, 65, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "もらいび" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("キュウコン", new uint[] { 73, 76, 75, 81, 100, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "もらいび" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("プリン", new uint[] { 115, 45, 20, 45, 25, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("プクリン", new uint[] { 140, 70, 45, 75, 50, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("ズバット", new uint[] { 40, 45, 35, 30, 40, 55 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "せいしんりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ゴルバット", new uint[] { 75, 80, 70, 65, 75, 90 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "せいしんりょく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ナゾノクサ", new uint[] { 45, 50, 55, 75, 65, 30 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("クサイハナ", new uint[] { 60, 65, 70, 85, 75, 40 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ラフレシア", new uint[] { 75, 80, 85, 100, 90, 50 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("パラス", new uint[] { 35, 70, 55, 45, 55, 25 }, new PokeType[] { PokeType.Bug, PokeType.Grass }, new string[] { "ほうし", "ほうし" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("パラセクト", new uint[] { 60, 95, 80, 60, 80, 30 }, new PokeType[] { PokeType.Bug, PokeType.Grass }, new string[] { "ほうし", "ほうし" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("コンパン", new uint[] { 60, 55, 50, 40, 55, 45 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "ふくがん", "ふくがん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("モルフォン", new uint[] { 70, 65, 60, 90, 75, 90 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "りんぷん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ディグダ", new uint[] { 10, 55, 25, 35, 45, 95 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "ありじごく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ダグトリオ", new uint[] { 35, 80, 50, 50, 70, 120 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "ありじごく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ニャース", new uint[] { 40, 45, 35, 40, 40, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ペルシアン", new uint[] { 65, 70, 60, 65, 65, 115 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "じゅうなん", "じゅうなん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("コダック", new uint[] { 50, 52, 48, 65, 50, 55 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "しめりけ", "ノーてんき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ゴルダック", new uint[] { 80, 82, 78, 95, 80, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "しめりけ", "ノーてんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("マンキー", new uint[] { 40, 80, 35, 35, 45, 70 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "やるき", "やるき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オコリザル", new uint[] { 65, 105, 60, 60, 70, 95 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "やるき", "やるき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ガーディ", new uint[] { 55, 70, 45, 70, 50, 60 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "いかく", "もらいび" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ウインディ", new uint[] { 90, 110, 80, 100, 80, 95 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "いかく", "もらいび" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("ニョロモ", new uint[] { 40, 50, 40, 40, 40, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ニョロゾ", new uint[] { 65, 65, 65, 50, 50, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ニョロボン", new uint[] { 90, 85, 95, 70, 90, 70 }, new PokeType[] { PokeType.Water, PokeType.Fighting }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ケーシィ", new uint[] { 25, 20, 15, 105, 55, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ユンゲラー", new uint[] { 40, 35, 30, 120, 70, 105 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("フーディン", new uint[] { 55, 50, 45, 135, 85, 120 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("ワンリキー", new uint[] { 70, 80, 50, 35, 35, 35 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "こんじょう" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ゴーリキー", new uint[] { 80, 100, 70, 50, 60, 45 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "こんじょう" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("カイリキー", new uint[] { 90, 130, 80, 65, 85, 55 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "こんじょう" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("マダツボミ", new uint[] { 50, 75, 35, 70, 30, 40 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ウツドン", new uint[] { 65, 90, 50, 85, 45, 55 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ウツボット", new uint[] { 80, 105, 65, 100, 60, 70 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("メノクラゲ", new uint[] { 40, 40, 35, 50, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "クリアボディ", "ヘドロえき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドククラゲ", new uint[] { 80, 70, 65, 80, 120, 100 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "クリアボディ", "ヘドロえき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("イシツブテ", new uint[] { 40, 80, 100, 30, 30, 20 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ゴローン", new uint[] { 55, 95, 115, 45, 45, 35 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ゴローニャ", new uint[] { 80, 110, 130, 55, 65, 45 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ポニータ", new uint[] { 50, 85, 55, 65, 65, 90 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "にげあし", "もらいび" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ギャロップ", new uint[] { 65, 100, 70, 80, 80, 105 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "にげあし", "もらいび" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヤドン", new uint[] { 90, 65, 65, 40, 40, 15 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヤドラン", new uint[] { 95, 75, 110, 100, 80, 30 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("コイル", new uint[] { 25, 35, 70, 95, 55, 45 }, new PokeType[] { PokeType.Electric, PokeType.Steel }, new string[] { "じりょく", "がんじょう" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("レアコイル", new uint[] { 50, 60, 95, 120, 70, 70 }, new PokeType[] { PokeType.Electric, PokeType.Steel }, new string[] { "じりょく", "がんじょう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("カモネギ", new uint[] { 52, 65, 55, 58, 62, 60 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "せいしんりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドードー", new uint[] { 35, 85, 45, 35, 35, 75 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "にげあし", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドードリオ", new uint[] { 60, 110, 70, 60, 60, 100 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "にげあし", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("パウワウ", new uint[] { 65, 45, 55, 45, 70, 45 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "あついしぼう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ジュゴン", new uint[] { 90, 70, 80, 70, 95, 70 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "あついしぼう", "あついしぼう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ベトベター", new uint[] { 80, 80, 50, 40, 50, 25 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "あくしゅう", "ねんちゃく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ベトベトン", new uint[] { 105, 105, 75, 65, 100, 50 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "あくしゅう", "ねんちゃく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("シェルダー", new uint[] { 30, 65, 100, 45, 25, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "シェルアーマー", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("パルシェン", new uint[] { 50, 95, 180, 85, 45, 70 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "シェルアーマー", "シェルアーマー" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ゴース", new uint[] { 30, 35, 30, 100, 35, 80 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ゴースト", new uint[] { 45, 50, 45, 115, 55, 95 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ゲンガー", new uint[] { 60, 65, 60, 130, 75, 110 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("イワーク", new uint[] { 35, 45, 160, 30, 45, 70 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("スリープ", new uint[] { 60, 48, 45, 43, 90, 42 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふみん", "ふみん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("スリーパー", new uint[] { 85, 73, 70, 73, 115, 67 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふみん", "ふみん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("クラブ", new uint[] { 30, 105, 90, 25, 25, 50 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("キングラー", new uint[] { 55, 130, 115, 50, 50, 75 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ビリリダマ", new uint[] { 40, 30, 50, 55, 55, 100 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ぼうおん", "せいでんき" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("マルマイン", new uint[] { 60, 50, 70, 80, 80, 140 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ぼうおん", "せいでんき" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("タマタマ", new uint[] { 60, 40, 80, 60, 45, 40 }, new PokeType[] { PokeType.Grass, PokeType.Psychic }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ナッシー", new uint[] { 95, 95, 85, 125, 65, 55 }, new PokeType[] { PokeType.Grass, PokeType.Psychic }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("カラカラ", new uint[] { 50, 50, 95, 40, 50, 35 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "いしあたま", "ひらいしん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ガラガラ", new uint[] { 60, 80, 110, 50, 80, 45 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "いしあたま", "ひらいしん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サワムラー", new uint[] { 50, 120, 53, 35, 110, 87 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "じゅうなん", "じゅうなん" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("エビワラー", new uint[] { 50, 105, 79, 35, 110, 76 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "するどいめ", "するどいめ" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("ベロリンガ", new uint[] { 90, 55, 75, 60, 75, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドガース", new uint[] { 40, 65, 95, 60, 45, 35 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マタドガス", new uint[] { 65, 90, 120, 85, 70, 60 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サイホーン", new uint[] { 80, 85, 95, 30, 30, 25 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "ひらいしん", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("サイドン", new uint[] { 105, 130, 120, 45, 45, 40 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "ひらいしん", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ラッキー", new uint[] { 250, 5, 5, 35, 105, 50 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "しぜんかいふく", "てんのめぐみ" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("モンジャラ", new uint[] { 65, 55, 115, 100, 40, 60 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ガルーラ", new uint[] { 105, 95, 80, 40, 80, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "はやおき", "はやおき" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("タッツー", new uint[] { 30, 40, 70, 70, 25, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("シードラ", new uint[] { 55, 65, 95, 95, 45, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "どくのトゲ", "どくのトゲ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("トサキント", new uint[] { 45, 67, 60, 35, 50, 63 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "みずのベール" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アズマオウ", new uint[] { 80, 92, 65, 65, 80, 68 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "みずのベール" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヒトデマン", new uint[] { 30, 45, 55, 70, 55, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "はっこう", "しぜんかいふく" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("スターミー", new uint[] { 60, 75, 85, 100, 85, 115 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "はっこう", "しぜんかいふく" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("バリヤード", new uint[] { 40, 45, 65, 100, 120, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ぼうおん", "ぼうおん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ストライク", new uint[] { 70, 110, 80, 55, 80, 105 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "むしのしらせ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ルージュラ", new uint[] { 65, 50, 35, 115, 95, 95 }, new PokeType[] { PokeType.Ice, PokeType.Psychic }, new string[] { "どんかん", "どんかん" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Species("エレブー", new uint[] { 65, 83, 57, 95, 85, 105 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("ブーバー", new uint[] { 65, 95, 57, 100, 85, 93 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "ほのおのからだ", "ほのおのからだ" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("カイロス", new uint[] { 65, 125, 100, 55, 70, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "かいりきバサミ", "かいりきバサミ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ケンタロス", new uint[] { 75, 100, 95, 40, 70, 110 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "いかく" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Species("コイキング", new uint[] { 20, 10, 55, 15, 20, 80 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ギャラドス", new uint[] { 95, 125, 79, 60, 100, 81 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "いかく", "いかく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ラプラス", new uint[] { 130, 85, 80, 85, 95, 60 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "ちょすい", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("メタモン", new uint[] { 48, 48, 48, 48, 48, 48 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "じゅうなん", "じゅうなん" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("イーブイ", new uint[] { 55, 55, 50, 45, 65, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "にげあし" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("シャワーズ", new uint[] { 130, 65, 60, 110, 95, 65 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "ちょすい" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("サンダース", new uint[] { 65, 65, 60, 110, 95, 130 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ちくでん", "ちくでん" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ブースター", new uint[] { 65, 130, 60, 95, 110, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "もらいび" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ポリゴン", new uint[] { 65, 60, 70, 85, 75, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "トレース", "トレース" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("オムナイト", new uint[] { 35, 40, 100, 90, 55, 35 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "シェルアーマー" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("オムスター", new uint[] { 70, 60, 125, 115, 70, 55 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "シェルアーマー" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("カブト", new uint[] { 30, 80, 90, 55, 45, 55 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "カブトアーマー" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("カブトプス", new uint[] { 60, 115, 105, 65, 70, 80 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "カブトアーマー" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("プテラ", new uint[] { 80, 105, 65, 60, 75, 130 }, new PokeType[] { PokeType.Rock, PokeType.Flying }, new string[] { "いしあたま", "プレッシャー" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("カビゴン", new uint[] { 160, 110, 65, 65, 110, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "めんえき", "あついしぼう" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("フリーザー", new uint[] { 90, 85, 100, 95, 125, 85 }, new PokeType[] { PokeType.Ice, PokeType.Flying }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("サンダー", new uint[] { 90, 90, 85, 125, 90, 100 }, new PokeType[] { PokeType.Electric, PokeType.Flying }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ファイヤー", new uint[] { 90, 100, 90, 125, 85, 90 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ミニリュウ", new uint[] { 41, 64, 45, 50, 50, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハクリュー", new uint[] { 61, 84, 65, 70, 70, 70 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("カイリュー", new uint[] { 91, 134, 95, 100, 100, 80 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "せいしんりょく", "せいしんりょく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ミュウツー", new uint[] { 106, 110, 90, 154, 90, 130 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ミュウ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "シンクロ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("チコリータ", new uint[] { 45, 49, 65, 49, 65, 45 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ベイリーフ", new uint[] { 60, 62, 80, 63, 80, 60 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("メガニウム", new uint[] { 80, 82, 100, 83, 100, 80 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ヒノアラシ", new uint[] { 39, 52, 43, 60, 50, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("マグマラシ", new uint[] { 58, 64, 58, 80, 65, 80 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("バクフーン", new uint[] { 78, 84, 78, 109, 85, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ワニノコ", new uint[] { 50, 65, 64, 44, 48, 43 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("アリゲイツ", new uint[] { 65, 80, 80, 59, 63, 58 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("オーダイル", new uint[] { 85, 105, 100, 79, 83, 78 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("オタチ", new uint[] { 35, 46, 34, 35, 45, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オオタチ", new uint[] { 85, 76, 64, 45, 55, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ホーホー", new uint[] { 60, 30, 30, 36, 56, 50 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "ふみん", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヨルノズク", new uint[] { 100, 50, 50, 76, 96, 70 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "ふみん", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("レディバ", new uint[] { 40, 20, 30, 40, 80, 55 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("レディアン", new uint[] { 55, 35, 50, 55, 110, 85 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("イトマル", new uint[] { 40, 60, 40, 40, 40, 30 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "ふみん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アリアドス", new uint[] { 70, 90, 70, 60, 60, 40 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "ふみん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("クロバット", new uint[] { 85, 90, 80, 70, 80, 130 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "せいしんりょく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("チョンチー", new uint[] { 75, 38, 38, 56, 56, 67 }, new PokeType[] { PokeType.Water, PokeType.Electric }, new string[] { "ちくでん", "はっこう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ランターン", new uint[] { 125, 58, 58, 76, 76, 67 }, new PokeType[] { PokeType.Water, PokeType.Electric }, new string[] { "ちくでん", "はっこう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ピチュー", new uint[] { 20, 40, 15, 35, 35, 60 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ピィ", new uint[] { 50, 25, 28, 45, 55, 15 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("ププリン", new uint[] { 90, 30, 15, 40, 20, 15 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("トゲピー", new uint[] { 35, 20, 65, 40, 65, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "はりきり", "てんのめぐみ" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("トゲチック", new uint[] { 55, 40, 85, 80, 105, 40 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "はりきり", "てんのめぐみ" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ネイティ", new uint[] { 40, 50, 45, 70, 45, 70 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "シンクロ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ネイティオ", new uint[] { 65, 75, 70, 95, 70, 95 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "シンクロ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("メリープ", new uint[] { 55, 40, 40, 65, 45, 35 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("モココ", new uint[] { 70, 55, 55, 80, 60, 45 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("デンリュウ", new uint[] { 90, 75, 75, 115, 90, 55 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("キレイハナ", new uint[] { 75, 80, 85, 90, 100, 50 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("マリル", new uint[] { 70, 20, 50, 20, 50, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マリルリ", new uint[] { 100, 50, 80, 50, 80, 50 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ウソッキー", new uint[] { 70, 100, 115, 30, 65, 30 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "がんじょう", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ニョロトノ", new uint[] { 90, 75, 75, 90, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ハネッコ", new uint[] { 35, 35, 40, 35, 55, 50 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ポポッコ", new uint[] { 55, 45, 50, 45, 65, 80 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ワタッコ", new uint[] { 75, 55, 70, 55, 85, 110 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("エイパム", new uint[] { 55, 70, 55, 40, 55, 85 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヒマナッツ", new uint[] { 30, 30, 30, 30, 30, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("キマワリ", new uint[] { 75, 75, 55, 105, 85, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "ようりょくそ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヤンヤンマ", new uint[] { 65, 65, 45, 75, 45, 95 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "かそく", "ふくがん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ウパー", new uint[] { 55, 45, 45, 25, 25, 15 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "しめりけ", "ちょすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヌオー", new uint[] { 95, 85, 85, 65, 65, 35 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "しめりけ", "ちょすい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("エーフィ", new uint[] { 65, 65, 60, 130, 95, 110 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "シンクロ" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ブラッキー", new uint[] { 95, 65, 110, 60, 130, 65 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "シンクロ", "シンクロ" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ヤミカラス", new uint[] { 60, 85, 42, 85, 42, 91 }, new PokeType[] { PokeType.Dark, PokeType.Flying }, new string[] { "ふみん", "ふみん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヤドキング", new uint[] { 95, 75, 80, 100, 110, 30 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ムウマ", new uint[] { 60, 60, 60, 85, 85, 85 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アンノーン", "A", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "B", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "C", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "D", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "E", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "F", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "G", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "H", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "I", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "J", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "K", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "L", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "M", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "N", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "O", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "P", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "Q", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "R", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "S", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "T", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "U", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "V", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "W", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "X", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "Y", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "Z", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "!", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("アンノーン", "?", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ソーナンス", new uint[] { 190, 33, 58, 33, 58, 33 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "かげふみ", "かげふみ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("キリンリキ", new uint[] { 70, 80, 65, 90, 65, 85 }, new PokeType[] { PokeType.Normal, PokeType.Psychic }, new string[] { "せいしんりょく", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("クヌギダマ", new uint[] { 50, 65, 90, 35, 35, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "がんじょう", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("フォレトス", new uint[] { 75, 90, 140, 60, 60, 40 }, new PokeType[] { PokeType.Bug, PokeType.Steel }, new string[] { "がんじょう", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ノコッチ", new uint[] { 100, 70, 70, 65, 65, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "てんのめぐみ", "にげあし" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("グライガー", new uint[] { 65, 75, 105, 35, 65, 85 }, new PokeType[] { PokeType.Ground, PokeType.Flying }, new string[] { "かいりきバサミ", "すながくれ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハガネール", new uint[] { 75, 85, 200, 55, 65, 30 }, new PokeType[] { PokeType.Steel, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ブルー", new uint[] { 60, 80, 50, 40, 40, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "にげあし" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("グランブル", new uint[] { 90, 120, 75, 60, 60, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "いかく" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("ハリーセン", new uint[] { 65, 95, 75, 55, 55, 85 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "どくのトゲ", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハッサム", new uint[] { 70, 130, 100, 55, 80, 65 }, new PokeType[] { PokeType.Bug, PokeType.Steel }, new string[] { "むしのしらせ", "むしのしらせ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ツボツボ", new uint[] { 20, 10, 230, 10, 230, 5 }, new PokeType[] { PokeType.Bug, PokeType.Rock }, new string[] { "がんじょう", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヘラクロス", new uint[] { 80, 125, 75, 40, 95, 85 }, new PokeType[] { PokeType.Bug, PokeType.Fighting }, new string[] { "むしのしらせ", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ニューラ", new uint[] { 55, 95, 55, 35, 75, 115 }, new PokeType[] { PokeType.Dark, PokeType.Ice }, new string[] { "せいしんりょく", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヒメグマ", new uint[] { 60, 80, 50, 50, 50, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("リングマ", new uint[] { 90, 130, 75, 75, 75, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "こんじょう", "こんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("マグマッグ", new uint[] { 40, 40, 40, 70, 40, 20 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "マグマのよろい", "ほのおのからだ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マグカルゴ", new uint[] { 50, 50, 120, 80, 80, 30 }, new PokeType[] { PokeType.Fire, PokeType.Rock }, new string[] { "マグマのよろい", "ほのおのからだ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ウリムー", new uint[] { 50, 50, 40, 30, 30, 50 }, new PokeType[] { PokeType.Ice, PokeType.Ground }, new string[] { "どんかん", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("イノムー", new uint[] { 100, 100, 80, 60, 60, 50 }, new PokeType[] { PokeType.Ice, PokeType.Ground }, new string[] { "どんかん", "どんかん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サニーゴ", new uint[] { 55, 55, 85, 65, 85, 35 }, new PokeType[] { PokeType.Water, PokeType.Rock }, new string[] { "はりきり", "しぜんかいふく" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("テッポウオ", new uint[] { 35, 65, 35, 65, 35, 65 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "はりきり", "はりきり" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オクタン", new uint[] { 75, 105, 75, 105, 75, 45 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "きゅうばん", "きゅうばん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("デリバード", new uint[] { 45, 55, 45, 65, 45, 75 }, new PokeType[] { PokeType.Ice, PokeType.Flying }, new string[] { "やるき", "はりきり" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マンタイン", new uint[] { 65, 40, 70, 80, 140, 70 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "すいすい", "ちょすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("エアームド", new uint[] { 65, 80, 140, 40, 70, 70 }, new PokeType[] { PokeType.Steel, PokeType.Flying }, new string[] { "するどいめ", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("デルビル", new uint[] { 45, 60, 30, 80, 50, 65 }, new PokeType[] { PokeType.Dark, PokeType.Fire }, new string[] { "はやおき", "もらいび" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヘルガー", new uint[] { 75, 90, 50, 110, 80, 95 }, new PokeType[] { PokeType.Dark, PokeType.Fire }, new string[] { "はやおき", "もらいび" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("キングドラ", new uint[] { 75, 95, 95, 95, 95, 85 }, new PokeType[] { PokeType.Water, PokeType.Dragon }, new string[] { "すいすい", "すいすい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ゴマゾウ", new uint[] { 90, 60, 60, 40, 40, 40 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "ものひろい", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドンファン", new uint[] { 90, 120, 120, 60, 60, 50 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "がんじょう", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ポリゴン2", new uint[] { 85, 80, 90, 105, 95, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "トレース", "トレース" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("オドシシ", new uint[] { 73, 95, 62, 85, 65, 85 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "いかく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドーブル", new uint[] { 55, 20, 35, 20, 45, 75 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("バルキー", new uint[] { 35, 35, 35, 35, 35, 35 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "こんじょう" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Species("カポエラー", new uint[] { 50, 95, 95, 35, 110, 70 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "いかく", "いかく" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("ムチュール", new uint[] { 45, 30, 15, 85, 65, 65 }, new PokeType[] { PokeType.Ice, PokeType.Psychic }, new string[] { "どんかん", "どんかん" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("エレキッド", new uint[] { 45, 63, 37, 65, 55, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "せいでんき" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ブビィ", new uint[] { 45, 75, 37, 70, 55, 83 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "ほのおのからだ", "ほのおのからだ" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ミルタンク", new uint[] { 95, 80, 105, 40, 70, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "あついしぼう", "あついしぼう" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ハピナス", new uint[] { 255, 10, 10, 75, 135, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "しぜんかいふく", "てんのめぐみ" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ライコウ", new uint[] { 90, 85, 75, 115, 100, 115 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("エンテイ", new uint[] { 115, 115, 85, 90, 75, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("スイクン", new uint[] { 100, 75, 115, 90, 115, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ヨーギラス", new uint[] { 50, 64, 50, 45, 50, 41 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "こんじょう", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("サナギラス", new uint[] { 70, 84, 70, 65, 70, 51 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("バンギラス", new uint[] { 100, 134, 110, 95, 100, 61 }, new PokeType[] { PokeType.Rock, PokeType.Dark }, new string[] { "すなおこし", "すなおこし" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ルギア", new uint[] { 106, 90, 130, 90, 154, 110 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ホウオウ", new uint[] { 106, 130, 90, 110, 154, 90 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("セレビィ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Psychic, PokeType.Grass }, new string[] { "しぜんかいふく", "しぜんかいふく" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("キモリ", new uint[] { 40, 45, 35, 65, 55, 70 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ジュプトル", new uint[] { 50, 65, 45, 85, 65, 95 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ジュカイン", new uint[] { 70, 85, 65, 105, 85, 120 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "しんりょく" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("アチャモ", new uint[] { 45, 60, 40, 70, 50, 45 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "もうか" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ワカシャモ", new uint[] { 60, 85, 60, 85, 60, 55 }, new PokeType[] { PokeType.Fire, PokeType.Fighting }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("バシャーモ", new uint[] { 80, 120, 70, 110, 70, 80 }, new PokeType[] { PokeType.Fire, PokeType.Fighting }, new string[] { "もうか", "もうか" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ミズゴロウ", new uint[] { 50, 70, 50, 50, 50, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "げきりゅう" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ヌマクロー", new uint[] { 70, 85, 70, 60, 70, 50 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ラグラージ", new uint[] { 100, 110, 90, 85, 90, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "げきりゅう", "げきりゅう" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ポチエナ", new uint[] { 35, 55, 35, 30, 30, 35 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "にげあし", "にげあし" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("グラエナ", new uint[] { 70, 90, 70, 60, 60, 70 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "いかく", "いかく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ジグザグマ", new uint[] { 38, 30, 41, 30, 41, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マッスグマ", new uint[] { 78, 70, 61, 50, 61, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "ものひろい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ケムッソ", new uint[] { 45, 45, 35, 20, 30, 20 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "りんぷん", "りんぷん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("カラサリス", new uint[] { 50, 35, 55, 25, 25, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("アゲハント", new uint[] { 60, 70, 50, 90, 50, 65 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "むしのしらせ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("マユルド", new uint[] { 50, 35, 55, 25, 25, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ドクケイル", new uint[] { 60, 50, 70, 50, 90, 65 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "りんぷん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ハスボー", new uint[] { 40, 30, 30, 40, 50, 30 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハスブレロ", new uint[] { 60, 50, 50, 60, 70, 50 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ルンパッパ", new uint[] { 80, 70, 70, 90, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("タネボー", new uint[] { 40, 40, 50, 30, 30, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("コノハナ", new uint[] { 70, 70, 40, 60, 40, 60 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "ようりょくそ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ダーテング", new uint[] { 90, 100, 60, 90, 60, 80 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "ようりょくそ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("スバメ", new uint[] { 40, 55, 30, 30, 30, 85 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "こんじょう", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オオスバメ", new uint[] { 60, 85, 60, 50, 50, 125 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "こんじょう", "こんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("キャモメ", new uint[] { 40, 30, 30, 55, 30, 85 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ペリッパー", new uint[] { 60, 50, 100, 85, 70, 65 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "するどいめ", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ラルトス", new uint[] { 28, 25, 25, 45, 35, 40 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("キルリア", new uint[] { 38, 35, 35, 65, 55, 50 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サーナイト", new uint[] { 68, 65, 65, 125, 115, 80 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("アメタマ", new uint[] { 40, 30, 32, 50, 52, 65 }, new PokeType[] { PokeType.Bug, PokeType.Water }, new string[] { "すいすい", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アメモース", new uint[] { 70, 60, 62, 80, 82, 60 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "いかく", "いかく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("キノココ", new uint[] { 60, 40, 60, 40, 60, 35 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ほうし", "ほうし" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("キノガッサ", new uint[] { 60, 130, 80, 60, 60, 70 }, new PokeType[] { PokeType.Grass, PokeType.Fighting }, new string[] { "ほうし", "ほうし" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ナマケロ", new uint[] { 60, 60, 60, 35, 35, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "なまけ", "なまけ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ヤルキモノ", new uint[] { 80, 80, 80, 55, 55, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "やるき", "やるき" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ケッキング", new uint[] { 150, 160, 100, 95, 65, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "なまけ", "なまけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ツチニン", new uint[] { 31, 45, 90, 30, 30, 40 }, new PokeType[] { PokeType.Bug, PokeType.Ground }, new string[] { "ふくがん", "ふくがん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("テッカニン", new uint[] { 61, 90, 45, 50, 50, 160 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "かそく", "かそく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヌケニン", new uint[] { 1, 90, 45, 30, 30, 40 }, new PokeType[] { PokeType.Bug, PokeType.Ghost }, new string[] { "ふしぎなまもり", "ふしぎなまもり" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ゴニョニョ", new uint[] { 64, 51, 23, 51, 23, 28 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "ぼうおん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ドゴーム", new uint[] { 84, 71, 43, 71, 43, 48 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "ぼうおん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("バクオング", new uint[] { 104, 91, 63, 91, 63, 68 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "ぼうおん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("マクノシタ", new uint[] { 72, 60, 30, 20, 30, 25 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "あついしぼう", "こんじょう" }, true, GenderRatio.M3F1));
            DexData.Add(new Species("ハリテヤマ", new uint[] { 144, 120, 60, 40, 60, 50 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "あついしぼう", "こんじょう" }, false, GenderRatio.M3F1));
            DexData.Add(new Species("ルリリ", new uint[] { 50, 20, 40, 20, 40, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("ノズパス", new uint[] { 30, 45, 135, 45, 90, 30 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "がんじょう", "じりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("エネコ", new uint[] { 50, 45, 45, 35, 35, 50 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("エネコロロ", new uint[] { 70, 65, 65, 55, 55, 70 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "メロメロボディ" }, false, GenderRatio.M1F3));
            DexData.Add(new Species("ヤミラミ", new uint[] { 50, 75, 75, 65, 65, 50 }, new PokeType[] { PokeType.Dark, PokeType.Ghost }, new string[] { "するどいめ", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("クチート", new uint[] { 50, 85, 85, 55, 55, 50 }, new PokeType[] { PokeType.Steel, PokeType.Non }, new string[] { "かいりきバサミ", "いかく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ココドラ", new uint[] { 50, 70, 100, 40, 40, 30 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("コドラ", new uint[] { 60, 90, 140, 50, 50, 40 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ボスゴドラ", new uint[] { 70, 110, 180, 60, 60, 50 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("アサナン", new uint[] { 30, 40, 55, 40, 55, 60 }, new PokeType[] { PokeType.Fighting, PokeType.Psychic }, new string[] { "ヨガパワー", "ヨガパワー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("チャーレム", new uint[] { 60, 60, 75, 60, 75, 80 }, new PokeType[] { PokeType.Fighting, PokeType.Psychic }, new string[] { "ヨガパワー", "ヨガパワー" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ラクライ", new uint[] { 40, 45, 40, 65, 40, 65 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "ひらいしん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ライボルト", new uint[] { 70, 75, 60, 105, 60, 105 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "ひらいしん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("プラスル", new uint[] { 60, 50, 40, 85, 75, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "プラス", "プラス" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マイナン", new uint[] { 60, 40, 50, 75, 85, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "マイナス", "マイナス" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("バルビート", new uint[] { 65, 73, 55, 47, 75, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "はっこう", "むしのしらせ" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Species("イルミーゼ", new uint[] { 65, 47, 55, 73, 75, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "どんかん", "どんかん" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ロゼリア", new uint[] { 50, 60, 45, 100, 80, 65 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しぜんかいふく", "どくのトゲ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ゴクリン", new uint[] { 70, 43, 53, 43, 53, 40 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ヘドロえき", "ねんちゃく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("マルノーム", new uint[] { 100, 73, 83, 73, 83, 55 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ヘドロえき", "ねんちゃく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("キバニア", new uint[] { 45, 90, 20, 65, 20, 65 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "さめはだ", "さめはだ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("サメハダー", new uint[] { 70, 120, 40, 95, 40, 95 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "さめはだ", "さめはだ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ホエルコ", new uint[] { 130, 70, 35, 70, 35, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "みずのベール", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ホエルオー", new uint[] { 170, 90, 45, 90, 45, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "みずのベール", "どんかん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ドンメル", new uint[] { 60, 60, 40, 65, 45, 35 }, new PokeType[] { PokeType.Fire, PokeType.Ground }, new string[] { "どんかん", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("バクーダ", new uint[] { 70, 100, 70, 105, 75, 40 }, new PokeType[] { PokeType.Fire, PokeType.Ground }, new string[] { "マグマのよろい", "マグマのよろい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("コータス", new uint[] { 70, 85, 140, 85, 70, 20 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "しろいけむり", "しろいけむり" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("バネブー", new uint[] { 60, 25, 35, 70, 80, 60 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "あついしぼう", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ブーピッグ", new uint[] { 80, 45, 65, 90, 110, 80 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "あついしぼう", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("パッチール", new uint[] { 60, 60, 60, 60, 60, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ナックラー", new uint[] { 45, 100, 45, 45, 45, 10 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "かいりきバサミ", "ありじごく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ビブラーバ", new uint[] { 50, 70, 50, 50, 50, 70 }, new PokeType[] { PokeType.Ground, PokeType.Dragon }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("フライゴン", new uint[] { 80, 100, 80, 80, 80, 100 }, new PokeType[] { PokeType.Ground, PokeType.Dragon }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サボネア", new uint[] { 50, 85, 40, 85, 40, 35 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "すながくれ", "すながくれ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ノクタス", new uint[] { 70, 115, 60, 115, 60, 55 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "すながくれ", "すながくれ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("チルット", new uint[] { 45, 40, 60, 40, 75, 50 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "しぜんかいふく", "しぜんかいふく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("チルタリス", new uint[] { 75, 70, 90, 70, 105, 80 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "しぜんかいふく", "しぜんかいふく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ザングース", new uint[] { 73, 115, 60, 60, 60, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "めんえき", "めんえき" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハブネーク", new uint[] { 73, 100, 60, 100, 60, 65 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "だっぴ", "だっぴ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ルナトーン", new uint[] { 70, 55, 65, 95, 85, 70 }, new PokeType[] { PokeType.Rock, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("ソルロック", new uint[] { 70, 95, 85, 55, 65, 70 }, new PokeType[] { PokeType.Rock, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("ドジョッチ", new uint[] { 50, 48, 43, 46, 41, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "どんかん", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ナマズン", new uint[] { 110, 78, 73, 76, 71, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "どんかん", "どんかん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヘイガニ", new uint[] { 43, 80, 65, 50, 35, 35 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("シザリガー", new uint[] { 63, 120, 85, 90, 55, 55 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "かいりきバサミ", "シェルアーマー" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヤジロン", new uint[] { 40, 40, 55, 40, 70, 55 }, new PokeType[] { PokeType.Ground, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("ネンドール", new uint[] { 60, 70, 105, 70, 120, 75 }, new PokeType[] { PokeType.Ground, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("リリーラ", new uint[] { 66, 41, 77, 61, 87, 23 }, new PokeType[] { PokeType.Rock, PokeType.Grass }, new string[] { "きゅうばん", "きゅうばん" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ユレイドル", new uint[] { 86, 81, 97, 81, 107, 43 }, new PokeType[] { PokeType.Rock, PokeType.Grass }, new string[] { "きゅうばん", "きゅうばん" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("アノプス", new uint[] { 45, 95, 50, 40, 50, 75 }, new PokeType[] { PokeType.Rock, PokeType.Bug }, new string[] { "カブトアーマー", "カブトアーマー" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("アーマルド", new uint[] { 75, 125, 100, 70, 80, 45 }, new PokeType[] { PokeType.Rock, PokeType.Bug }, new string[] { "カブトアーマー", "カブトアーマー" }, false, GenderRatio.M7F1));
            DexData.Add(new Species("ヒンバス", new uint[] { 20, 15, 20, 10, 55, 80 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ミロカロス", new uint[] { 95, 60, 79, 100, 125, 81 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ふしぎなうろこ", "ふしぎなうろこ" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ポワルン", new uint[] { 70, 70, 70, 70, 70, 70 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "てんきや", "てんきや" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("カクレオン", new uint[] { 60, 90, 70, 60, 120, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "へんしょく", "へんしょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("カゲボウズ", new uint[] { 44, 75, 35, 63, 33, 45 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふみん", "ふみん" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ジュペッタ", new uint[] { 64, 115, 65, 83, 63, 65 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふみん", "ふみん" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ヨマワル", new uint[] { 20, 40, 90, 30, 90, 25 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("サマヨール", new uint[] { 40, 70, 130, 60, 130, 25 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("トロピウス", new uint[] { 99, 68, 83, 72, 87, 51 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "ようりょくそ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("チリーン", new uint[] { 65, 50, 70, 95, 80, 65 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "ふゆう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("アブソル", new uint[] { 65, 130, 60, 75, 60, 75 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ソーナノ", new uint[] { 95, 23, 48, 23, 48, 23 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "かげふみ", "かげふみ" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ユキワラシ", new uint[] { 50, 50, 50, 50, 50, 50 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "せいしんりょく", "せいしんりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("オニゴーリ", new uint[] { 80, 80, 80, 80, 80, 80 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "せいしんりょく", "せいしんりょく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("タマザラシ", new uint[] { 70, 40, 50, 55, 50, 25 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "あついしぼう" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("トドグラー", new uint[] { 90, 60, 70, 75, 70, 45 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "あついしぼう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("トドゼルガ", new uint[] { 110, 80, 90, 95, 90, 65 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "あついしぼう" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("パールル", new uint[] { 35, 64, 85, 74, 55, 32 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "シェルアーマー", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("ハンテール", new uint[] { 55, 104, 105, 94, 75, 52 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("サクラビス", new uint[] { 55, 84, 105, 114, 75, 52 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ジーランス", new uint[] { 100, 90, 130, 45, 65, 55 }, new PokeType[] { PokeType.Water, PokeType.Rock }, new string[] { "すいすい", "いしあたま" }, true, GenderRatio.M7F1));
            DexData.Add(new Species("ラブカス", new uint[] { 43, 30, 55, 40, 65, 97 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "すいすい" }, true, GenderRatio.M1F3));
            DexData.Add(new Species("タツベイ", new uint[] { 45, 75, 60, 40, 30, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "いしあたま", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Species("コモルー", new uint[] { 65, 95, 100, 60, 50, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "いしあたま", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ボーマンダ", new uint[] { 95, 135, 80, 110, 80, 100 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "いかく", "いかく" }, false, GenderRatio.M1F1));
            DexData.Add(new Species("ダンバル", new uint[] { 40, 55, 80, 35, 60, 30 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "クリアボディ" }, true, GenderRatio.Genderless));
            DexData.Add(new Species("メタング", new uint[] { 60, 75, 100, 55, 80, 50 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "クリアボディ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("メタグロス", new uint[] { 80, 135, 130, 95, 90, 70 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "クリアボディ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("レジロック", new uint[] { 80, 100, 200, 50, 100, 50 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "クリアボディ", "クリアボディ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("レジアイス", new uint[] { 80, 50, 100, 100, 200, 50 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "クリアボディ", "クリアボディ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("レジスチル", new uint[] { 80, 75, 150, 75, 150, 50 }, new PokeType[] { PokeType.Steel, PokeType.Non }, new string[] { "クリアボディ", "クリアボディ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ラティアス", new uint[] { 80, 80, 90, 110, 130, 110 }, new PokeType[] { PokeType.Dragon, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Species("ラティオス", new uint[] { 80, 90, 80, 130, 110, 110 }, new PokeType[] { PokeType.Dragon, PokeType.Psychic }, new string[] { "ふゆう", "ふゆう" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Species("カイオーガ", new uint[] { 100, 100, 90, 150, 140, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あめふらし", "あめふらし" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("グラードン", new uint[] { 100, 150, 140, 100, 90, 90 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "ひでり", "ひでり" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("レックウザ", new uint[] { 105, 150, 90, 150, 90, 95 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "エアロック", "エアロック" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("ジラーチ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "てんのめぐみ", "てんのめぐみ" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("デオキシス", "N", new uint[] { 50, 150, 50, 150, 50, 150 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("デオキシス", "A", new uint[] { 50, 180, 20, 180, 20, 150 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("デオキシス", "D", new uint[] { 50, 70, 160, 70, 160, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));
            DexData.Add(new Species("デオキシス", "S", new uint[] { 50, 95, 90, 95, 90, 180 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "プレッシャー" }, false, GenderRatio.Genderless));

            UniqueData = DexData.Distinct(new SpeciesComparer()).ToList();
            UniqueDex = UniqueData.ToDictionary(_ => _.Name, _ => _);
            DexDictionary = DexData.ToDictionary(_ => _.Name + _.FormName, _ => _);
        }
        class SpeciesComparer : IEqualityComparer<Species>
        {
            public bool Equals(Species x, Species y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(Species s) => s.Name.GetHashCode();
        }
    }

}
