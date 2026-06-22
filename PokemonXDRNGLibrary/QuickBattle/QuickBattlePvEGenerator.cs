using PokemonPRNG.LCG32;
using PokemonPRNG.LCG32.GCLCG;
using System.Security.Cryptography;

namespace PokemonXDRNGLibrary.QuickBattle
{
    public class QuickBattlePvEGenerator: IGeneratable<QuickBattlePvEResult>, IGeneratableEffectful<QuickBattlePvEResult>
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

        private static readonly GCSlot dummy = new GCSlot("Dummy", "Genderless");

        private static readonly string[] battleField = new [] { "パイラ", "ラルガ", "バトル山", "岩場", "オアシス", "洞窟" };

        private readonly uint _tsv;

        public QuickBattlePvEResult Generate(uint seed)
        {
            seed.Advance();
            var playerTeamIndex = seed.GetRand(5);
            var enemyTeamIndex = seed.GetRand(5);

            var field = battleField[seed.GetRand(6)];

            var enemyTSV = seed.GetRand() ^ seed.GetRand();

            var eTeam = (
                enemyTeam[enemyTeamIndex].First.GenerateDummy(ref seed, _tsv),
                enemyTeam[enemyTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            RerollDummyTSV(ref seed, ref enemyTSV, eTeam.Item1.PID, eTeam.Item2.PID);

            seed.Advance();

            var playerTSV = seed.GetRand() ^ seed.GetRand();

            var pTeam = (
                playerTeam[playerTeamIndex].First.GenerateDummy(ref seed, _tsv),
                playerTeam[playerTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            return new QuickBattlePvEResult(pTeam, eTeam, field, ((PlayerTeam)playerTeamIndex, (EnemyTeam)enemyTeamIndex));
        }

        public QuickBattlePvEResult Generate(ref uint seed)
        {
            seed.Advance();
            uint playerTeamIndex = seed.GetRand(5);
            uint enemyTeamIndex = seed.GetRand(5);

            string field = battleField[seed.GetRand(6)];

            uint enemyTSV = seed.GetRand() ^ seed.GetRand();

            var eTeam = (
                enemyTeam[enemyTeamIndex].First.GenerateDummy(ref seed, _tsv),
                enemyTeam[enemyTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            // PID
            RerollDummyTSV(ref seed, ref enemyTSV, eTeam.Item1.PID, eTeam.Item2.PID);

            seed.Advance();

            uint playerTSV = seed.GetRand() ^ seed.GetRand();

            var pTeam = (
                playerTeam[playerTeamIndex].First.GenerateDummy(ref seed, _tsv),
                playerTeam[playerTeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            RerollDummyTSV(ref seed, ref playerTSV, pTeam.Item1.PID, pTeam.Item2.PID);

            return new QuickBattlePvEResult(pTeam, eTeam, field, ((PlayerTeam)playerTeamIndex, (EnemyTeam)enemyTeamIndex));
        }

        public uint Use(uint seed)
        {
            uint seedCopy = seed;
            Generate(ref seedCopy);

            return seedCopy;
        }

        // see https://sina-poke.hatenablog.com/entry/2022/04/23/021304
        public uint EnterQuickBattle(uint seed)
        {
            seed.Advance(122);

            for (int i = 0; i < 4; i++)
            {
                uint tsv = seed.GetRand() ^ seed.GetRand();

                for (int j = 0; j < 2; j++)
                {
                    dummy.Use(ref seed, tsv);
                    seed.GenerateEVsDummy();
                }
            }
            return seed;
        }

        private void RerollDummyTSV(ref uint seed, ref uint tsv,　params uint[] pids)
        {
            foreach (uint pid in pids)
            {
                while (pid.IsShiny(tsv))
                {
                    tsv = seed.GetRand() ^ seed.GetRand();
                }
            }
        }

        public QuickBattlePvEGenerator(uint tsv)
            => _tsv = tsv;
    }

    public class QuickBattlePvEResult
    {
        public (GCIndividual First, GCIndividual Second) PlayerTeam { get; }
        public (GCIndividual First, GCIndividual Second) EnemyTeam { get; }
        public string BattleField { get; }
        public (PlayerTeam PlayerTeam, EnemyTeam EnemyTeam) GeneratedTeams { get; }

        public QuickBattlePvEResult((GCIndividual First, GCIndividual Second) p, (GCIndividual First, GCIndividual Second) e, string battleField, (PlayerTeam PlayerTeam, EnemyTeam COMTeam) generatedTeams)
        {
            PlayerTeam = p;
            EnemyTeam = e;
            BattleField = battleField;
            GeneratedTeams = generatedTeams;
        }
    }


    public class QuickBattlePvPGenerator : IGeneratable<QuickBattlePvPResult>, IGeneratableEffectful<QuickBattlePvPResult>
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

        private static readonly string[] battleField = new[] { "パイラ", "ラルガ", "バトル山", "岩場", "オアシス", "洞窟" };

        private readonly uint _tsv;

        public QuickBattlePvPResult Generate(uint seed) //TODO test this for edge cases
        {
            seed.Advance();
            uint player1TeamIndex = seed.GetRand(5);
            uint player2TeamIndex;
            do { player2TeamIndex = seed.GetRand(5); } while (player2TeamIndex == player1TeamIndex);

            var selectedTable = ((seed.GetRand() & 0x80u) != 0u) ? enemyTeam : playerTeam;

            string field = battleField[seed.GetRand(6)];

            uint player2TSV = seed.GetRand() ^ seed.GetRand();

            var p2Team = (
                selectedTable[player2TeamIndex].First.GenerateDummy(ref seed, _tsv),
                selectedTable[player2TeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            GenerateDummy(ref seed, ref player2TSV, p2Team.Item1.PID, p2Team.Item2.PID);

            seed.Advance();

            uint player1TSV = seed.GetRand() ^ seed.GetRand();

            var p1Team = (
                selectedTable[player1TeamIndex].First.GenerateDummy(ref seed, _tsv),
                selectedTable[player1TeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            return new QuickBattlePvPResult(p1Team, p2Team, field);
        }

        public QuickBattlePvPResult Generate(ref uint seed) //TODO test this for edge cases
        {
            seed.Advance();
            uint player1TeamIndex = seed.GetRand(5);
            uint player2TeamIndex;
            do { player2TeamIndex = seed.GetRand(5); } while (player2TeamIndex == player1TeamIndex);

            var selectedTable = ((seed.GetRand() & 0x80u) != 0u) ? enemyTeam : playerTeam;

            string field = battleField[seed.GetRand(6)];

            uint player2TSV = seed.GetRand() ^ seed.GetRand();

            var p2Team = (
                selectedTable[player2TeamIndex].First.GenerateDummy(ref seed, _tsv),
                selectedTable[player2TeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            GenerateDummy(ref seed, ref player2TSV, p2Team.Item1.PID, p2Team.Item2.PID);

            seed.Advance();

            uint player1TSV = seed.GetRand() ^ seed.GetRand();

            var p1Team = (
                selectedTable[player1TeamIndex].First.GenerateDummy(ref seed, _tsv),
                selectedTable[player1TeamIndex].Second.GenerateDummy(ref seed, _tsv)
            );

            GenerateDummy(ref seed, ref player1TSV, p1Team.Item1.PID, p1Team.Item2.PID);

            return new QuickBattlePvPResult(p1Team, p2Team, field);
        }

        public uint Use(uint seed)
        {
            Generate(ref seed);

            return seed;
        }

        // see https://sina-poke.hatenablog.com/entry/2024/02/08/000000
        private void GenerateDummy(ref uint seed, ref uint tsv, params uint[] pids)
        {
            foreach (uint pid in pids)
            {
                while (pid.IsShiny(tsv))
                {
                    tsv = seed.GetRand() ^ seed.GetRand();
                }
            }
        }

        public QuickBattlePvPGenerator(uint tsv)
            => _tsv = tsv;
    }

    public class QuickBattlePvPResult
    {
        public (GCIndividual First, GCIndividual Second) Player1Team { get; }
        public (GCIndividual First, GCIndividual Second) Player2Team { get; }
        public string BattleField { get; }

        public QuickBattlePvPResult((GCIndividual First, GCIndividual Second) p1, (GCIndividual First, GCIndividual Second) p2, string battleField)
        {
            Player1Team = p1;
            Player2Team = p2;
            BattleField = battleField;
        }
    }
}
