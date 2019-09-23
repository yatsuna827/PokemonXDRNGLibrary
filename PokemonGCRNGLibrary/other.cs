using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGCRNGLibrary
{

    public enum Nature
    {
        Hardy, Lonely, Brave, Adamant, Naughty,
        Bold, Docile, Relaxed, Impish, Lax,
        Timid, Hasty, Serious, Jolly, Naive,
        Modest, Mild, Quiet, Bashful, Rash,
        Calm, Gentle, Sassy, Careful, Quirky, other
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
        static public Gender Reverse(this Gender gender) { if (gender == Gender.Male) return Gender.Female; else if (gender == Gender.Female) return Gender.Male; else return Gender.Genderless; }

        public static string ToJapanese(this Nature nature) { return Nature_JP[(int)nature]; }
        public static double[] ToMagnification(this Nature nature) { return Magnifications[(int)nature]; }
        public static string ToSymbol(this Gender gender) { if (gender == Gender.Male) return "♂"; else if (gender == Gender.Female) return "♀"; else return "-"; }
    }
}
