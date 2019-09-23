using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public class Pokemon
    {
        public readonly string Name;
        public readonly uint[] BS;
        public readonly string[] Ability;
        public readonly PokeType[] Type;
        public readonly GenderRatio GenderRatio;
        public readonly bool Hatchable;
        public readonly string FormName;
        public string GetFullName() { return Name + FormName; }

        public Individual GetIndividual(IndivKernel Kernel)
        {
            return new Individual()
            {
                Name = Name,
                Lv = Kernel.Lv,
                PID = Kernel.PID,
                Stats = GetStats(Kernel.IVs, GetNature(Kernel.PID), Kernel.Lv),
                IVs = Kernel.IVs,
                Nature = GetNature(Kernel.PID),
                Ability = GetAbility(Kernel.PID),
                Gender = GetGender(Kernel.PID),
            };
        }

        private uint[] GetStats(uint[] IVs, Nature Nature = Nature.Hardy, uint Lv = 50)
        {
            uint[] stats = new uint[6];
            double[] mag = Nature.ToMagnification();

            stats[0] = (IVs[0] + BS[0] * 2) * Lv / 100 + 10 + Lv;
            for (int i = 1; i < 6; i++)
                stats[i] = (uint)(((IVs[i] + BS[i] * 2) * Lv / 100 + 5) * mag[i]);

            return stats;
        }

        private Gender GetGender(uint PID)
        {
            if (GenderRatio == GenderRatio.Genderless) return Gender.Genderless;
            return (PID & 0xFF) < (uint)GenderRatio ? Gender.Female : Gender.Male;
        }
        private Nature GetNature(uint PID)
        {
            return (Nature)(PID % 25);
        }
        private string GetAbility(uint PID)
        {
            return Ability[PID & 1];
        }

        private Pokemon(GenderRatio ratio)
        {
            Name = "";
            BS = new uint[6] { 100, 100, 100, 100, 100, 100 };
            Ability = new string[2] { "", "" };
            Type = new PokeType[2] { PokeType.Non, PokeType.Non };
            GenderRatio = ratio;
            Hatchable = false;
            FormName = "";
        }
        private Pokemon(string name, uint[] bs, PokeType[] type, string[] ability, bool hatchable, GenderRatio ratio)
        {
            Name = name;
            BS = bs;
            Ability = ability;
            Type = type;
            GenderRatio = ratio;
            Hatchable = hatchable;
            FormName = "";
        }
        private Pokemon(string name, string FormName, uint[] bs, PokeType[] type, string[] ability, bool hatchable, GenderRatio ratio)
        {
            Name = name;
            BS = bs;
            Ability = ability;
            Type = type;
            GenderRatio = ratio;
            Hatchable = hatchable;
            this.FormName = FormName;
        }

        internal static Pokemon GetDommyPokemon(GenderRatio ratio) { return DommyDex[ratio]; }
        public static Pokemon GetPokemon(uint index) { return DexData[(int)(index > 386 ? 0 : index)]; }
        public static Pokemon GetPokemon(uint index, string form)
        {
            if (index == 201) return UnownDex[form];
            if (index == 386) return DeoxysDex[form];
            return DexData[(int)(index > 386 ? 0 : index)];
        }
        public static Pokemon GetPokemon(string Name) { return DexDictionary[Name]; }
        public static Pokemon GetPokemon(string Name, string form)
        {
            if (Name == "アンノーン") return UnownDex[form];
            if (Name == "デオキシス") return DeoxysDex[form];
            return DexDictionary[Name];
        }

        private static readonly List<Pokemon> DexData;
        private static readonly Dictionary<string, Pokemon> DexDictionary;
        private static readonly Dictionary<string, Pokemon> UnownDex;
        private static readonly Dictionary<string, Pokemon> DeoxysDex;

        private static readonly Dictionary<GenderRatio, Pokemon> DommyDex;

        static Pokemon()
        {
            DommyDex = new Dictionary<GenderRatio, Pokemon>()
            {
                { GenderRatio.MaleOnly, new Pokemon(GenderRatio.MaleOnly) },
                { GenderRatio.M7F1, new Pokemon(GenderRatio.M7F1) },
                { GenderRatio.M3F1, new Pokemon(GenderRatio.M3F1) },
                { GenderRatio.M1F1, new Pokemon(GenderRatio.M1F1) },
                { GenderRatio.M1F3, new Pokemon(GenderRatio.M1F3) },
                { GenderRatio.FemaleOnly, new Pokemon(GenderRatio.FemaleOnly) },
                { GenderRatio.Genderless, new Pokemon(GenderRatio.Genderless) }
            };

            DexData = new List<Pokemon>();
            UnownDex = new Dictionary<string, Pokemon>();
            DeoxysDex = new Dictionary<string, Pokemon>();

            DexData.Add(new Pokemon("---", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Non, PokeType.Non }, new string[] { "特性1", "特性2" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("フシギダネ", new uint[] { 45, 49, 49, 65, 65, 45 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("フシギソウ", new uint[] { 60, 62, 63, 80, 80, 60 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("フシギバナ", new uint[] { 80, 82, 83, 100, 100, 80 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ヒトカゲ", new uint[] { 39, 52, 43, 60, 50, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("リザード", new uint[] { 58, 64, 58, 80, 65, 80 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("リザードン", new uint[] { 78, 84, 78, 109, 85, 100 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ゼニガメ", new uint[] { 44, 48, 65, 50, 64, 43 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("カメール", new uint[] { 59, 63, 80, 65, 80, 58 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("カメックス", new uint[] { 79, 83, 100, 85, 105, 78 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("キャタピー", new uint[] { 45, 30, 35, 20, 20, 45 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "りんぷん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("トランセル", new uint[] { 50, 20, 55, 25, 25, 30 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バタフリー", new uint[] { 60, 45, 50, 80, 80, 70 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "ふくがん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ビードル", new uint[] { 40, 35, 30, 20, 20, 50 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コクーン", new uint[] { 45, 25, 50, 25, 25, 35 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("スピアー", new uint[] { 65, 80, 40, 45, 80, 75 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ポッポ", new uint[] { 40, 45, 40, 35, 35, 56 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ピジョン", new uint[] { 63, 60, 55, 50, 50, 71 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ピジョット", new uint[] { 83, 80, 75, 70, 70, 91 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コラッタ", new uint[] { 30, 56, 35, 25, 35, 72 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラッタ", new uint[] { 55, 81, 60, 50, 70, 97 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "こんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オニスズメ", new uint[] { 40, 60, 30, 31, 31, 70 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オニドリル", new uint[] { 65, 90, 65, 61, 61, 100 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アーボ", new uint[] { 35, 60, 44, 40, 54, 55 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "いかく", "だっぴ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アーボック", new uint[] { 60, 85, 69, 65, 79, 80 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "いかく", "だっぴ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ピカチュウ", new uint[] { 35, 55, 30, 50, 40, 90 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ライチュウ", new uint[] { 60, 90, 55, 90, 80, 100 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サンド", new uint[] { 50, 75, 85, 20, 30, 40 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サンドパン", new uint[] { 75, 100, 110, 45, 55, 65 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニドラン♀", new uint[] { 55, 47, 52, 40, 40, 41 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ニドリーナ", new uint[] { 70, 62, 67, 55, 55, 56 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ニドクイン", new uint[] { 90, 82, 87, 75, 85, 76 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ニドラン♂", new uint[] { 46, 57, 40, 40, 40, 50 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("ニドリーノ", new uint[] { 61, 72, 57, 55, 55, 65 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("ニドキング", new uint[] { 81, 92, 77, 85, 75, 85 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "どくのトゲ", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("ピッピ", new uint[] { 70, 45, 48, 60, 65, 35 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ピクシー", new uint[] { 95, 70, 73, 85, 90, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ロコン", new uint[] { 38, 41, 40, 50, 65, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "---" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("キュウコン", new uint[] { 73, 76, 75, 81, 100, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("プリン", new uint[] { 115, 45, 20, 45, 25, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("プクリン", new uint[] { 140, 70, 45, 75, 50, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ズバット", new uint[] { 40, 45, 35, 30, 40, 55 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴルバット", new uint[] { 75, 80, 70, 65, 75, 90 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ナゾノクサ", new uint[] { 45, 50, 55, 75, 65, 30 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("クサイハナ", new uint[] { 60, 65, 70, 85, 75, 40 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラフレシア", new uint[] { 75, 80, 85, 100, 90, 50 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パラス", new uint[] { 35, 70, 55, 45, 55, 25 }, new PokeType[] { PokeType.Bug, PokeType.Grass }, new string[] { "ほうし", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パラセクト", new uint[] { 60, 95, 80, 60, 80, 30 }, new PokeType[] { PokeType.Bug, PokeType.Grass }, new string[] { "ほうし", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コンパン", new uint[] { 60, 55, 50, 40, 55, 45 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "ふくがん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("モルフォン", new uint[] { 70, 65, 60, 90, 75, 90 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ディグダ", new uint[] { 10, 55, 25, 35, 45, 95 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "ありじごく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ダグトリオ", new uint[] { 35, 80, 50, 50, 70, 120 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "すながくれ", "ありじごく" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニャース", new uint[] { 40, 45, 35, 40, 40, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ペルシアン", new uint[] { 65, 70, 60, 65, 65, 115 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "じゅうなん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コダック", new uint[] { 50, 52, 48, 65, 50, 55 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "しめりけ", "ノーてんき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴルダック", new uint[] { 80, 82, 78, 95, 80, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "しめりけ", "ノーてんき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マンキー", new uint[] { 40, 80, 35, 35, 45, 70 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "やるき", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オコリザル", new uint[] { 65, 105, 60, 60, 70, 95 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "やるき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ガーディ", new uint[] { 55, 70, 45, 70, 50, 60 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "いかく", "もらいび" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ウインディ", new uint[] { 90, 110, 80, 100, 80, 95 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "いかく", "もらいび" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ニョロモ", new uint[] { 40, 50, 40, 40, 40, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニョロゾ", new uint[] { 65, 65, 65, 50, 50, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニョロボン", new uint[] { 90, 85, 95, 70, 90, 70 }, new PokeType[] { PokeType.Water, PokeType.Fighting }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ケーシィ", new uint[] { 25, 20, 15, 105, 55, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ユンゲラー", new uint[] { 40, 35, 30, 120, 70, 105 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("フーディン", new uint[] { 55, 50, 45, 135, 85, 120 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "せいしんりょく" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ワンリキー", new uint[] { 70, 80, 50, 35, 35, 35 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "---" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ゴーリキー", new uint[] { 80, 100, 70, 50, 60, 45 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "---" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("カイリキー", new uint[] { 90, 130, 80, 65, 85, 55 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "---" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("マダツボミ", new uint[] { 50, 75, 35, 70, 30, 40 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ウツドン", new uint[] { 65, 90, 50, 85, 45, 55 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ウツボット", new uint[] { 80, 105, 65, 100, 60, 70 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("メノクラゲ", new uint[] { 40, 40, 35, 50, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "クリアボディ", "ヘドロえき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドククラゲ", new uint[] { 80, 70, 65, 80, 120, 100 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "クリアボディ", "ヘドロえき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("イシツブテ", new uint[] { 40, 80, 100, 30, 30, 20 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴローン", new uint[] { 55, 95, 115, 45, 45, 35 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴローニャ", new uint[] { 80, 110, 130, 55, 65, 45 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ポニータ", new uint[] { 50, 85, 55, 65, 65, 90 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "にげあし", "もらいび" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ギャロップ", new uint[] { 65, 100, 70, 80, 80, 105 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "にげあし", "もらいび" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤドン", new uint[] { 90, 65, 65, 40, 40, 15 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤドラン", new uint[] { 95, 75, 110, 100, 80, 30 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コイル", new uint[] { 25, 35, 70, 95, 55, 45 }, new PokeType[] { PokeType.Electric, PokeType.Steel }, new string[] { "じりょく", "がんじょう" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("レアコイル", new uint[] { 50, 60, 95, 120, 70, 70 }, new PokeType[] { PokeType.Electric, PokeType.Steel }, new string[] { "じりょく", "がんじょう" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("カモネギ", new uint[] { 52, 65, 55, 58, 62, 60 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "するどいめ", "せいしんりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドードー", new uint[] { 35, 85, 45, 35, 35, 75 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "にげあし", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドードリオ", new uint[] { 60, 110, 70, 60, 60, 100 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "にげあし", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パウワウ", new uint[] { 65, 45, 55, 45, 70, 45 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ジュゴン", new uint[] { 90, 70, 80, 70, 95, 70 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "あついしぼう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ベトベター", new uint[] { 80, 80, 50, 40, 50, 25 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "あくしゅう", "ねんちゃく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ベトベトン", new uint[] { 105, 105, 75, 65, 100, 50 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "あくしゅう", "ねんちゃく" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("シェルダー", new uint[] { 30, 65, 100, 45, 25, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "シェルアーマー", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パルシェン", new uint[] { 50, 95, 180, 85, 45, 70 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "シェルアーマー", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴース", new uint[] { 30, 35, 30, 100, 35, 80 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴースト", new uint[] { 45, 50, 45, 115, 55, 95 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゲンガー", new uint[] { 60, 65, 60, 130, 75, 110 }, new PokeType[] { PokeType.Ghost, PokeType.Poison }, new string[] { "ふゆう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("イワーク", new uint[] { 35, 45, 160, 30, 45, 70 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("スリープ", new uint[] { 60, 48, 45, 43, 90, 42 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふみん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("スリーパー", new uint[] { 85, 73, 70, 73, 115, 67 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふみん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("クラブ", new uint[] { 30, 105, 90, 25, 25, 50 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キングラー", new uint[] { 55, 130, 115, 50, 50, 75 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ビリリダマ", new uint[] { 40, 30, 50, 55, 55, 100 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ぼうおん", "せいでんき" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("マルマイン", new uint[] { 60, 50, 70, 80, 80, 140 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ぼうおん", "せいでんき" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("タマタマ", new uint[] { 60, 40, 80, 60, 45, 40 }, new PokeType[] { PokeType.Grass, PokeType.Psychic }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ナッシー", new uint[] { 95, 95, 85, 125, 65, 55 }, new PokeType[] { PokeType.Grass, PokeType.Psychic }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("カラカラ", new uint[] { 50, 50, 95, 40, 50, 35 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "いしあたま", "ひらいしん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ガラガラ", new uint[] { 60, 80, 110, 50, 80, 45 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "いしあたま", "ひらいしん" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サワムラー", new uint[] { 50, 120, 53, 35, 110, 87 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "じゅうなん", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("エビワラー", new uint[] { 50, 105, 79, 35, 110, 76 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "するどいめ", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("ベロリンガ", new uint[] { 90, 55, 75, 60, 75, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドガース", new uint[] { 40, 65, 95, 60, 45, 35 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ふゆう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マタドガス", new uint[] { 65, 90, 120, 85, 70, 60 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サイホーン", new uint[] { 80, 85, 95, 30, 30, 25 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "ひらいしん", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サイドン", new uint[] { 105, 130, 120, 45, 45, 40 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "ひらいしん", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラッキー", new uint[] { 250, 5, 5, 35, 105, 50 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "しぜんかいふく", "てんのめぐみ" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("モンジャラ", new uint[] { 65, 55, 115, 100, 40, 60 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ガルーラ", new uint[] { 105, 95, 80, 40, 80, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "はやおき", "---" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("タッツー", new uint[] { 30, 40, 70, 70, 25, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("シードラ", new uint[] { 55, 65, 95, 95, 45, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "どくのトゲ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("トサキント", new uint[] { 45, 67, 60, 35, 50, 63 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "みずのベール" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アズマオウ", new uint[] { 80, 92, 65, 65, 80, 68 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "みずのベール" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヒトデマン", new uint[] { 30, 45, 55, 70, 55, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "はっこう", "しぜんかいふく" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("スターミー", new uint[] { 60, 75, 85, 100, 85, 115 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "はっこう", "しぜんかいふく" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("バリヤード", new uint[] { 40, 45, 65, 100, 120, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ぼうおん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ストライク", new uint[] { 70, 110, 80, 55, 80, 105 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ルージュラ", new uint[] { 65, 50, 35, 115, 95, 95 }, new PokeType[] { PokeType.Ice, PokeType.Psychic }, new string[] { "どんかん", "---" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("エレブー", new uint[] { 65, 83, 57, 95, 85, 105 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ブーバー", new uint[] { 65, 95, 57, 100, 85, 93 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "ほのおのからだ", "---" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("カイロス", new uint[] { 65, 125, 100, 55, 70, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "かいりきバサミ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ケンタロス", new uint[] { 75, 100, 95, 40, 70, 110 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "---" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("コイキング", new uint[] { 20, 10, 55, 15, 20, 80 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ギャラドス", new uint[] { 95, 125, 79, 60, 100, 81 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "いかく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラプラス", new uint[] { 130, 85, 80, 85, 95, 60 }, new PokeType[] { PokeType.Water, PokeType.Ice }, new string[] { "ちょすい", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("メタモン", new uint[] { 48, 48, 48, 48, 48, 48 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "じゅうなん", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("イーブイ", new uint[] { 55, 55, 50, 45, 65, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("シャワーズ", new uint[] { 130, 65, 60, 110, 95, 65 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("サンダース", new uint[] { 65, 65, 60, 110, 95, 130 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "ちくでん", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ブースター", new uint[] { 65, 130, 60, 95, 110, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もらいび", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ポリゴン", new uint[] { 65, 60, 70, 85, 75, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "トレース", "---" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("オムナイト", new uint[] { 35, 40, 100, 90, 55, 35 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "シェルアーマー" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("オムスター", new uint[] { 70, 60, 125, 115, 70, 55 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "シェルアーマー" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("カブト", new uint[] { 30, 80, 90, 55, 45, 55 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "カブトアーマー" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("カブトプス", new uint[] { 60, 115, 105, 65, 70, 80 }, new PokeType[] { PokeType.Rock, PokeType.Water }, new string[] { "すいすい", "カブトアーマー" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("プテラ", new uint[] { 80, 105, 65, 60, 75, 130 }, new PokeType[] { PokeType.Rock, PokeType.Flying }, new string[] { "いしあたま", "プレッシャー" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("カビゴン", new uint[] { 160, 110, 65, 65, 110, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "めんえき", "あついしぼう" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("フリーザー", new uint[] { 90, 85, 100, 95, 125, 85 }, new PokeType[] { PokeType.Ice, PokeType.Flying }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("サンダー", new uint[] { 90, 90, 85, 125, 90, 100 }, new PokeType[] { PokeType.Electric, PokeType.Flying }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ファイヤー", new uint[] { 90, 100, 90, 125, 85, 90 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ミニリュウ", new uint[] { 41, 64, 45, 50, 50, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "だっぴ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハクリュー", new uint[] { 61, 84, 65, 70, 70, 70 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("カイリュー", new uint[] { 91, 134, 95, 100, 100, 80 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "せいしんりょく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ミュウツー", new uint[] { 106, 110, 90, 154, 90, 130 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ミュウ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("チコリータ", new uint[] { 45, 49, 65, 49, 65, 45 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ベイリーフ", new uint[] { 60, 62, 80, 63, 80, 60 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("メガニウム", new uint[] { 80, 82, 100, 83, 100, 80 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ヒノアラシ", new uint[] { 39, 52, 43, 60, 50, 65 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("マグマラシ", new uint[] { 58, 64, 58, 80, 65, 80 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("バクフーン", new uint[] { 78, 84, 78, 109, 85, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ワニノコ", new uint[] { 50, 65, 64, 44, 48, 43 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("アリゲイツ", new uint[] { 65, 80, 80, 59, 63, 58 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("オーダイル", new uint[] { 85, 105, 100, 79, 83, 78 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("オタチ", new uint[] { 35, 46, 34, 35, 45, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オオタチ", new uint[] { 85, 76, 64, 45, 55, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ホーホー", new uint[] { 60, 30, 30, 36, 56, 50 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "ふみん", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヨルノズク", new uint[] { 100, 50, 50, 76, 96, 70 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "ふみん", "するどいめ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("レディバ", new uint[] { 40, 20, 30, 40, 80, 55 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("レディアン", new uint[] { 55, 35, 50, 55, 110, 85 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("イトマル", new uint[] { 40, 60, 40, 40, 40, 30 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "ふみん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アリアドス", new uint[] { 70, 90, 70, 60, 60, 40 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "むしのしらせ", "ふみん" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("クロバット", new uint[] { 85, 90, 80, 70, 80, 130 }, new PokeType[] { PokeType.Poison, PokeType.Flying }, new string[] { "せいしんりょく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("チョンチー", new uint[] { 75, 38, 38, 56, 56, 67 }, new PokeType[] { PokeType.Water, PokeType.Electric }, new string[] { "ちくでん", "はっこう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ランターン", new uint[] { 125, 58, 58, 76, 76, 67 }, new PokeType[] { PokeType.Water, PokeType.Electric }, new string[] { "ちくでん", "はっこう" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ピチュー", new uint[] { 20, 40, 15, 35, 35, 60 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ピィ", new uint[] { 50, 25, 28, 45, 55, 15 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ププリン", new uint[] { 90, 30, 15, 40, 20, 15 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("トゲピー", new uint[] { 35, 20, 65, 40, 65, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "はりきり", "てんのめぐみ" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("トゲチック", new uint[] { 55, 40, 85, 80, 105, 40 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "はりきり", "てんのめぐみ" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ネイティ", new uint[] { 40, 50, 45, 70, 45, 70 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "シンクロ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ネイティオ", new uint[] { 65, 75, 70, 95, 70, 95 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "シンクロ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("メリープ", new uint[] { 55, 40, 40, 65, 45, 35 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("モココ", new uint[] { 70, 55, 55, 80, 60, 45 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("デンリュウ", new uint[] { 90, 75, 75, 115, 90, 55 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キレイハナ", new uint[] { 75, 80, 85, 90, 100, 50 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マリル", new uint[] { 70, 20, 50, 20, 50, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マリルリ", new uint[] { 100, 50, 80, 50, 80, 50 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ウソッキー", new uint[] { 70, 100, 115, 30, 65, 30 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "がんじょう", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニョロトノ", new uint[] { 90, 75, 75, 90, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ちょすい", "しめりけ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハネッコ", new uint[] { 35, 35, 40, 35, 55, 50 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ポポッコ", new uint[] { 55, 45, 50, 45, 65, 80 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ワタッコ", new uint[] { 75, 55, 70, 55, 85, 110 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("エイパム", new uint[] { 55, 70, 55, 40, 55, 85 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "にげあし", "ものひろい" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヒマナッツ", new uint[] { 30, 30, 30, 30, 30, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キマワリ", new uint[] { 75, 75, 55, 105, 85, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤンヤンマ", new uint[] { 65, 65, 45, 75, 45, 95 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "かそく", "ふくがん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ウパー", new uint[] { 55, 45, 45, 25, 25, 15 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "しめりけ", "ちょすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヌオー", new uint[] { 95, 85, 85, 65, 65, 35 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "しめりけ", "ちょすい" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("エーフィ", new uint[] { 65, 65, 60, 130, 95, 110 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ブラッキー", new uint[] { 95, 65, 110, 60, 130, 65 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "シンクロ", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ヤミカラス", new uint[] { 60, 85, 42, 85, 42, 91 }, new PokeType[] { PokeType.Dark, PokeType.Flying }, new string[] { "ふみん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤドキング", new uint[] { 95, 75, 80, 100, 110, 30 }, new PokeType[] { PokeType.Water, PokeType.Psychic }, new string[] { "どんかん", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ムウマ", new uint[] { 60, 60, 60, 85, 85, 85 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふゆう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アンノーン", "A", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ソーナンス", new uint[] { 190, 33, 58, 33, 58, 33 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "かげふみ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キリンリキ", new uint[] { 70, 80, 65, 90, 65, 85 }, new PokeType[] { PokeType.Normal, PokeType.Psychic }, new string[] { "せいしんりょく", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("クヌギダマ", new uint[] { 50, 65, 90, 35, 35, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "がんじょう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("フォレトス", new uint[] { 75, 90, 140, 60, 60, 40 }, new PokeType[] { PokeType.Bug, PokeType.Steel }, new string[] { "がんじょう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ノコッチ", new uint[] { 100, 70, 70, 65, 65, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "てんのめぐみ", "にげあし" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("グライガー", new uint[] { 65, 75, 105, 35, 65, 85 }, new PokeType[] { PokeType.Ground, PokeType.Flying }, new string[] { "かいりきバサミ", "すながくれ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハガネール", new uint[] { 75, 85, 200, 55, 65, 30 }, new PokeType[] { PokeType.Steel, PokeType.Ground }, new string[] { "いしあたま", "がんじょう" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ブルー", new uint[] { 60, 80, 50, 40, 40, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "にげあし" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("グランブル", new uint[] { 90, 120, 75, 60, 60, 45 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ハリーセン", new uint[] { 65, 95, 75, 55, 55, 85 }, new PokeType[] { PokeType.Water, PokeType.Poison }, new string[] { "どくのトゲ", "すいすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハッサム", new uint[] { 70, 130, 100, 55, 80, 65 }, new PokeType[] { PokeType.Bug, PokeType.Steel }, new string[] { "むしのしらせ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ツボツボ", new uint[] { 20, 10, 230, 10, 230, 5 }, new PokeType[] { PokeType.Bug, PokeType.Rock }, new string[] { "がんじょう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヘラクロス", new uint[] { 80, 125, 75, 40, 95, 85 }, new PokeType[] { PokeType.Bug, PokeType.Fighting }, new string[] { "むしのしらせ", "こんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ニューラ", new uint[] { 55, 95, 55, 35, 75, 115 }, new PokeType[] { PokeType.Dark, PokeType.Ice }, new string[] { "せいしんりょく", "するどいめ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヒメグマ", new uint[] { 60, 80, 50, 50, 50, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("リングマ", new uint[] { 90, 130, 75, 75, 75, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "こんじょう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マグマッグ", new uint[] { 40, 40, 40, 70, 40, 20 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "マグマのよろい", "ほのおのからだ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マグカルゴ", new uint[] { 50, 50, 120, 80, 80, 30 }, new PokeType[] { PokeType.Fire, PokeType.Rock }, new string[] { "マグマのよろい", "ほのおのからだ" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ウリムー", new uint[] { 50, 50, 40, 30, 30, 50 }, new PokeType[] { PokeType.Ice, PokeType.Ground }, new string[] { "どんかん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("イノムー", new uint[] { 100, 100, 80, 60, 60, 50 }, new PokeType[] { PokeType.Ice, PokeType.Ground }, new string[] { "どんかん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サニーゴ", new uint[] { 55, 55, 85, 65, 85, 35 }, new PokeType[] { PokeType.Water, PokeType.Rock }, new string[] { "はりきり", "しぜんかいふく" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("テッポウオ", new uint[] { 35, 65, 35, 65, 35, 65 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "はりきり", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オクタン", new uint[] { 75, 105, 75, 105, 75, 45 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "きゅうばん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("デリバード", new uint[] { 45, 55, 45, 65, 45, 75 }, new PokeType[] { PokeType.Ice, PokeType.Flying }, new string[] { "やるき", "はりきり" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マンタイン", new uint[] { 65, 40, 70, 80, 140, 70 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "すいすい", "ちょすい" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("エアームド", new uint[] { 65, 80, 140, 40, 70, 70 }, new PokeType[] { PokeType.Steel, PokeType.Flying }, new string[] { "するどいめ", "がんじょう" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("デルビル", new uint[] { 45, 60, 30, 80, 50, 65 }, new PokeType[] { PokeType.Dark, PokeType.Fire }, new string[] { "はやおき", "もらいび" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヘルガー", new uint[] { 75, 90, 50, 110, 80, 95 }, new PokeType[] { PokeType.Dark, PokeType.Fire }, new string[] { "はやおき", "もらいび" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キングドラ", new uint[] { 75, 95, 95, 95, 95, 85 }, new PokeType[] { PokeType.Water, PokeType.Dragon }, new string[] { "すいすい", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴマゾウ", new uint[] { 90, 60, 60, 40, 40, 40 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "ものひろい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドンファン", new uint[] { 90, 120, 120, 60, 60, 50 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "がんじょう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ポリゴン2", new uint[] { 85, 80, 90, 105, 95, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "トレース", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("オドシシ", new uint[] { 73, 95, 62, 85, 65, 85 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "いかく", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドーブル", new uint[] { 55, 20, 35, 20, 45, 75 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バルキー", new uint[] { 35, 35, 35, 35, 35, 35 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "こんじょう", "---" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("カポエラー", new uint[] { 50, 95, 95, 35, 110, 70 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "いかく", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("ムチュール", new uint[] { 45, 30, 15, 85, 65, 65 }, new PokeType[] { PokeType.Ice, PokeType.Psychic }, new string[] { "どんかん", "---" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("エレキッド", new uint[] { 45, 63, 37, 65, 55, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "---" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ブビィ", new uint[] { 45, 75, 37, 70, 55, 83 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "ほのおのからだ", "---" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ミルタンク", new uint[] { 95, 80, 105, 40, 70, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "あついしぼう", "---" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ハピナス", new uint[] { 255, 10, 10, 75, 135, 55 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "しぜんかいふく", "てんのめぐみ" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ライコウ", new uint[] { 90, 85, 75, 115, 100, 115 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("エンテイ", new uint[] { 115, 115, 85, 90, 75, 100 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("スイクン", new uint[] { 100, 75, 115, 90, 115, 85 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ヨーギラス", new uint[] { 50, 64, 50, 45, 50, 41 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "こんじょう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サナギラス", new uint[] { 70, 84, 70, 65, 70, 51 }, new PokeType[] { PokeType.Rock, PokeType.Ground }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バンギラス", new uint[] { 100, 134, 110, 95, 100, 61 }, new PokeType[] { PokeType.Rock, PokeType.Dark }, new string[] { "すなおこし", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ルギア", new uint[] { 106, 90, 130, 90, 154, 110 }, new PokeType[] { PokeType.Psychic, PokeType.Flying }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ホウオウ", new uint[] { 106, 130, 90, 110, 154, 90 }, new PokeType[] { PokeType.Fire, PokeType.Flying }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("セレビィ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Psychic, PokeType.Grass }, new string[] { "しぜんかいふく", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("キモリ", new uint[] { 40, 45, 35, 65, 55, 70 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ジュプトル", new uint[] { 50, 65, 45, 85, 65, 95 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ジュカイン", new uint[] { 70, 85, 65, 105, 85, 120 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "しんりょく", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("アチャモ", new uint[] { 45, 60, 40, 70, 50, 45 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "もうか", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ワカシャモ", new uint[] { 60, 85, 60, 85, 60, 55 }, new PokeType[] { PokeType.Fire, PokeType.Fighting }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("バシャーモ", new uint[] { 80, 120, 70, 110, 70, 80 }, new PokeType[] { PokeType.Fire, PokeType.Fighting }, new string[] { "もうか", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ミズゴロウ", new uint[] { 50, 70, 50, 50, 50, 40 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "げきりゅう", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ヌマクロー", new uint[] { 70, 85, 70, 60, 70, 50 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ラグラージ", new uint[] { 100, 110, 90, 85, 90, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "げきりゅう", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ポチエナ", new uint[] { 35, 55, 35, 30, 30, 35 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "にげあし", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("グラエナ", new uint[] { 70, 90, 70, 60, 60, 70 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "いかく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ジグザグマ", new uint[] { 38, 30, 41, 30, 41, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マッスグマ", new uint[] { 78, 70, 61, 50, 61, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ものひろい", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ケムッソ", new uint[] { 45, 45, 35, 20, 30, 20 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "りんぷん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("カラサリス", new uint[] { 50, 35, 55, 25, 25, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アゲハント", new uint[] { 60, 70, 50, 90, 50, 65 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "むしのしらせ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マユルド", new uint[] { 50, 35, 55, 25, 25, 15 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "だっぴ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドクケイル", new uint[] { 60, 50, 70, 50, 90, 65 }, new PokeType[] { PokeType.Bug, PokeType.Poison }, new string[] { "りんぷん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハスボー", new uint[] { 40, 30, 30, 40, 50, 30 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハスブレロ", new uint[] { 60, 50, 50, 60, 70, 50 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ルンパッパ", new uint[] { 80, 70, 70, 90, 100, 70 }, new PokeType[] { PokeType.Water, PokeType.Grass }, new string[] { "すいすい", "あめうけざら" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("タネボー", new uint[] { 40, 40, 50, 30, 30, 30 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ようりょくそ", "はやおき" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コノハナ", new uint[] { 70, 70, 40, 60, 40, 60 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "ようりょくそ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ダーテング", new uint[] { 90, 100, 60, 90, 60, 80 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "ようりょくそ", "はやおき" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("スバメ", new uint[] { 40, 55, 30, 30, 30, 85 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "こんじょう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オオスバメ", new uint[] { 60, 85, 60, 50, 50, 125 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "こんじょう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キャモメ", new uint[] { 40, 30, 30, 55, 30, 85 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "するどいめ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ペリッパー", new uint[] { 60, 50, 100, 85, 70, 65 }, new PokeType[] { PokeType.Water, PokeType.Flying }, new string[] { "するどいめ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラルトス", new uint[] { 28, 25, 25, 45, 35, 40 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キルリア", new uint[] { 38, 35, 35, 65, 55, 50 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サーナイト", new uint[] { 68, 65, 65, 125, 115, 80 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "シンクロ", "トレース" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アメタマ", new uint[] { 40, 30, 32, 50, 52, 65 }, new PokeType[] { PokeType.Bug, PokeType.Water }, new string[] { "すいすい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アメモース", new uint[] { 70, 60, 62, 80, 82, 60 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "いかく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キノココ", new uint[] { 60, 40, 60, 40, 60, 35 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "ほうし", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キノガッサ", new uint[] { 60, 130, 80, 60, 60, 70 }, new PokeType[] { PokeType.Grass, PokeType.Fighting }, new string[] { "ほうし", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ナマケロ", new uint[] { 60, 60, 60, 35, 35, 30 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "なまけ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤルキモノ", new uint[] { 80, 80, 80, 55, 55, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "やるき", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ケッキング", new uint[] { 150, 160, 100, 95, 65, 100 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "なまけ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ツチニン", new uint[] { 31, 45, 90, 30, 30, 40 }, new PokeType[] { PokeType.Bug, PokeType.Ground }, new string[] { "ふくがん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("テッカニン", new uint[] { 61, 90, 45, 50, 50, 160 }, new PokeType[] { PokeType.Bug, PokeType.Flying }, new string[] { "かそく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヌケニン", new uint[] { 1, 90, 45, 30, 30, 40 }, new PokeType[] { PokeType.Bug, PokeType.Ghost }, new string[] { "ふしぎなまもり", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ゴニョニョ", new uint[] { 64, 51, 23, 51, 23, 28 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドゴーム", new uint[] { 84, 71, 43, 71, 43, 48 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バクオング", new uint[] { 104, 91, 63, 91, 63, 68 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "ぼうおん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マクノシタ", new uint[] { 72, 60, 30, 20, 30, 25 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "あついしぼう", "こんじょう" }, true, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ハリテヤマ", new uint[] { 144, 120, 60, 40, 60, 50 }, new PokeType[] { PokeType.Fighting, PokeType.Non }, new string[] { "あついしぼう", "こんじょう" }, false, GenderRatio.M3F1));
            DexData.Add(new Pokemon("ルリリ", new uint[] { 50, 20, 40, 20, 40, 20 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "あついしぼう", "ちからもち" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ノズパス", new uint[] { 30, 45, 135, 45, 90, 30 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "がんじょう", "じりょく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("エネコ", new uint[] { 50, 45, 45, 35, 35, 50 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("エネコロロ", new uint[] { 70, 65, 65, 55, 55, 70 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "メロメロボディ", "---" }, false, GenderRatio.M1F3));
            DexData.Add(new Pokemon("ヤミラミ", new uint[] { 50, 75, 75, 65, 65, 50 }, new PokeType[] { PokeType.Dark, PokeType.Ghost }, new string[] { "するどいめ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("クチート", new uint[] { 50, 85, 85, 55, 55, 50 }, new PokeType[] { PokeType.Steel, PokeType.Non }, new string[] { "かいりきバサミ", "いかく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ココドラ", new uint[] { 50, 70, 100, 40, 40, 30 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コドラ", new uint[] { 60, 90, 140, 50, 50, 40 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ボスゴドラ", new uint[] { 70, 110, 180, 60, 60, 50 }, new PokeType[] { PokeType.Steel, PokeType.Rock }, new string[] { "がんじょう", "いしあたま" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アサナン", new uint[] { 30, 40, 55, 40, 55, 60 }, new PokeType[] { PokeType.Fighting, PokeType.Psychic }, new string[] { "ヨガパワー", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("チャーレム", new uint[] { 60, 60, 75, 60, 75, 80 }, new PokeType[] { PokeType.Fighting, PokeType.Psychic }, new string[] { "ヨガパワー", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ラクライ", new uint[] { 40, 45, 40, 65, 40, 65 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "ひらいしん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ライボルト", new uint[] { 70, 75, 60, 105, 60, 105 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "せいでんき", "ひらいしん" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("プラスル", new uint[] { 60, 50, 40, 85, 75, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "プラス", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マイナン", new uint[] { 60, 40, 50, 75, 85, 95 }, new PokeType[] { PokeType.Electric, PokeType.Non }, new string[] { "マイナス", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バルビート", new uint[] { 65, 73, 55, 47, 75, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "はっこう", "むしのしらせ" }, true, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("イルミーゼ", new uint[] { 65, 47, 55, 73, 75, 85 }, new PokeType[] { PokeType.Bug, PokeType.Non }, new string[] { "どんかん", "---" }, true, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ロゼリア", new uint[] { 50, 60, 45, 100, 80, 65 }, new PokeType[] { PokeType.Grass, PokeType.Poison }, new string[] { "しぜんかいふく", "どくのトゲ" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ゴクリン", new uint[] { 70, 43, 53, 43, 53, 40 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ヘドロえき", "ねんちゃく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("マルノーム", new uint[] { 100, 73, 83, 73, 83, 55 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "ヘドロえき", "ねんちゃく" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("キバニア", new uint[] { 45, 90, 20, 65, 20, 65 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "さめはだ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サメハダー", new uint[] { 70, 120, 40, 95, 40, 95 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "さめはだ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ホエルコ", new uint[] { 130, 70, 35, 70, 35, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "みずのベール", "どんかん" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ホエルオー", new uint[] { 170, 90, 45, 90, 45, 60 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "みずのベール", "どんかん" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ドンメル", new uint[] { 60, 60, 40, 65, 45, 35 }, new PokeType[] { PokeType.Fire, PokeType.Ground }, new string[] { "どんかん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バクーダ", new uint[] { 70, 100, 70, 105, 75, 40 }, new PokeType[] { PokeType.Fire, PokeType.Ground }, new string[] { "マグマのよろい", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コータス", new uint[] { 70, 85, 140, 85, 70, 20 }, new PokeType[] { PokeType.Fire, PokeType.Non }, new string[] { "しろいけむり", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("バネブー", new uint[] { 60, 25, 35, 70, 80, 60 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "あついしぼう", "マイペース" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ブーピッグ", new uint[] { 80, 45, 65, 90, 110, 80 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "あついしぼう", "マイペース" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パッチール", new uint[] { 60, 60, 60, 60, 60, 60 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "マイペース", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ナックラー", new uint[] { 45, 100, 45, 45, 45, 10 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "かいりきバサミ", "ありじごく" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ビブラーバ", new uint[] { 50, 70, 50, 50, 50, 70 }, new PokeType[] { PokeType.Ground, PokeType.Dragon }, new string[] { "ふゆう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("フライゴン", new uint[] { 80, 100, 80, 80, 80, 100 }, new PokeType[] { PokeType.Ground, PokeType.Dragon }, new string[] { "ふゆう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サボネア", new uint[] { 50, 85, 40, 85, 40, 35 }, new PokeType[] { PokeType.Grass, PokeType.Non }, new string[] { "すながくれ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ノクタス", new uint[] { 70, 115, 60, 115, 60, 55 }, new PokeType[] { PokeType.Grass, PokeType.Dark }, new string[] { "すながくれ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("チルット", new uint[] { 45, 40, 60, 40, 75, 50 }, new PokeType[] { PokeType.Normal, PokeType.Flying }, new string[] { "しぜんかいふく", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("チルタリス", new uint[] { 75, 70, 90, 70, 105, 80 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "しぜんかいふく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ザングース", new uint[] { 73, 115, 60, 60, 60, 90 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "めんえき", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハブネーク", new uint[] { 73, 100, 60, 100, 60, 65 }, new PokeType[] { PokeType.Poison, PokeType.Non }, new string[] { "だっぴ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ルナトーン", new uint[] { 70, 55, 65, 95, 85, 70 }, new PokeType[] { PokeType.Rock, PokeType.Psychic }, new string[] { "ふゆう", "---" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ソルロック", new uint[] { 70, 95, 85, 55, 65, 70 }, new PokeType[] { PokeType.Rock, PokeType.Psychic }, new string[] { "ふゆう", "---" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ドジョッチ", new uint[] { 50, 48, 43, 46, 41, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "どんかん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ナマズン", new uint[] { 110, 78, 73, 76, 71, 60 }, new PokeType[] { PokeType.Water, PokeType.Ground }, new string[] { "どんかん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヘイガニ", new uint[] { 43, 80, 65, 50, 35, 35 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "かいりきバサミ", "シェルアーマー" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("シザリガー", new uint[] { 63, 120, 85, 90, 55, 55 }, new PokeType[] { PokeType.Water, PokeType.Dark }, new string[] { "かいりきバサミ", "シェルアーマー" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヤジロン", new uint[] { 40, 40, 55, 40, 70, 55 }, new PokeType[] { PokeType.Ground, PokeType.Psychic }, new string[] { "ふゆう", "---" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ネンドール", new uint[] { 60, 70, 105, 70, 120, 75 }, new PokeType[] { PokeType.Ground, PokeType.Psychic }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("リリーラ", new uint[] { 66, 41, 77, 61, 87, 23 }, new PokeType[] { PokeType.Rock, PokeType.Grass }, new string[] { "きゅうばん", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ユレイドル", new uint[] { 86, 81, 97, 81, 107, 43 }, new PokeType[] { PokeType.Rock, PokeType.Grass }, new string[] { "きゅうばん", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("アノプス", new uint[] { 45, 95, 50, 40, 50, 75 }, new PokeType[] { PokeType.Rock, PokeType.Bug }, new string[] { "カブトアーマー", "---" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("アーマルド", new uint[] { 75, 125, 100, 70, 80, 45 }, new PokeType[] { PokeType.Rock, PokeType.Bug }, new string[] { "カブトアーマー", "---" }, false, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ヒンバス", new uint[] { 20, 15, 20, 10, 55, 80 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ミロカロス", new uint[] { 95, 60, 79, 100, 125, 81 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "ふしぎなうろこ", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ポワルン", new uint[] { 70, 70, 70, 70, 70, 70 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "てんきや", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("カクレオン", new uint[] { 60, 90, 70, 60, 120, 40 }, new PokeType[] { PokeType.Normal, PokeType.Non }, new string[] { "へんしょく", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("カゲボウズ", new uint[] { 44, 75, 35, 63, 33, 45 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふみん", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ジュペッタ", new uint[] { 64, 115, 65, 83, 63, 65 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふみん", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ヨマワル", new uint[] { 20, 40, 90, 30, 90, 25 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "ふゆう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サマヨール", new uint[] { 40, 70, 130, 60, 130, 25 }, new PokeType[] { PokeType.Ghost, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("トロピウス", new uint[] { 99, 68, 83, 72, 87, 51 }, new PokeType[] { PokeType.Grass, PokeType.Flying }, new string[] { "ようりょくそ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("チリーン", new uint[] { 65, 50, 70, 95, 80, 65 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("アブソル", new uint[] { 65, 130, 60, 75, 60, 75 }, new PokeType[] { PokeType.Dark, PokeType.Non }, new string[] { "プレッシャー", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ソーナノ", new uint[] { 95, 23, 48, 23, 48, 23 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "かげふみ", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ユキワラシ", new uint[] { 50, 50, 50, 50, 50, 50 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "せいしんりょく", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("オニゴーリ", new uint[] { 80, 80, 80, 80, 80, 80 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "せいしんりょく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("タマザラシ", new uint[] { 70, 40, 50, 55, 50, 25 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("トドグラー", new uint[] { 90, 60, 70, 75, 70, 45 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("トドゼルガ", new uint[] { 110, 80, 90, 95, 90, 65 }, new PokeType[] { PokeType.Ice, PokeType.Water }, new string[] { "あついしぼう", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("パールル", new uint[] { 35, 64, 85, 74, 55, 32 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "シェルアーマー", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ハンテール", new uint[] { 55, 104, 105, 94, 75, 52 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("サクラビス", new uint[] { 55, 84, 105, 114, 75, 52 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ジーランス", new uint[] { 100, 90, 130, 45, 65, 55 }, new PokeType[] { PokeType.Water, PokeType.Rock }, new string[] { "すいすい", "いしあたま" }, true, GenderRatio.M7F1));
            DexData.Add(new Pokemon("ラブカス", new uint[] { 43, 30, 55, 40, 65, 97 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "すいすい", "---" }, true, GenderRatio.M1F3));
            DexData.Add(new Pokemon("タツベイ", new uint[] { 45, 75, 60, 40, 30, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "いしあたま", "---" }, true, GenderRatio.M1F1));
            DexData.Add(new Pokemon("コモルー", new uint[] { 65, 95, 100, 60, 50, 50 }, new PokeType[] { PokeType.Dragon, PokeType.Non }, new string[] { "いしあたま", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ボーマンダ", new uint[] { 95, 135, 80, 110, 80, 100 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "いかく", "---" }, false, GenderRatio.M1F1));
            DexData.Add(new Pokemon("ダンバル", new uint[] { 40, 55, 80, 35, 60, 30 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "---" }, true, GenderRatio.Genderless));
            DexData.Add(new Pokemon("メタング", new uint[] { 60, 75, 100, 55, 80, 50 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("メタグロス", new uint[] { 80, 135, 130, 95, 90, 70 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "クリアボディ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("レジロック", new uint[] { 80, 100, 200, 50, 100, 50 }, new PokeType[] { PokeType.Rock, PokeType.Non }, new string[] { "クリアボディ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("レジアイス", new uint[] { 80, 50, 100, 100, 200, 50 }, new PokeType[] { PokeType.Ice, PokeType.Non }, new string[] { "クリアボディ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("レジスチル", new uint[] { 80, 75, 150, 75, 150, 50 }, new PokeType[] { PokeType.Steel, PokeType.Non }, new string[] { "クリアボディ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ラティアス", new uint[] { 80, 80, 90, 110, 130, 110 }, new PokeType[] { PokeType.Dragon, PokeType.Psychic }, new string[] { "ふゆう", "---" }, false, GenderRatio.FemaleOnly));
            DexData.Add(new Pokemon("ラティオス", new uint[] { 80, 90, 80, 130, 110, 110 }, new PokeType[] { PokeType.Dragon, PokeType.Psychic }, new string[] { "ふゆう", "---" }, false, GenderRatio.MaleOnly));
            DexData.Add(new Pokemon("カイオーガ", new uint[] { 100, 100, 90, 150, 140, 90 }, new PokeType[] { PokeType.Water, PokeType.Non }, new string[] { "あめふらし", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("グラードン", new uint[] { 100, 150, 140, 100, 90, 90 }, new PokeType[] { PokeType.Ground, PokeType.Non }, new string[] { "ひでり", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("レックウザ", new uint[] { 105, 150, 90, 150, 90, 95 }, new PokeType[] { PokeType.Dragon, PokeType.Flying }, new string[] { "エアロック", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("ジラーチ", new uint[] { 100, 100, 100, 100, 100, 100 }, new PokeType[] { PokeType.Steel, PokeType.Psychic }, new string[] { "てんのめぐみ", "---" }, false, GenderRatio.Genderless));
            DexData.Add(new Pokemon("デオキシス", "N", new uint[] { 50, 150, 50, 150, 50, 150 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));

            UnownDex.Add("A", new Pokemon("アンノーン", "A", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("B", new Pokemon("アンノーン", "B", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("C", new Pokemon("アンノーン", "C", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("D", new Pokemon("アンノーン", "D", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("E", new Pokemon("アンノーン", "E", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("F", new Pokemon("アンノーン", "F", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("G", new Pokemon("アンノーン", "G", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("H", new Pokemon("アンノーン", "H", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("I", new Pokemon("アンノーン", "I", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("J", new Pokemon("アンノーン", "J", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("K", new Pokemon("アンノーン", "K", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("L", new Pokemon("アンノーン", "L", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("M", new Pokemon("アンノーン", "M", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("N", new Pokemon("アンノーン", "N", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("O", new Pokemon("アンノーン", "O", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("P", new Pokemon("アンノーン", "P", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("Q", new Pokemon("アンノーン", "Q", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("R", new Pokemon("アンノーン", "R", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("S", new Pokemon("アンノーン", "S", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("T", new Pokemon("アンノーン", "T", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("U", new Pokemon("アンノーン", "U", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("V", new Pokemon("アンノーン", "V", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("W", new Pokemon("アンノーン", "W", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("X", new Pokemon("アンノーン", "X", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("Y", new Pokemon("アンノーン", "Y", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("Z", new Pokemon("アンノーン", "Z", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("!", new Pokemon("アンノーン", "!", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));
            UnownDex.Add("?", new Pokemon("アンノーン", "?", new uint[] { 48, 72, 48, 72, 48, 48 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "ふゆう", "---" }, false, GenderRatio.Genderless));

            DeoxysDex.Add("N", new Pokemon("デオキシス", "N", new uint[] { 50, 150, 50, 150, 50, 150 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DeoxysDex.Add("A", new Pokemon("デオキシス", "A", new uint[] { 50, 180, 20, 180, 20, 150 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DeoxysDex.Add("D", new Pokemon("デオキシス", "D", new uint[] { 50, 70, 160, 70, 160, 90 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));
            DeoxysDex.Add("S", new Pokemon("デオキシス", "S", new uint[] { 50, 95, 90, 95, 90, 180 }, new PokeType[] { PokeType.Psychic, PokeType.Non }, new string[] { "プレッシャー", "---" }, false, GenderRatio.Genderless));

            DexDictionary = DexData.ToDictionary(_ => _.Name, _ => _);
        }
    }

}
