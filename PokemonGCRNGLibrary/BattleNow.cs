using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    public static class BattleNow
    {
        public static class SingleBattle
        {
            public static readonly RentalPartyRank Ultimate = new RentalPartyRank("シングル最強", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("バシャーモ", Gender.Male, Nature.Sassy),
                    new GCSlot("ラフレシア", Gender.Female, Nature.Gentle),
                    new GCSlot("ランターン", Gender.Female, Nature.Modest),
                    new GCSlot("オニゴーリ", Gender.Male, Nature.Rash),
                    new GCSlot("グランブル", Gender.Male, Nature.Naughty),
                    new GCSlot("ジュペッタ", Gender.Female, Nature.Naughty),
                },
                new GCSlot[] {
                    new GCSlot("エンテイ", Gender.Genderless, Nature.Hasty),
                    new GCSlot("ゴローニャ", Gender.Female, Nature.Impish),
                    new GCSlot("ベトベトン", Gender.Male, Nature.Lonely),
                    new GCSlot("コータス", Gender.Male, Nature.Mild),
                    new GCSlot("ライボルト", Gender.Female, Nature.Mild),
                    new GCSlot("ドククラゲ", Gender.Male, Nature.Serious),
                },
                new GCSlot[] {
                    new GCSlot("ラグラージ", Gender.Male, Nature.Brave),
                    new GCSlot("フーディン", Gender.Female, Nature.Mild),
                    new GCSlot("ルンパッパ", Gender.Male, Nature.Modest),
                    new GCSlot("トドゼルガ", Gender.Female, Nature.Bashful),
                    new GCSlot("ゴルダック", Gender.Male, Nature.Modest),
                    new GCSlot("バクオング", Gender.Female, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("ライコウ", Gender.Genderless, Nature.Mild),
                    new GCSlot("キュウコン", Gender.Female, Nature.Rash),
                    new GCSlot("マタドガス", Gender.Female, Nature.Adamant),
                    new GCSlot("ツボツボ", Gender.Female, Nature.Sassy),
                    new GCSlot("アーマルド", Gender.Male, Nature.Adamant),
                    new GCSlot("ネイティオ", Gender.Male, Nature.Quirky),
                },
                new GCSlot[] {
                    new GCSlot("メガニウム", Gender.Male, Nature.Quiet),
                    new GCSlot("バクフーン", Gender.Male, Nature.Mild),
                    new GCSlot("オーダイル", Gender.Male, Nature.Modest),
                    new GCSlot("エーフィ", Gender.Male, Nature.Rash),
                    new GCSlot("ブラッキー", Gender.Male, Nature.Bold),
                    new GCSlot("カイロス", Gender.Female, Nature.Naughty),
                },
                new GCSlot[] {
                    new GCSlot("スイクン", Gender.Genderless, Nature.Modest),
                    new GCSlot("デンリュウ", Gender.Female, Nature.Quiet),
                    new GCSlot("ネンドール", Gender.Genderless, Nature.Lonely),
                    new GCSlot("オドシシ", Gender.Male, Nature.Adamant),
                    new GCSlot("ポリゴン2", Gender.Genderless, Nature.Rash),
                    new GCSlot("ドンファン", Gender.Female, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("メタグロス", Gender.Genderless, Nature.Lonely),
                    new GCSlot("ユレイドル", Gender.Male, Nature.Impish),
                    new GCSlot("カイリキー", Gender.Male, Nature.Adamant),
                    new GCSlot("エアームド", Gender.Female, Nature.Lonely),
                    new GCSlot("サイドン", Gender.Female, Nature.Adamant),
                    new GCSlot("ハリテヤマ", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("ヘラクロス", Gender.Female, Nature.Adamant),
                    new GCSlot("ソーナンス", Gender.Male, Nature.Timid),
                    new GCSlot("ミロカロス", Gender.Female, Nature.Modest),
                    new GCSlot("ドードリオ", Gender.Male, Nature.Adamant),
                    new GCSlot("ノクタス", Gender.Female, Nature.Modest),
                    new GCSlot("ヤミラミ", Gender.Male, Nature.Adamant),
                }
            });
            public static readonly RentalPartyRank Hard = new RentalPartyRank("シングル強い", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("バクーダ", Gender.Male, Nature.Impish),
                    new GCSlot("ランターン", Gender.Female, Nature.Quirky),
                    new GCSlot("ラフレシア", Gender.Female, Nature.Careful),
                    new GCSlot("マルノーム", Gender.Male, Nature.Docile),
                    new GCSlot("アブソル", Gender.Female, Nature.Bold),
                    new GCSlot("オドシシ", Gender.Female, Nature.Bashful),
                },
                new GCSlot[] {
                    new GCSlot("ブーピッグ", Gender.Female, Nature.Naughty),
                    new GCSlot("ハリテヤマ", Gender.Male, Nature.Modest),
                    new GCSlot("グランブル", Gender.Female, Nature.Lonely),
                    new GCSlot("ジュペッタ", Gender.Male, Nature.Adamant),
                    new GCSlot("コータス", Gender.Male, Nature.Hasty),
                    new GCSlot("ライチュウ", Gender.Male, Nature.Sassy),
                },
                new GCSlot[] {
                    new GCSlot("ダーテング", Gender.Male, Nature.Bold),
                    new GCSlot("マルマイン", Gender.Genderless, Nature.Relaxed),
                    new GCSlot("バクオング", Gender.Male, Nature.Impish),
                    new GCSlot("ドククラゲ", Gender.Male, Nature.Quirky),
                    new GCSlot("ゴローニャ", Gender.Female, Nature.Gentle),
                    new GCSlot("オニゴーリ", Gender.Male, Nature.Docile),
                },
                new GCSlot[] {
                    new GCSlot("キレイハナ", Gender.Female, Nature.Careful),
                    new GCSlot("サイドン", Gender.Male, Nature.Calm),
                    new GCSlot("サクラビス", Gender.Male, Nature.Gentle),
                    new GCSlot("マタドガス", Gender.Female, Nature.Docile),
                    new GCSlot("レアコイル", Gender.Genderless, Nature.Careful),
                    new GCSlot("フーディン", Gender.Male, Nature.Relaxed),
                },
                new GCSlot[] {
                    new GCSlot("ユレイドル", Gender.Male, Nature.Naughty),
                    new GCSlot("カイロス", Gender.Female, Nature.Bold),
                    new GCSlot("アーマルド", Gender.Male, Nature.Calm),
                    new GCSlot("ミルタンク", Gender.Female, Nature.Relaxed),
                    new GCSlot("ネンドール", Gender.Genderless, Nature.Sassy),
                    new GCSlot("ホエルオー", Gender.Female, Nature.Careful),
                },
                new GCSlot[] {
                    new GCSlot("ドンファン", Gender.Female, Nature.Gentle),
                    new GCSlot("ゴルダック", Gender.Male, Nature.Jolly),
                    new GCSlot("ザングース", Gender.Male, Nature.Brave),
                    new GCSlot("デンリュウ", Gender.Female, Nature.Impish),
                    new GCSlot("ヘラクロス", Gender.Male, Nature.Bold),
                    new GCSlot("ヘルガー", Gender.Male, Nature.Relaxed),
                },
                new GCSlot[] {
                    new GCSlot("ベトベトン", Gender.Female, Nature.Bashful),
                    new GCSlot("サメハダー", Gender.Male, Nature.Calm),
                    new GCSlot("キュウコン", Gender.Female, Nature.Quirky),
                    new GCSlot("ポリゴン2", Gender.Genderless, Nature.Impish),
                    new GCSlot("カイリキー", Gender.Male, Nature.Bold),
                    new GCSlot("サーナイト", Gender.Female, Nature.Impish),
                },
                new GCSlot[] {
                    new GCSlot("ケッキング", Gender.Female, Nature.Naive),
                    new GCSlot("ギャラドス", Gender.Male, Nature.Impish),
                    new GCSlot("ボスゴドラ", Gender.Male, Nature.Lax),
                    new GCSlot("トドゼルガ", Gender.Male, Nature.Relaxed),
                    new GCSlot("ライボルト", Gender.Female, Nature.Quiet),
                    new GCSlot("ノクタス", Gender.Female, Nature.Careful),
                },
            });
            public static readonly RentalPartyRank Normal = new RentalPartyRank("シングル普通", new GCSlot[][]
            {
                new GCSlot[]
                {
                    new GCSlot("サニーゴ", Gender.Female, Nature.Sassy),
                    new GCSlot("ポポッコ", Gender.Male, Nature.Impish),
                    new GCSlot("ゴーリキー", Gender.Male, Nature.Brave),
                    new GCSlot("プラスル", Gender.Male, Nature.Brave),
                    new GCSlot("アゲハント", Gender.Male, Nature.Mild),
                    new GCSlot("マッスグマ", Gender.Female, Nature.Modest),
                },
                new GCSlot[]
                {
                    new GCSlot("ユンゲラー", Gender.Male, Nature.Sassy),
                    new GCSlot("クチート", Gender.Male, Nature.Naughty),
                    new GCSlot("ドクケイル", Gender.Female, Nature.Careful),
                    new GCSlot("オオスバメ", Gender.Female, Nature.Sassy),
                    new GCSlot("ジュプトル", Gender.Male, Nature.Lax),
                    new GCSlot("ヌマクロー", Gender.Male, Nature.Docile),
                },
                new GCSlot[]
                {
                    new GCSlot("マイナン", Gender.Female, Nature.Docile),
                    new GCSlot("アリアドス", Gender.Male, Nature.Bold),
                    new GCSlot("サイホーン", Gender.Female, Nature.Hardy),
                    new GCSlot("デルビル", Gender.Male, Nature.Sassy),
                    new GCSlot("ヘイガニ", Gender.Male, Nature.Adamant),
                    new GCSlot("ソーナンス", Gender.Male, Nature.Naive),
                },
                new GCSlot[]
                {
                    new GCSlot("ユキワラシ", Gender.Female, Nature.Serious),
                    new GCSlot("ベトベター", Gender.Female, Nature.Bold),
                    new GCSlot("コダック", Gender.Female, Nature.Hasty),
                    new GCSlot("コイル", Gender.Genderless, Nature.Rash),
                    new GCSlot("ヒノアラシ", Gender.Male, Nature.Relaxed),
                    new GCSlot("ヨーギラス", Gender.Male, Nature.Relaxed),
                },
                new GCSlot[]
                {
                    new GCSlot("チリーン", Gender.Female, Nature.Impish),
                    new GCSlot("ドードー", Gender.Female, Nature.Jolly),
                    new GCSlot("ケーシィ", Gender.Male, Nature.Naive),
                    new GCSlot("キモリ", Gender.Male, Nature.Quirky),
                    new GCSlot("ビリリダマ", Gender.Genderless, Nature.Naive),
                    new GCSlot("ヒトデマン", Gender.Genderless, Nature.Timid),
                },
                new GCSlot[]
                {
                    new GCSlot("デリバード", Gender.Female, Nature.Careful),
                    new GCSlot("モココ", Gender.Female, Nature.Sassy),
                    new GCSlot("バネブー", Gender.Male, Nature.Adamant),
                    new GCSlot("チルット", Gender.Female, Nature.Naughty),
                    new GCSlot("メノクラゲ", Gender.Male, Nature.Brave),
                    new GCSlot("ドンメル", Gender.Male, Nature.Lonely),
                },
                new GCSlot[]
                {
                    new GCSlot("ベイリーフ", Gender.Male, Nature.Bold),
                    new GCSlot("マグマラシ", Gender.Male, Nature.Modest),
                    new GCSlot("アリゲイツ", Gender.Male, Nature.Calm),
                    new GCSlot("ヤミカラス", Gender.Female, Nature.Brave),
                    new GCSlot("チャーレム", Gender.Female, Nature.Impish),
                    new GCSlot("トドグラー", Gender.Female, Nature.Mild),
                },
                new GCSlot[]
                {
                    new GCSlot("ポワルン", Gender.Female, Nature.Bashful),
                    new GCSlot("ネイティ", Gender.Female, Nature.Hasty),
                    new GCSlot("ホエルコ", Gender.Female, Nature.Bold),
                    new GCSlot("メタング", Gender.Genderless, Nature.Hasty),
                    new GCSlot("コモルー", Gender.Male, Nature.Gentle),
                    new GCSlot("グラエナ", Gender.Male, Nature.Docile),
                }
            });
            public static readonly RentalPartyRank Easy = new RentalPartyRank("シングル弱い", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("ピチュー", Gender.Male, Nature.Impish),
                    new GCSlot("マクノシタ", Gender.Male, Nature.Serious),
                    new GCSlot("ポチエナ", Gender.Male, Nature.Lonely),
                    new GCSlot("ヨマワル", Gender.Female, Nature.Careful),
                    new GCSlot("タネボー", Gender.Female, Nature.Modest),
                    new GCSlot("ジグザグマ", Gender.Female, Nature.Hasty),
                },
                new GCSlot[] {
                    new GCSlot("マリル", Gender.Female, Nature.Rash),
                    new GCSlot("ズバット", Gender.Male, Nature.Sassy),
                    new GCSlot("ドジョッチ", Gender.Male, Nature.Mild),
                    new GCSlot("マグマッグ", Gender.Female, Nature.Bashful),
                    new GCSlot("レディバ", Gender.Male, Nature.Adamant),
                    new GCSlot("エネコ", Gender.Female, Nature.Impish),
                },
                new GCSlot[] {
                    new GCSlot("ソーナノ", Gender.Male, Nature.Lax),
                    new GCSlot("ウリムー", Gender.Female, Nature.Gentle),
                    new GCSlot("オタチ", Gender.Male, Nature.Brave),
                    new GCSlot("ホーホー", Gender.Female, Nature.Careful),
                    new GCSlot("キルリア", Gender.Female, Nature.Sassy),
                    new GCSlot("キャモメ", Gender.Female, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("プリン", Gender.Female, Nature.Jolly),
                    new GCSlot("アサナン", Gender.Male, Nature.Serious),
                    new GCSlot("タマザラシ", Gender.Male, Nature.Timid),
                    new GCSlot("メリープ", Gender.Female, Nature.Quiet),
                    new GCSlot("イシツブテ", Gender.Male, Nature.Rash),
                    new GCSlot("イトマル", Gender.Male, Nature.Hardy),
                },
                new GCSlot[] {
                    new GCSlot("ラクライ", Gender.Male, Nature.Modest),
                    new GCSlot("ロコン", Gender.Female, Nature.Bold),
                    new GCSlot("ナマケロ", Gender.Male, Nature.Relaxed),
                    new GCSlot("クヌギダマ", Gender.Male, Nature.Brave),
                    new GCSlot("カゲボウズ", Gender.Female, Nature.Bashful),
                    new GCSlot("タッツー", Gender.Male, Nature.Quirky),
                },
                new GCSlot[] {
                    new GCSlot("キノココ", Gender.Male, Nature.Sassy),
                    new GCSlot("ゴニョニョ", Gender.Female, Nature.Lonely),
                    new GCSlot("ユキワラシ", Gender.Female, Nature.Quiet),
                    new GCSlot("アメタマ", Gender.Female, Nature.Naive),
                    new GCSlot("ピカチュウ", Gender.Male, Nature.Hardy),
                    new GCSlot("サンド", Gender.Female, Nature.Docile),
                },
                new GCSlot[] {
                    new GCSlot("ヒノアラシ", Gender.Male, Nature.Calm),
                    new GCSlot("ケーシィ", Gender.Male, Nature.Timid),
                    new GCSlot("ドードー", Gender.Female, Nature.Hasty),
                    new GCSlot("ワンリキー", Gender.Male, Nature.Brave),
                    new GCSlot("ワニノコ", Gender.Male, Nature.Docile),
                    new GCSlot("チルット", Gender.Female, Nature.Quiet),
                },
                new GCSlot[] {
                    new GCSlot("バネブー", Gender.Female, Nature.Naive),
                    new GCSlot("ベトベター", Gender.Male, Nature.Hardy),
                    new GCSlot("ツチニン", Gender.Male, Nature.Serious),
                    new GCSlot("ココドラ", Gender.Female, Nature.Lonely),
                    new GCSlot("ラブカス", Gender.Female, Nature.Lax),
                    new GCSlot("デルビル", Gender.Male, Nature.Brave),
                },
            });
        }
        public static class DoubleBattle
        {
            public static readonly RentalPartyRank Ultimate = new RentalPartyRank("ダブル最強", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("ヘラクロス", Gender.Male, Nature.Careful),
                    new GCSlot("オオスバメ", Gender.Female, Nature.Jolly),
                    new GCSlot("ミロカロス", Gender.Female, Nature.Calm),
                    new GCSlot("テッカニン", Gender.Male, Nature.Jolly),
                    new GCSlot("マタドガス", Gender.Female, Nature.Adamant),
                    new GCSlot("キュウコン", Gender.Female, Nature.Lax),
                },
                new GCSlot[] {
                    new GCSlot("メタグロス", Gender.Genderless, Nature.Adamant),
                    new GCSlot("ノクタス", Gender.Male, Nature.Modest),
                    new GCSlot("ツボツボ", Gender.Female, Nature.Calm),
                    new GCSlot("レジスチル", Gender.Genderless, Nature.Careful),
                    new GCSlot("ユレイドル", Gender.Female, Nature.Sassy),
                    new GCSlot("アーマルド", Gender.Female, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("ジュカイン", Gender.Male, Nature.Timid),
                    new GCSlot("グランブル", Gender.Female, Nature.Adamant),
                    new GCSlot("ラグラージ", Gender.Male, Nature.Sassy),
                    new GCSlot("レジロック", Gender.Genderless, Nature.Brave),
                    new GCSlot("エアームド", Gender.Female, Nature.Adamant),
                    new GCSlot("バシャーモ", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("オオスバメ", Gender.Female, Nature.Jolly),
                    new GCSlot("ハリテヤマ", Gender.Male, Nature.Careful),
                    new GCSlot("ミルタンク", Gender.Female, Nature.Brave),
                    new GCSlot("フーディン", Gender.Male, Nature.Modest),
                    new GCSlot("ワタッコ", Gender.Female, Nature.Calm),
                    new GCSlot("サメハダー", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("スターミー", Gender.Genderless, Nature.Modest),
                    new GCSlot("レジアイス", Gender.Genderless, Nature.Modest),
                    new GCSlot("ポリゴン2", Gender.Genderless, Nature.Hardy),
                    new GCSlot("マルマイン", Gender.Genderless, Nature.Modest),
                    new GCSlot("レアコイル", Gender.Genderless, Nature.Bashful),
                    new GCSlot("ソルロック", Gender.Genderless, Nature.Quiet),
                },
                new GCSlot[] {
                    new GCSlot("メガニウム", Gender.Female, Nature.Quiet),
                    new GCSlot("バクフーン", Gender.Male, Nature.Bashful),
                    new GCSlot("オーダイル", Gender.Male, Nature.Adamant),
                    new GCSlot("エーフィ", Gender.Female, Nature.Modest),
                    new GCSlot("ブラッキー", Gender.Male, Nature.Bold),
                    new GCSlot("リングマ", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("サイドン", Gender.Male, Nature.Hardy),
                    new GCSlot("ライボルト", Gender.Female, Nature.Naive),
                    new GCSlot("フライゴン", Gender.Male, Nature.Docile),
                    new GCSlot("ギャラドス", Gender.Male, Nature.Brave),
                    new GCSlot("マンタイン", Gender.Female, Nature.Calm),
                    new GCSlot("ドードリオ", Gender.Male, Nature.Jolly),
                },
                new GCSlot[] {
                    new GCSlot("スイクン", Gender.Genderless, Nature.Careful),
                    new GCSlot("ライコウ", Gender.Genderless, Nature.Naive),
                    new GCSlot("エンテイ", Gender.Genderless, Nature.Modest),
                    new GCSlot("ボスゴドラ", Gender.Male, Nature.Brave),
                    new GCSlot("ドンファン", Gender.Female, Nature.Hardy),
                    new GCSlot("ボーマンダ", Gender.Male, Nature.Jolly),
                },
            });
            public static readonly RentalPartyRank Hard = new RentalPartyRank("ダブル強い", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("ムウマ", Gender.Female, Nature.Jolly),
                    new GCSlot("ソーナンス", Gender.Female, Nature.Hardy),
                    new GCSlot("ヌオー", Gender.Female, Nature.Sassy),
                    new GCSlot("オオスバメ", Gender.Male, Nature.Jolly),
                    new GCSlot("ゴルバット", Gender.Male, Nature.Careful),
                    new GCSlot("サンドパン", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("オオタチ", Gender.Male, Nature.Jolly),
                    new GCSlot("チャーレム", Gender.Male, Nature.Timid),
                    new GCSlot("ヌマクロー", Gender.Male, Nature.Sassy),
                    new GCSlot("ネイティオ", Gender.Male, Nature.Modest),
                    new GCSlot("ザングース", Gender.Male, Nature.Adamant),
                    new GCSlot("ハブネーク", Gender.Male, Nature.Bashful),
                },
                new GCSlot[] {
                    new GCSlot("ユンゲラー", Gender.Male, Nature.Hasty),
                    new GCSlot("マッスグマ", Gender.Female, Nature.Impish),
                    new GCSlot("サニーゴ", Gender.Female, Nature.Bashful),
                    new GCSlot("レディアン", Gender.Female, Nature.Quirky),
                    new GCSlot("ドードリオ", Gender.Male, Nature.Quiet),
                    new GCSlot("イノムー", Gender.Male, Nature.Rash),
                },
                new GCSlot[] {
                    new GCSlot("グラエナ", Gender.Male, Nature.Modest),
                    new GCSlot("アメモース", Gender.Female, Nature.Serious),
                    new GCSlot("カポエラー", Gender.Male, Nature.Careful),
                    new GCSlot("クチート", Gender.Female, Nature.Careful),
                    new GCSlot("グランブル", Gender.Male, Nature.Brave),
                    new GCSlot("オドシシ", Gender.Female, Nature.Modest),
                },
                new GCSlot[] {
                    new GCSlot("アリゲイツ", Gender.Male, Nature.Mild),
                    new GCSlot("ヤミカラス", Gender.Female, Nature.Rash),
                    new GCSlot("ピカチュウ", Gender.Female, Nature.Relaxed),
                    new GCSlot("ベトベトン", Gender.Male, Nature.Lonely),
                    new GCSlot("キレイハナ", Gender.Female, Nature.Docile),
                    new GCSlot("ゴーリキー", Gender.Male, Nature.Adamant),
                },
                new GCSlot[] {
                    new GCSlot("ワカシャモ", Gender.Male, Nature.Adamant),
                    new GCSlot("トロピウス", Gender.Female, Nature.Quiet),
                    new GCSlot("カクレオン", Gender.Female, Nature.Lonely),
                    new GCSlot("ナマズン", Gender.Male, Nature.Mild),
                    new GCSlot("ニューラ", Gender.Female, Nature.Modest),
                    new GCSlot("マルノーム", Gender.Male, Nature.Serious),
                },
                new GCSlot[] {
                    new GCSlot("マグマラシ", Gender.Female, Nature.Serious),
                    new GCSlot("トドグラー", Gender.Male, Nature.Gentle),
                    new GCSlot("キリンリキ", Gender.Male, Nature.Modest),
                    new GCSlot("ミルタンク", Gender.Female, Nature.Adamant),
                    new GCSlot("ゴローニャ", Gender.Male, Nature.Adamant),
                    new GCSlot("ベイリーフ", Gender.Male, Nature.Hardy),
                },
                new GCSlot[] {
                    new GCSlot("ライチュウ", Gender.Male, Nature.Timid),
                    new GCSlot("ゴルダック", Gender.Male, Nature.Modest),
                    new GCSlot("ドンファン", Gender.Female, Nature.Serious),
                    new GCSlot("カイロス", Gender.Male, Nature.Brave),
                    new GCSlot("ジュプトル", Gender.Female, Nature.Timid),
                    new GCSlot("プクリン", Gender.Female, Nature.Careful),
                },
            });
            public static readonly RentalPartyRank Normal = new RentalPartyRank("ダブル普通", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("ピカチュウ", Gender.Female, Nature.Hardy),
                    new GCSlot("コダック", Gender.Male, Nature.Modest),
                    new GCSlot("キモリ", Gender.Female, Nature.Docile),
                    new GCSlot("ヨーギラス", Gender.Male, Nature.Lonely),
                    new GCSlot("ミズゴロウ", Gender.Female, Nature.Lax),
                    new GCSlot("アチャモ", Gender.Male, Nature.Naive),
                },
                new GCSlot[] {
                    new GCSlot("ナゾノクサ", Gender.Female, Nature.Mild),
                    new GCSlot("ヒメグマ", Gender.Male, Nature.Rash),
                    new GCSlot("ゴマゾウ", Gender.Female, Nature.Brave),
                    new GCSlot("チコリータ", Gender.Male, Nature.Quirky),
                    new GCSlot("ワニノコ", Gender.Female, Nature.Naughty),
                    new GCSlot("ヒノアラシ", Gender.Male, Nature.Modest),
                },
                new GCSlot[] {
                    new GCSlot("タツベイ", Gender.Male, Nature.Hasty),
                    new GCSlot("チョンチー", Gender.Male, Nature.Modest),
                    new GCSlot("キバニア", Gender.Female, Nature.Gentle),
                    new GCSlot("ベトベター", Gender.Male, Nature.Jolly),
                    new GCSlot("ケーシィ", Gender.Female, Nature.Timid),
                    new GCSlot("デルビル", Gender.Male, Nature.Hardy),
                },
                new GCSlot[] {
                    new GCSlot("オタチ", Gender.Female, Nature.Impish),
                    new GCSlot("ビブラーバ", Gender.Male, Nature.Calm),
                    new GCSlot("サイホーン", Gender.Male, Nature.Lonely),
                    new GCSlot("デリバード", Gender.Female, Nature.Relaxed),
                    new GCSlot("ビリリダマ", Gender.Genderless, Nature.Timid),
                    new GCSlot("ネイティ", Gender.Male, Nature.Modest),
                },
                new GCSlot[] {
                    new GCSlot("ゴローン", Gender.Female, Nature.Brave),
                    new GCSlot("チルット", Gender.Female, Nature.Calm),
                    new GCSlot("プラスル", Gender.Male, Nature.Docile),
                    new GCSlot("マイナン", Gender.Female, Nature.Hasty),
                    new GCSlot("ホエルコ", Gender.Female, Nature.Gentle),
                    new GCSlot("サナギラス", Gender.Male, Nature.Serious),
                },
                new GCSlot[] {
                    new GCSlot("レディアン", Gender.Female, Nature.Gentle),
                    new GCSlot("コータス", Gender.Male, Nature.Gentle),
                    new GCSlot("チリーン", Gender.Female, Nature.Gentle),
                    new GCSlot("ドーブル", Gender.Male, Nature.Gentle),
                    new GCSlot("サンドパン", Gender.Male, Nature.Gentle),
                    new GCSlot("サニーゴ", Gender.Female, Nature.Gentle),
                },
                new GCSlot[] {
                    new GCSlot("ウパー", Gender.Male, Nature.Naughty),
                    new GCSlot("グライガー", Gender.Male, Nature.Hasty),
                    new GCSlot("サボネア", Gender.Male, Nature.Modest),
                    new GCSlot("モココ", Gender.Female, Nature.Quirky),
                    new GCSlot("ウリムー", Gender.Female, Nature.Docile),
                    new GCSlot("ドンメル", Gender.Male, Nature.Mild),
                },
                new GCSlot[] {
                    new GCSlot("ノズパス", Gender.Male, Nature.Sassy),
                    new GCSlot("パールル", Gender.Male, Nature.Careful),
                    new GCSlot("ゴーリキー", Gender.Female, Nature.Impish),
                    new GCSlot("アリアドス", Gender.Male, Nature.Calm),
                    new GCSlot("チャーレム", Gender.Female, Nature.Serious),
                    new GCSlot("エネコロロ", Gender.Male, Nature.Jolly),
                },
            });
            public static readonly RentalPartyRank Easy = new RentalPartyRank("ダブル弱い", new GCSlot[][]
            {
                new GCSlot[] {
                    new GCSlot("ポチエナ", Gender.Male, Nature.Naughty),
                    new GCSlot("クヌギダマ", Gender.Female, Nature.Quirky),
                    new GCSlot("ウリムー", Gender.Female, Nature.Mild),
                    new GCSlot("サンド", Gender.Male, Nature.Bashful),
                    new GCSlot("ナマケロ", Gender.Female, Nature.Impish),
                    new GCSlot("メリープ", Gender.Female, Nature.Timid),
                },
                new GCSlot[] {
                    new GCSlot("ラルトス", Gender.Female, Nature.Calm),
                    new GCSlot("キノココ", Gender.Male, Nature.Sassy),
                    new GCSlot("オタチ", Gender.Male, Nature.Hasty),
                    new GCSlot("ロコン", Gender.Female, Nature.Mild),
                    new GCSlot("ヨマワル", Gender.Female, Nature.Sassy),
                    new GCSlot("イトマル", Gender.Male, Nature.Relaxed),
                },
                new GCSlot[] {
                    new GCSlot("ゴニョニョ", Gender.Male, Nature.Gentle),
                    new GCSlot("カゲボウズ", Gender.Female, Nature.Adamant),
                    new GCSlot("マグマッグ", Gender.Male, Nature.Quiet),
                    new GCSlot("ツチニン", Gender.Female, Nature.Relaxed),
                    new GCSlot("ドジョッチ", Gender.Male, Nature.Brave),
                    new GCSlot("アサナン", Gender.Female, Nature.Naughty),
                },
                new GCSlot[] {
                    new GCSlot("ピチュー", Gender.Male, Nature.Bold),
                    new GCSlot("ヤジロン", Gender.Genderless, Nature.Quiet),
                    new GCSlot("ハスボー", Gender.Female, Nature.Calm),
                    new GCSlot("ブルー", Gender.Male, Nature.Lonely),
                    new GCSlot("レディバ", Gender.Male, Nature.Docile),
                    new GCSlot("マリル", Gender.Male, Nature.Mild),
                },
                new GCSlot[] {
                    new GCSlot("ヒマナッツ", Gender.Female, Nature.Timid),
                    new GCSlot("トゲピー", Gender.Male, Nature.Naive),
                    new GCSlot("ケムッソ", Gender.Male, Nature.Serious),
                    new GCSlot("ココドラ", Gender.Male, Nature.Careful),
                    new GCSlot("ヌケニン", Gender.Genderless, Nature.Naive),
                    new GCSlot("マクノシタ", Gender.Female, Nature.Hardy),
                },
                new GCSlot[] {
                    new GCSlot("エネコ", Gender.Female, Nature.Impish),
                    new GCSlot("プリン", Gender.Female, Nature.Adamant),
                    new GCSlot("キルリア", Gender.Female, Nature.Bashful),
                    new GCSlot("アメタマ", Gender.Female, Nature.Modest),
                    new GCSlot("ナックラー", Gender.Female, Nature.Careful),
                    new GCSlot("ジグザグマ", Gender.Male, Nature.Naive),
                },
                new GCSlot[] {
                    new GCSlot("ダンバル", Gender.Genderless, Nature.Lonely),
                    new GCSlot("ワンリキー", Gender.Male, Nature.Careful),
                    new GCSlot("ラクライ", Gender.Male, Nature.Rash),
                    new GCSlot("タッツー", Gender.Male, Nature.Calm),
                    new GCSlot("ユキワラシ", Gender.Male, Nature.Bold),
                    new GCSlot("ドンメル", Gender.Male, Nature.Naughty),
                },
                new GCSlot[] {
                    new GCSlot("パールル", Gender.Female, Nature.Serious),
                    new GCSlot("コイル", Gender.Genderless, Nature.Lax),
                    new GCSlot("タネボー", Gender.Female, Nature.Jolly),
                    new GCSlot("ププリン", Gender.Female, Nature.Rash),
                    new GCSlot("ドガース", Gender.Male, Nature.Gentle),
                    new GCSlot("イシツブテ", Gender.Female, Nature.Adamant),
                },
            });
        }
    }
    public class RentalPartyRank : IReadOnlyList<IReadOnlyList<GCSlot>>
    {
        private readonly IReadOnlyList<IReadOnlyList<GCSlot>> parties;
        public RentalBattleResult GenerateBattleTeam(uint seed)
        {
            uint StartingSeed = seed;

            uint EnemyTeamIndex = seed.GetRand(8);
            uint PlayerTeamIndex;
            do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            var EnemyParty = parties[(int)EnemyTeamIndex];
            var EList = new List<GCIndividual>();
            for (int i = 0; i < 6; i++)
            {
                EList.Add(EnemyParty[i].Generate(seed, out seed, EnemyTSV));
            }
            uint PlayerNameIndex = seed.GetRand(3);

            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            var PlayerParty = parties[(int)PlayerTeamIndex];
            var PList = new List<GCIndividual>();
            for (int i = 0; i < 6; i++)
            {
                PList.Add(PlayerParty[i].Generate(seed, out seed, PlayerTSV));
            }

            return new RentalBattleResult()
            {
                StartingSeed = StartingSeed,
                FinishingSeed = seed,
                code = (short)(PlayerNameIndex * 8 + PlayerTeamIndex),
                PlayerName = RentalBattleResult.PlayerNameList[PlayerNameIndex],
                EnemyParty = EList,
                PlayerParty = PList
            };
        }
        public byte GenerateCode(uint seed, out uint finSeed)
        {
            uint EnemyTeamIndex = seed.GetRand(8);
            uint PlayerTeamIndex;
            do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            var EnemyParty = parties[(int)EnemyTeamIndex];
            var EList = new List<GCIndividual>();
            for (int i = 0; i < 6; i++)
            {
                EList.Add(EnemyParty[i].Generate(seed, out seed, EnemyTSV));
            }
            uint PlayerNameIndex = seed.GetRand(3);

            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            var PlayerParty = parties[(int)PlayerTeamIndex];
            var PList = new List<GCIndividual>();
            for (int i = 0; i < 6; i++)
            {
                PList.Add(PlayerParty[i].Generate(seed, out seed, PlayerTSV));
            }

            finSeed = seed;

            return (byte)(PlayerNameIndex * 8 + PlayerTeamIndex);
        }

        public uint AdvanceSeed(uint seed)
        {
            uint EnemyTeamIndex = seed.GetRand(8);
            uint PlayerTeamIndex;
            do { PlayerTeamIndex = seed.GetRand(8); } while (EnemyTeamIndex == PlayerTeamIndex);

            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            var EnemyParty = parties[(int)EnemyTeamIndex];
            for (int i = 0; i < 6; i++)
            {
                EnemyParty[i].Generate(seed, out seed, EnemyTSV);
            }

            seed.GetRand(3); // PlayerName

            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            var PlayerParty = parties[(int)PlayerTeamIndex];
            for (int i = 0; i < 6; i++)
            {
                PlayerParty[i].Generate(seed, out seed, PlayerTSV);
            }

            return seed;
        }
        public readonly string RuleName;
        internal RentalPartyRank(string label, GCSlot[][] p)
        {
            RuleName = label;
            parties = p;
        }

        public IReadOnlyList<GCSlot> this[int i] { get { return parties[i]; } }
        public IReadOnlyList<GCSlot> this[uint i] { get { return parties[(int)i]; } }
        public int Count { get { return parties.Count; } }

        public IEnumerator<IReadOnlyList<GCSlot>> GetEnumerator()
        {
            return parties.AsEnumerable().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class RentalBattleResult
    {
        static internal readonly string[] PlayerNameList = { "レオ", "ユータ", "タツキ" };
        public short code { get; set; }
        public string PlayerName { get; set; }
        public IReadOnlyList<GCIndividual> PlayerParty { get; set; }
        public IReadOnlyList<GCIndividual> EnemyParty { get; set; }
        public uint StartingSeed { get; set; }
        public uint FinishingSeed { get; set; }
    }
}
