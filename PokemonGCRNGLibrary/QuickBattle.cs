using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary
{
    public static class QuickBattle
    {
        private static readonly (GCSlot First, GCSlot Second)[] PlayerParty = new (GCSlot, GCSlot)[]
        {
            (new GCSlot("ミュウツー"), new GCSlot("エンテイ")),
            (new GCSlot("ミュウ"), new GCSlot("ライコウ")),
            (new GCSlot("デオキシス"), new GCSlot("ハピナス")),
            (new GCSlot("レックウザ"), new GCSlot("ネンドール")),
            (new GCSlot("ジラーチ"), new GCSlot("スイクン")),
        };
        private static readonly (GCSlot First, GCSlot Second)[] EnemyParty = new (GCSlot, GCSlot)[]
        {
            (new GCSlot("フリーザー"), new GCSlot("ラグラージ")),
            (new GCSlot("サンダー"), new GCSlot("バシャーモ")),
            (new GCSlot("ファイヤー"), new GCSlot("ジュカイン")),
            (new GCSlot("ガルーラ"), new GCSlot("ラティオス")),
            (new GCSlot("ラティアス"), new GCSlot("ゲンガー")),
        };
        private static readonly string[] battleField = new string[]
        {
            "パイラ", "ラルガ", "バトル山", "岩場", "オアシス", "洞窟"
        };
        public static (uint finSeed, QuickBattleResult result) GenerateBattleTeam(uint seed)
        {
            var res = new QuickBattleResult();

            seed.Advance();
            var playerTeamIndex = seed.GetRand(5);
            var enemyTeamIndex = seed.GetRand(5);
            res.GeneratedTeams = ((PlayerTeam)playerTeamIndex, (COMTeam)enemyTeamIndex);

            res.BattleField = battleField[seed.GetRand(6)];
            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            res.COM.First = EnemyParty[enemyTeamIndex].First.GenerateDummy(ref seed, EnemyTSV);
            res.COM.Second = EnemyParty[enemyTeamIndex].Second.GenerateDummy(ref seed, EnemyTSV);

            seed.Advance();
            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            res.P1.First = PlayerParty[playerTeamIndex].First.GenerateDummy(ref seed, PlayerTSV);
            res.P1.Second = PlayerParty[playerTeamIndex].Second.GenerateDummy(ref seed, PlayerTSV);

            return (seed, res);
        }
        public static QuickBattleResult GenerateBattleTeam(uint seed, out uint finSeed)
        {
            var res = new QuickBattleResult();

            seed.Advance();
            var playerTeamIndex = seed.GetRand(5);
            var enemyTeamIndex = seed.GetRand(5);

            res.BattleField = battleField[seed.GetRand(6)];
            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();
            res.COM.First = EnemyParty[enemyTeamIndex].First.GenerateDummy(ref seed, EnemyTSV);
            res.COM.Second = EnemyParty[enemyTeamIndex].Second.GenerateDummy(ref seed, EnemyTSV);

            seed.Advance();
            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();
            res.P1.First = PlayerParty[playerTeamIndex].First.GenerateDummy(ref seed, PlayerTSV);
            res.P1.Second = PlayerParty[playerTeamIndex].Second.GenerateDummy(ref seed, PlayerTSV);

            finSeed = seed;
            return res;
        }

        public class QuickBattleResult
        {
            public (GCIndividual First, GCIndividual Second) P1;
            public (GCIndividual First, GCIndividual Second) COM;
            public string BattleField;
            public (PlayerTeam PlayerTeam, COMTeam COMTeam) GeneratedTeams;
        }
    }
}
