using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{
    public enum Nature
    {
        /// <summary> がんばりや </summary>
        Hardy,
        /// <summary> さみしがり </summary>
        Lonely,
        /// <summary> ゆうかん </summary>
        Brave,
        /// <summary> いじっぱり </summary>
        Adamant,
        /// <summary> やんちゃ </summary>
        Naughty,
        /// <summary> ずぶとい </summary>
        Bold,
        /// <summary> すなお </summary>
        Docile,
        /// <summary> のんき </summary>
        Relaxed,
        /// <summary> わんぱく </summary>
        Impish,
        /// <summary> のうてんき </summary>
        Lax,
        /// <summary> おくびょう </summary>
        Timid,
        /// <summary> せっかち </summary>
        Hasty,
        /// <summary> まじめ </summary>
        Serious,
        /// <summary> ようき </summary>
        Jolly,
        /// <summary> むじゃき </summary>
        Naive,
        /// <summary> ひかえめ </summary>
        Modest,
        /// <summary> おっとり </summary>
        Mild,
        /// <summary> れいせい </summary>
        Quiet,
        /// <summary> てれや </summary>
        Bashful,
        /// <summary> うっかりや </summary>
        Rash,
        /// <summary> おだやか </summary>
        Calm,
        /// <summary> おとなしい </summary>
        Gentle,
        /// <summary> なまいき </summary>
        Sassy,
        /// <summary> しんちょう </summary>
        Careful,
        /// <summary> きまぐれ </summary>
        Quirky,
        other
    }
    public enum Gender { Genderless, Male, Female }
    public enum GenderRatio : uint
    {
        MaleOnly = 0,
        M7F1 = 0x1F,
        M3F1 = 0x3F,
        M1F1 = 0x7F,
        M1F3 = 0xBF,
        FemaleOnly = 0x100,
        Genderless = 0x12C
    }
    public enum PokeType
    {
        Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison,
        Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Non
    }
    public enum PlayerTeam
    {
        Mewtwo,
        Mew,
        Deoxys,
        Rayquaza,
        Jirachi
    }
    public enum COMTeam
    {
        Articuno,
        Zapdos,
        Moltres,
        Kangaskhan,
        Latias
    }
    public static class EnumExtention
    {
        static private readonly string[] Nature_JP =
           {
            "がんばりや", "さみしがり", "ゆうかん", "いじっぱり",
            "やんちゃ", "ずぶとい", "すなお", "のんき", "わんぱく",
            "のうてんき", "おくびょう", "せっかち", "まじめ", "ようき",
            "むじゃき", "ひかえめ", "おっとり", "れいせい", "てれや",
            "うっかりや", "おだやか", "おとなしい",
            "なまいき", "しんちょう", "きまぐれ", "---"
        };
        static private readonly double[][] Magnifications =
            {
                new double[] { 1, 1, 1, 1, 1, 1 },
                new double[] { 1, 1.1, 0.9, 1, 1, 1 },
                new double[] { 1, 1.1, 1, 1, 1, 0.9 },
                new double[] { 1, 1.1, 1, 0.9, 1, 1 },
                new double[] { 1, 1.1, 1, 1, 0.9, 1 },
                new double[] { 1, 0.9, 1.1, 1, 1, 1 },
                new double[] { 1, 1, 1, 1, 1, 1 },
                new double[] { 1, 1, 1.1, 1, 1, 0.9 },
                new double[] { 1, 1, 1.1, 0.9, 1, 1 },
                new double[] { 1, 1, 1.1, 1, 0.9, 1 },
                new double[] { 1, 0.9, 1,1, 1, 1.1 },
                new double[] { 1, 1, 0.9, 1,1, 1.1 },
                new double[] { 1, 1,1, 1, 1, 1 },
                new double[] { 1, 1,1, 0.9, 1, 1.1 },
                new double[] { 1, 1,1, 1, 0.9, 1.1 },
                new double[] { 1, 0.9, 1, 1.1, 1,1 },
                new double[] { 1, 1, 0.9, 1.1, 1, 1 },
                new double[] { 1, 1, 1, 1.1, 1, 0.9 },
                new double[] { 1, 1, 1, 1, 1, 1 },
                new double[] { 1, 1, 1, 1.1, 0.9, 1 },
                new double[] { 1, 0.9, 1,1, 1.1, 1 },
                new double[] { 1, 1, 0.9, 1, 1.1, 1},
                new double[] { 1, 1, 1, 1, 1.1, 0.9 },
                new double[] { 1, 1, 1, 0.9, 1.1, 1 },
                new double[] { 1, 1, 1, 1, 1, 1 },
                new double[] { 1, 1, 1, 1, 1, 1 }
            };
        static private readonly string[] PokeType_Kanji = new string[]
        {
            "普","炎","水","草","電","氷","闘","毒","地","飛","超","虫","岩","霊","龍","悪","鋼","Error"
        };
        static private readonly string[] PokeType_JP = new string[]
        {
            "ノーマル","ほのお","みず","くさ","でんき","こおり","かくとう","どく","じめん","ひこう","エスパー","むし","いわ","ゴースト","ドラゴン","あく","はがね","---"
        };
        static public Gender Reverse(this Gender gender) { if (gender == Gender.Male) return Gender.Female; else if (gender == Gender.Female) return Gender.Male; else return Gender.Genderless; }

        public static string ToKanji(this PokeType pokeType) { return PokeType_Kanji[(int)pokeType]; }
        public static string ToJapanese(this PokeType pokeType) { return PokeType_JP[(int)pokeType]; }
        public static string ToJapanese(this Nature nature) { return Nature_JP[(int)nature]; }
        public static double[] ToMagnification(this Nature nature) { return Magnifications[(int)nature]; }
        public static string ToSymbol(this Gender gender) { if (gender == Gender.Male) return "♂"; else if (gender == Gender.Female) return "♀"; else return "-"; }
    }
}
