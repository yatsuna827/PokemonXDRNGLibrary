using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;

namespace PokemonXDRNGLibrary
{
    public static class XDRNGSystem
    {
        public static uint[] GenerateEVs(ref this uint seed)
        {
            var EVs = new uint[6];
            int sumEV = 0;
            for (int i = 0; i < 101; i++)
            {
                for (int j = 0; j < 6; j++)
                    EVs[j] = (EVs[j] + seed.GetRand(0x100)) & 0xFF;

                sumEV = EVs.Sum(_ => (int)_);
                if (sumEV == 510) return EVs;
                if (sumEV <= 490) continue;
                if (sumEV < 530) break;
                if (i != 100) EVs = new uint[6];
            }

            sumEV = EVs.Sum(_ => (int)_);
            var k = 0;
            while (sumEV != 510)
            {
                if (sumEV < 510 && EVs[k] < 255) { EVs[k]++; sumEV++; }
                if (sumEV > 510 && EVs[k] != 0) { EVs[k]--; sumEV--; }
                k = (k + 1) % 6;
            }
            return EVs;
        }

        internal static readonly IReadOnlyList<XDDarkPokemon> XDDarkPokemonList;
        internal static readonly Dictionary<string, XDDarkPokemon> XDDarkPokemonDictionary;

        static public IReadOnlyList<uint> CalcSmokeAdvanceList(uint seed, int maxFrame)
        {
            var counter = new SmokeCalculator();
            var resList = new List<uint>() { seed };
            for (int i = 0; i < maxFrame; i++)
            {
                if (i == 2) seed.Advance(2);
                if (i == 5) seed.Advance(8);
                counter.CountUp(ref seed);
                resList.Add(seed.PrevSeed(2));
            }

            return resList;
        }

        public static XDDarkPokemon GetDarkPokemon(int index) { return XDDarkPokemonList[index]; }
        public static XDDarkPokemon GetDarkPokemon(string name) { return XDDarkPokemonDictionary[name]; }
        public static IReadOnlyList<XDDarkPokemon> GetXDDarkPokemonList() { return XDDarkPokemonList; }

        public static void AdvanceNameScreen(ref this uint seed)
        {
            if (seed.GetRand_f() < 0.1f) seed.Advance(4);
        }

        public static int MirorB_Rader(uint seed)
        {
            var r = seed.GetRand(0x20);
            if (r < 0x10) return 1;
            if (r < 0x18) return -1;

            return 0;
        }

        public static string MirorB_Map(uint seed)
        {
            var r = seed.GetRand(9);
            if (r < 3) return "パイラ";
            if (r < 6) return "ラルガ";
            if (r == 6) return "岩場";
            if (r == 7) return "オアシス";
            return "洞窟";
        }

        static XDRNGSystem()
        {
            // TODO
            // パラスだったかガーディだったかの設定を間違えていたかも…。
            var xdList = new List<XDDarkPokemon>();
            xdList.Add(new XDDarkPokemon("ヒメグマ", 11));
            xdList.Add(new XDDarkPokemon("レディバ", 10, new PreGenerateSlot[]
            {
                new PreGenerateSlot("スバメ", Gender.Female, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("ポチエナ", 10, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ズバット", Gender.Female, Nature.Serious)
            }));
            xdList.Add(new XDDarkPokemon("デルビル", 17));
            xdList.Add(new XDDarkPokemon("タマザラシ", 17, new PreGenerateSlot[]
            {
                new PreGenerateSlot("タッツー", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("トサキント", Gender.Female, Nature.Serious),
                // new PreGenerateSlot("ダンバル", Gender.Genderless, Nature.Hardy),
            }));
            xdList.Add(new XDDarkPokemon("ヤジロン", 17));
            xdList.Add(new XDDarkPokemon("メリープ", 17));
            xdList.Add(new XDDarkPokemon("ゴクリン", 17, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ドガース", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ベトベター", Gender.Male, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("タネボー", 17, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ナゾノクサ", Gender.Male, Nature.Docile),
                new PreGenerateSlot("サボネア", Gender.Female, Nature.Quirky),
                new PreGenerateSlot("キノココ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("ハスボー", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("クヌギダマ", Gender.Male, Nature.Serious),
            }));
            xdList.Add(new XDDarkPokemon("イトマル", 14, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ウリムー", Gender.Female, Nature.Serious),
                new PreGenerateSlot("カゲボウズ", Gender.Male, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("ドンメル", 14, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ラルトス", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ビリリダマ", Gender.Genderless, Nature.Hardy),
                new PreGenerateSlot("タツベイ", Gender.Female, Nature.Quirky),
            }));
            xdList.Add(new XDDarkPokemon("キバニア", 15));
            xdList.Add(new XDDarkPokemon("キノココ", 15, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ブルー", Gender.Female, Nature.Quirky),
                new PreGenerateSlot("カクレオン", Gender.Female, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("エネコロロ", 18, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ラブカス", Gender.Female, Nature.Docile),
                new PreGenerateSlot("アゲハント", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("ロゼリア", Gender.Male, Nature.Quirky)
            }));
            xdList.Add(new XDDarkPokemon("ビリリダマ", 19, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハスブレロ", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("ハスブレロ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ハスブレロ", Gender.Female, Nature.Serious)
            }));
            xdList.Add(new XDDarkPokemon("マクノシタ", 18, new PreGenerateSlot[]
            {
                new PreGenerateSlot("カクレオン", Gender.Male, Nature.Docile),
                new PreGenerateSlot("アメタマ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ロコン", 18, new PreGenerateSlot[]
            {
                new PreGenerateSlot("イトマル", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("アゲハント", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ドクケイル", Gender.Male, Nature.Bashful),
            }));
            xdList.Add(new XDDarkPokemon("ヨマワル", 19, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ニューラ", Gender.Male, Nature.Serious),
                new PreGenerateSlot("ヤンヤンマ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("ムウマ", Gender.Male, Nature.Quirky)
            }));
            xdList.Add(new XDDarkPokemon("ラルトス", 20, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ユンゲラー", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("モココ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ヤルキモノ", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("クチート", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ドゴーム", Gender.Male, Nature.Docile),
                new PreGenerateSlot("キリンリキ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ユキワラシ", 20, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハブネーク", Gender.Female, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("クヌギダマ", 20, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヤミカラス", Gender.Male, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("ネイティ", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("キルリア", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("マッスグマ", Gender.Female, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("ロゼリア", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("テッポウオ", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ゴルバット", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ニャース", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ユンゲラー", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ニューラ", Gender.Female, Nature.Hardy),
                new PreGenerateSlot("ムウマ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ウリムー", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("コータス", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("コノハナ", Gender.Male, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("オニスズメ", 22, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ペリッパー", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("ラクライ", Gender.Male, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("ベトベター", 23, new PreGenerateSlot[]
            {
                new PreGenerateSlot("チリーン", Gender.Male, Nature.Serious),
                new PreGenerateSlot("オドシシ", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("パウワウ", 23, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ホーホー", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ゴローン", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ゴクリン", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ルナトーン", 25, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ランターン", Gender.Female, Nature.Hardy),
                new PreGenerateSlot("ヌオー", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ザングース", 28));
            xdList.Add(new XDDarkPokemon("ノズパス", 26, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハスブレロ", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("ハスブレロ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ハスブレロ", Gender.Female, Nature.Serious)
            }));
            xdList.Add(new XDTogepii());
            xdList.Add(new XDDarkPokemon("パラス", 28, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハブネーク", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ヤミカラス", Gender.Female, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("ガーディ", 28, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハブネーク", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ヤミカラス", Gender.Female, Nature.Docile),
                new PreGenerateDarkPokemon("パラス")
            }));
            xdList.Add(new XDDarkPokemon("シェルダー", 29));
            xdList.Add(new XDDarkPokemon("スピアー", 30));
            xdList.Add(new XDDarkPokemon("ピジョン", 30, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("スピアー"),
                new PreGenerateSlot("オオタチ", Gender.Male, Nature.Serious),
                new PreGenerateSlot("トゲチック", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("モンジャラ", 30, new PreGenerateSlot[]
            {
                new PreGenerateSlot("キュウコン", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ワタッコ", Gender.Male, Nature.Docile),
                new PreGenerateSlot("マリルリ", Gender.Female, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("バタフリー", 30, new PreGenerateSlot[]
            {
                new PreGenerateSlot("キュウコン", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ワタッコ", Gender.Male, Nature.Docile),
                new PreGenerateSlot("マリルリ", Gender.Female, Nature.Hardy),
                new PreGenerateDarkPokemon("モンジャラ")
            }));
            xdList.Add(new XDDarkPokemon("レアコイル", 30, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヌケニン", Gender.Genderless, Nature.Bashful),
                new PreGenerateSlot("ソーナンス", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("ビブラーバ", Gender.Female, Nature.Serious)
            }));
            xdList.Add(new XDDarkPokemon("モルフォン", 32, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ゴルダック", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("カポエラー", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ハリテヤマ", Gender.Male, Nature.Serious),
            }));
            xdList.Add(new XDDarkPokemon("ウツドン", 32, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ゴルダック", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("カポエラー", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ハリテヤマ", Gender.Male, Nature.Serious),
                new PreGenerateDarkPokemon("モルフォン")
            }));
            xdList.Add(new XDDarkPokemon("アーボック", 33, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ハンテール", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ノクタス", Gender.Female, Nature.Hardy),
                new PreGenerateSlot("マタドガス", Gender.Female, Nature.Serious),
                new PreGenerateSlot("リングマ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("オコリザル", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("コドラ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("トドグラー", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ヤドキング", Gender.Female, Nature.Docile),
                new PreGenerateSlot("リングマ", Gender.Male, Nature.Quirky)
            }));
            xdList.Add(new XDDarkPokemon("スリーパー", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("コドラ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("トドグラー", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ヤドキング", Gender.Female, Nature.Docile),
                new PreGenerateSlot("リングマ", Gender.Male, Nature.Quirky),
                new PreGenerateDarkPokemon("オコリザル")
            }));
            xdList.Add(new XDDarkPokemon("ゴルダック", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("シザリガー", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ペリッパー", Gender.Female, Nature.Docile),
                new PreGenerateSlot("マンタイン", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ヤミラミ", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("シザリガー", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ペリッパー", Gender.Female, Nature.Docile),
                new PreGenerateSlot("マンタイン", Gender.Female, Nature.Bashful),
                new PreGenerateDarkPokemon("ゴルダック")
            }));
            xdList.Add(new XDDarkPokemon("ドードリオ", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ネイティオ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("ラッタ", 34, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ネイティオ", Gender.Female, Nature.Bashful),
                new PreGenerateDarkPokemon("ドードリオ"),
                new PreGenerateSlot("ナマズン", Gender.Male, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("カモネギ", 36, new PreGenerateSlot[]
            {
                new PreGenerateSlot("サーナイト", Gender.Male, Nature.Serious),
                new PreGenerateSlot("サクラビス", Gender.Female, Nature.Hardy),
                new PreGenerateSlot("ロゼリア", Gender.Male, Nature.Quirky)
            }));
            xdList.Add(new XDDarkPokemon("チルタリス", 36, new PreGenerateSlot[]
            {
                new PreGenerateSlot("サーナイト", Gender.Male, Nature.Serious),
                new PreGenerateSlot("サクラビス", Gender.Female, Nature.Hardy),
                new PreGenerateSlot("ロゼリア", Gender.Male, Nature.Quirky),
                new PreGenerateDarkPokemon("カモネギ")
            }));
            xdList.Add(new XDDarkPokemon("ガルーラ", 35, new PreGenerateSlot[]
            {
                new PreGenerateSlot("マルマイン", Gender.Genderless, Nature.Hardy),
                new PreGenerateSlot("ムウマ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("ネンドール", Gender.Genderless, Nature.Serious)
            }));
            xdList.Add(new XDDarkPokemon("ジュペッタ", 37, new PreGenerateSlot[]
            {
                new PreGenerateSlot("マルマイン", Gender.Genderless, Nature.Hardy),
                new PreGenerateSlot("ムウマ", Gender.Female, Nature.Bashful),
                new PreGenerateSlot("ネンドール", Gender.Genderless, Nature.Serious),
                new PreGenerateDarkPokemon("ガルーラ")
            }));
            xdList.Add(new XDDarkPokemon("ブーバー", 36, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヘルガー", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("キュウコン", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ラフレシア", Gender.Female, Nature.Hardy)
            }));
            xdList.Add(new XDDarkPokemon("カイロス", 35, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヘルガー", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("キュウコン", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ラフレシア", Gender.Female, Nature.Hardy),
                new PreGenerateDarkPokemon("ブーバー")
            }));
            xdList.Add(new XDDarkPokemon("ギャロップ", 40, new PreGenerateSlot[]
            {
                new PreGenerateSlot("バクーダ", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("マタドガス", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ベトベトン", Gender.Female, Nature.Serious)
            }));
            xdList.Add(new XDDarkPokemon("マグカルゴ", 38, new PreGenerateSlot[]
            {
                new PreGenerateSlot("バクーダ", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("マタドガス", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ベトベトン", Gender.Female, Nature.Serious),
                new PreGenerateDarkPokemon("ギャロップ")
            }));
            xdList.Add(new XDDarkPokemon("エビワラー", 38, new PreGenerateSlot[]
            {
                new PreGenerateSlot("チャーレム", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("ゴローニャ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ネイティオ", Gender.Female, Nature.Bashful)
            }));
            xdList.Add(new XDDarkPokemon("サワムラー", 38, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ブーピッグ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("エアームド", Gender.Female, Nature.Serious),
                new PreGenerateSlot("メタング", Gender.Genderless, Nature.Docile),
                new PreGenerateSlot("ハリテヤマ", Gender.Female, Nature.Quirky),
            }));
            xdList.Add(new XDDarkPokemon("ベロリンガ", 38, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ランターン", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("レアコイル", Gender.Genderless, Nature.Docile)
            }));
            xdList.Add(new XDDarkPokemon("ストライク", 40, new PreGenerateSlot[]
            {
                new PreGenerateSlot("オドシシ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("バクオング", Gender.Male, Nature.Quirky),
            }));
            xdList.Add(new XDDarkPokemon("ラッキー", 39, new PreGenerateSlot[]
            {
                new PreGenerateSlot("オドシシ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("バクオング", Gender.Male, Nature.Quirky),
                new PreGenerateDarkPokemon("ストライク")
            }));
            xdList.Add(new XDDarkPokemon("ソルロック", 41, new PreGenerateSlot[]
            {
                new PreGenerateSlot("メタング", Gender.Genderless, Nature.Quirky),
                new PreGenerateSlot("ヌオー", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ハッサム", Gender.Female, Nature.Hardy),
            }));
            xdList.Add(new XDDarkPokemon("スターミー", 41, new PreGenerateSlot[]
            {
                new PreGenerateSlot("メタング", Gender.Genderless, Nature.Quirky),
                new PreGenerateSlot("ヌオー", Gender.Male, Nature.Docile),
                new PreGenerateSlot("ハッサム", Gender.Female, Nature.Hardy),
                new PreGenerateDarkPokemon("ソルロック"),
                new PreGenerateSlot("ポワルン", Gender.Male, Nature.Bashful),
            }));
            xdList.Add(new XDDarkPokemon("オオスバメ", 43));
            xdList.Add(new XDDarkPokemon("エレブー", 43, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("オオスバメ"),
                new PreGenerateSlot("フーディン", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("キングドラ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ヘラクロス", Gender.Female, Nature.Bashful),
            }));
            xdList.Add(new XDDarkPokemon("カビゴン", 43, new PreGenerateSlot[]
            {
                new PreGenerateSlot("オオスバメ"),
                new PreGenerateSlot("フーディン", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("キングドラ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ヘラクロス", Gender.Female, Nature.Bashful),
                new PreGenerateDarkPokemon("エレブー")
            }));
            xdList.Add(new XDDarkPokemon("ニョロボン", 42, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヤドキング", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("リングマ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ボスゴドラ", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("トドゼルガ", Gender.Female, Nature.Docile),
            }));
            xdList.Add(new XDDarkPokemon("バリヤード", 42, new PreGenerateSlot[]
            {
                new PreGenerateSlot("ヤドキング", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("リングマ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ボスゴドラ", Gender.Male, Nature.Quirky),
                new PreGenerateSlot("トドゼルガ", Gender.Female, Nature.Docile),
                new PreGenerateDarkPokemon("ニョロボン")
            }));
            xdList.Add(new XDDarkPokemon("ダグトリオ", 40, new PreGenerateSlot[]
            {
                new PreGenerateSlot("オニゴーリ", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("デンリュウ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("キノガッサ", Gender.Female, Nature.Docile),
                new PreGenerateSlot("ドンファン", Gender.Male, Nature.Serious),
            }));
            xdList.Add(new XDDarkPokemon("ライボルト", 44, new PreGenerateSlot[]
            {
                new PreGenerateSlot("テッカニン", Gender.Female, Nature.Docile),
            }));
            xdList.Add(new XDDarkPokemon("ボーマンダ", 50, new PreGenerateSlot[]
            {
                new PreGenerateSlot("テッカニン", Gender.Female, Nature.Docile),
                new PreGenerateDarkPokemon("ライボルト"),
            }));
            xdList.Add(new XDDarkPokemon("ガラガラ", 44, new PreGenerateSlot[]
            {
                new PreGenerateSlot("テッカニン", Gender.Female, Nature.Docile),
                new PreGenerateDarkPokemon("ライボルト"),
                new PreGenerateDarkPokemon("ボーマンダ"),
                new PreGenerateSlot("フライゴン", Gender.Male, Nature.Quirky),
            }));
            xdList.Add(new XDDarkPokemon("ラプラス", 44, new PreGenerateSlot[]
            {
                new PreGenerateSlot("テッカニン", Gender.Female, Nature.Docile),
                new PreGenerateDarkPokemon("ライボルト"),
                new PreGenerateDarkPokemon("ボーマンダ"),
                new PreGenerateSlot("フライゴン", Gender.Male, Nature.Quirky),
                new PreGenerateDarkPokemon("ガラガラ"),
            }));
            xdList.Add(new XDDarkPokemon("ルギア", 50));
            xdList.Add(new XDDarkPokemon("サイドン", 46));
            xdList.Add(new XDDarkPokemon("ファイヤー", 50, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("サイドン"),
            }));
            xdList.Add(new XDDarkPokemon("ナッシー", 46, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("サイドン"),
                new PreGenerateDarkPokemon("ファイヤー"),
            }));
            xdList.Add(new XDDarkPokemon("ケンタロス", 46, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("サイドン"),
                new PreGenerateDarkPokemon("ファイヤー"),
                new PreGenerateDarkPokemon("ナッシー"),
            }));
            xdList.Add(new XDDarkPokemon("フリーザー", 50, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("サイドン"),
                new PreGenerateDarkPokemon("ファイヤー"),
                new PreGenerateDarkPokemon("ナッシー"),
                new PreGenerateDarkPokemon("ケンタロス"),
            }));
            xdList.Add(new XDDarkPokemon("サンダー", 50, new PreGenerateSlot[]
            {
                new PreGenerateDarkPokemon("サイドン"),
                new PreGenerateDarkPokemon("ファイヤー"),
                new PreGenerateDarkPokemon("ナッシー"),
                new PreGenerateDarkPokemon("ケンタロス"),
                new PreGenerateDarkPokemon("フリーザー"),
            }));
            xdList.Add(new XDDarkPokemon("カイリュー", 55, new PreGenerateSlot[] 
            {
                new PreGenerateSlot("ルンパッパ", Gender.Male, Nature.Hardy),
                new PreGenerateSlot("ルンパッパ", Gender.Male, Nature.Bashful),
                new PreGenerateSlot("ルンパッパ", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ルンパッパ", Gender.Female, Nature.Serious),
                new PreGenerateSlot("ルンパッパ", Gender.Male, Nature.Hardy),
            }));

            XDDarkPokemonList = xdList;
            XDDarkPokemonDictionary = xdList.ToDictionary(_ => _.darkPokemon.Pokemon.Name, _ => _);
        }
    }
}
