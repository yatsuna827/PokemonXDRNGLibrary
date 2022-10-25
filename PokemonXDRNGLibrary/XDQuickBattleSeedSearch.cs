using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonGCRNGLibrary.XDQuickBattleSeedSearch
{
    public class XDQuickBattleArguments
    {
        private readonly uint pIndex;
        private readonly uint eIndex;
        internal readonly uint HPCode;
        public bool Check(uint pIndex, uint eIndex, uint hpCode) => hpCode == this.HPCode && pIndex == this.pIndex && eIndex == this.eIndex;

        private static readonly (uint First, uint Second)[] pBaseHP = new (uint First, uint Second)[]
        {
            (322, 340),
            (310, 290),
            (210, 620),
            (320, 230),
            (310, 310),
        };
        private static readonly (uint First, uint Second)[] eBaseHP = new (uint First, uint Second)[]
        {
            (290, 310),
            (290, 270),
            (290, 250),
            (320, 270),
            (270, 230),
        };

        public XDQuickBattleArguments(PlayerTeam playerTeam, COMTeam comTeam, uint pFirstHP, uint pSecondHP, uint eFirstHP, uint eSecondHP)
        {
            pIndex = (uint)playerTeam;
            eIndex = (uint)comTeam;
            pFirstHP -= pBaseHP[pIndex].First;
            pSecondHP -= pBaseHP[pIndex].Second;
            eFirstHP -= eBaseHP[eIndex].First;
            eSecondHP -= eBaseHP[eIndex].Second;
            HPCode = (eFirstHP << 24) | (eSecondHP << 16) | (pFirstHP << 8) | pSecondHP;
        }
    }
    public static class SeedSearch
    {
        private static readonly IReadOnlyList<(uint HP, uint Seed)> dataBase;

        static SeedSearch()
        {
            var seedList = new List<(uint, uint)>();

            using (var br = new BinaryReader(new MemoryStream(Properties.Resources.XDDB)))
            {
                var bs = br.BaseStream;
                while (bs.Position != bs.Length)
                {
                    seedList.Add((br.ReadUInt32(), br.ReadUInt32()));
                }
            }

            dataBase = seedList;
        }

        public static IEnumerable<uint> SearchSeed(XDQuickBattleArguments first, XDQuickBattleArguments second)
        {
            var key = first.HPCode;
            var idx = dataBase.Select(_ => _.HP).ToList().BinarySearch(key);
            if (idx < 0) yield break;

            var seedList = new List<uint>();
            for (int i = idx; dataBase[i].HP == key && i >= 0; i--) seedList.Add(dataBase[i].Seed);
            for (int i = idx + 1; dataBase[i].HP == key && i < dataBase.Count; i++) seedList.Add(dataBase[i].Seed);

            foreach (var seed in seedList)
            {
                for (uint h8 = 0; h8 < 0x100; h8++)
                {
                    var res = Generate(h8 << 24 | seed);
                    var next = Generate(res.seed);
                    if (first.Check(res.pIndex, res.eIndex, res.HP) && second.Check(next.pIndex, next.eIndex, next.HP))
                        yield return next.seed;
                }
            }
        }

        private static (uint pIndex, uint eIndex, uint HP, uint seed) Generate(uint seed)
        {
            seed.Advance(); // PlaynerName
            var playerTeamIndex = seed.GetRand() % 5;
            var enemyTeamIndex = seed.GetRand() % 5;

            var hp = new uint[4];

            seed.Advance();
            uint EnemyTSV = seed.GetRand() ^ seed.GetRand();

            // 相手1匹目
            seed.Advance(); // dummyPID
            seed.Advance(); // dummyPID
            hp[0] = seed.GetRand() & 0x1F;
            seed.Advance(); // SCD
            seed.Advance(); // Ability
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ EnemyTSV) >= 8) break; }
            hp[0] += seed.GenerateEVs() / 4;

            // 相手2匹目
            seed.Advance();
            seed.Advance();
            hp[1] = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ EnemyTSV) >= 8) break; }
            hp[1] += seed.GenerateEVs() / 4;

            seed.Advance();
            uint PlayerTSV = seed.GetRand() ^ seed.GetRand();

            // プレイヤー1匹目
            seed.Advance();
            seed.Advance();
            hp[2] = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ PlayerTSV) >= 8) break; }
            hp[2] += seed.GenerateEVs() / 4;

            // プレイヤー2匹目
            seed.Advance();
            seed.Advance();
            hp[3] = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ PlayerTSV) >= 8) break; }
            hp[3] += seed.GenerateEVs() / 4;

            return (playerTeamIndex, enemyTeamIndex, (hp[0] << 24) + (hp[1] << 16) + (hp[2] << 8) + (hp[3]), seed);
        }
        private static uint GenerateEVs(ref this uint seed)
        {
            var EVs = new byte[6];
            int sumEV = 0;
            for (var i = 0; i < 101; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    byte ev = (byte)(seed.GetRand() & 0xFF);
                    EVs[j] += ev;
                }
                sumEV = EVs.Sum(_ => _);

                if (sumEV == 510) return EVs[0];
                if (sumEV <= 490) continue;
                if (sumEV < 530) break;
                if (i != 100) EVs = new byte[6];
            }
            var k = 0;
            while (sumEV != 510)
            {
                if (sumEV < 510 && EVs[k] < 255) { EVs[k]++; sumEV++; }
                if (sumEV > 510 && EVs[k] != 0) { EVs[k]--; sumEV--; }
                k = (k + 1) % 6;
            }
            return EVs[0];
        }
    }
}
