using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary.QuickBattle
{
    public class QuickBattleGenerator: IGeneratable<QuickBattleResult>, ISideEffectiveGeneratable<QuickBattleResult>
    {
        private static readonly (GCSlot First, GCSlot Second)[] playerTeam = new (GCSlot, GCSlot)[]
        {
            (new GCSlot("ミュウツー"), new GCSlot("エンテイ")),
            (new GCSlot("ミュウ"), new GCSlot("ライコウ")),
            (new GCSlot("デオキシス"), new GCSlot("ハピナス")),
            (new GCSlot("レックウザ"), new GCSlot("ネンドール")),
            (new GCSlot("ジラーチ"), new GCSlot("スイクン")),
        };
        private static readonly (GCSlot First, GCSlot Second)[] enemyTeam = new (GCSlot, GCSlot)[]
        {
            (new GCSlot("フリーザー"), new GCSlot("ラグラージ")),
            (new GCSlot("サンダー"), new GCSlot("バシャーモ")),
            (new GCSlot("ファイヤー"), new GCSlot("ジュカイン")),
            (new GCSlot("ガルーラ"), new GCSlot("ラティオス")),
            (new GCSlot("ラティアス"), new GCSlot("ゲンガー")),
        };

        private static readonly string[] battleField = new [] { "パイラ", "ラルガ", "バトル山", "岩場", "オアシス", "洞窟" };

        private readonly uint _tsv;

        public QuickBattleResult Generate(uint seed)
        {
            seed.Advance();
            var playerTeamIndex = seed.GetRand(5);
            var enemyTeamIndex = seed.GetRand(5);

            var field = battleField[seed.GetRand(6)];
            var enemyTSV = seed.GetRand() ^ seed.GetRand();
            var eTeam = (
                enemyTeam[enemyTeamIndex].First.GenerateDummy(ref seed, enemyTSV),
                enemyTeam[enemyTeamIndex].Second.GenerateDummy(ref seed, enemyTSV)
            );

            seed.Advance(3);

            var pTeam = (
                playerTeam[playerTeamIndex].First.GenerateDummy(ref seed, _tsv),
                playerTeam[playerTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            return new QuickBattleResult(pTeam, eTeam, field, ((PlayerTeam)playerTeamIndex, (EnemyTeam)enemyTeamIndex));
        }
        public QuickBattleResult Generate(ref uint seed)
        {
            seed.Advance();
            var playerTeamIndex = seed.GetRand(5);
            var enemyTeamIndex = seed.GetRand(5);

            var field = battleField[seed.GetRand(6)];
            var enemyTSV = seed.GetRand() ^ seed.GetRand();
            var eTeam = (
                enemyTeam[enemyTeamIndex].First.GenerateDummy(ref seed, enemyTSV),
                enemyTeam[enemyTeamIndex].Second.GenerateDummy(ref seed, enemyTSV)
            );

            seed.Advance(3);

            var pTeam = (
                playerTeam[playerTeamIndex].First.GenerateDummy(ref seed, _tsv),
                playerTeam[playerTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            return new QuickBattleResult(pTeam, eTeam, field, ((PlayerTeam)playerTeamIndex, (EnemyTeam)enemyTeamIndex));
        }

        public QuickBattleGenerator(uint tsv)
            => _tsv = tsv;
    }
    public class QuickBattleResult
    {
        public (GCIndividual First, GCIndividual Second) PlayerTeam { get; }
        public (GCIndividual First, GCIndividual Second) EnemyTeam { get; }
        public string BattleField { get; }
        public (PlayerTeam PlayerTeam, EnemyTeam EnemyTeam) GeneratedTeams { get; }

        public QuickBattleResult((GCIndividual First, GCIndividual Second) p, (GCIndividual First, GCIndividual Second) e, string battleField, (PlayerTeam PlayerTeam, EnemyTeam COMTeam) generatedTeams)
        {
            PlayerTeam = p;
            EnemyTeam = e;
            BattleField = battleField;
            GeneratedTeams = generatedTeams;
        }
    }
}
