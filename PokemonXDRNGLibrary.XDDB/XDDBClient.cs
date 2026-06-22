using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PokemonPRNG.LCG32.GCLCG;
using PokemonPRNG.LCG32;
using PokemonXDRNGLibrary.QuickBattle;

namespace PokemonXDRNGLibrary.XDDB
{
    public readonly struct QuickBattleInput
    {
        public byte PlayerTeam { get; }
        public byte EnemyTeam { get; }
        public uint HPCode { get; }
        public bool Check(uint pIndex, uint eIndex, uint hpCode)
            => hpCode == HPCode && pIndex == PlayerTeam && eIndex == EnemyTeam;

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

        public QuickBattleInput(PlayerTeam playerTeam, EnemyTeam enemyTeam, uint pFirstHP, uint pSecondHP, uint eFirstHP, uint eSecondHP)
        {
            PlayerTeam = (byte)playerTeam;
            EnemyTeam = (byte)enemyTeam;
            pFirstHP -= pBaseHP[PlayerTeam].First;
            pSecondHP -= pBaseHP[PlayerTeam].Second;
            eFirstHP -= eBaseHP[EnemyTeam].First;
            eSecondHP -= eBaseHP[EnemyTeam].Second;
            HPCode = (eFirstHP << 24) | (eSecondHP << 16) | (pFirstHP << 8) | pSecondHP;
        }
    }

    public class XDDBClient
    {
        private static readonly RecSplitMph mph = new RecSplitMph(XDDBIndex.N, XDDBIndex.BucketSize, XDDBIndex.BitOffset, XDDBIndex.Bits);

        private readonly uint[] seeds;

        public XDDBClient(Stream seedsStream)
        {
            seeds = LoadSeeds(seedsStream, XDDBIndex.N);
        }

        private static uint[] LoadSeeds(Stream stream, int count)
        {
            using (var br = new BinaryReader(stream))
            {
                var seeds = new uint[count];
                var hash = 0xCBF29CE484222325UL;
                for (int i = 0; i < count; i++)
                {
                    uint b0 = br.ReadByte(), b1 = br.ReadByte(), b2 = br.ReadByte();
                    hash = (hash ^ b0) * 0x100000001B3UL;
                    hash = (hash ^ b1) * 0x100000001B3UL;
                    hash = (hash ^ b2) * 0x100000001B3UL;
                    seeds[i] = b0 | b1 << 8 | b2 << 16;
                }

                // seedsのslot割り当てはindex部のMPHと対になっているため、対応しないデータを検出したら受け付けない
                if (hash != XDDBIndex.SeedsHash)
                    throw new InvalidDataException("The provided seeds data does not match the index embedded in this assembly.");

                return seeds;
            }
        }

        public IEnumerable<uint> Search(QuickBattleInput first, QuickBattleInput second)
        {
            var hasSeen = new HashSet<uint>();
            foreach (var seed in GetCandidateSeeds(first.HPCode))
            {
                for (uint h8 = 0; h8 < 0x100; h8++)
                {
                    var res = (h8 << 24 | seed).GenerateQuickBattle();
                    var next = res.seed.GenerateQuickBattle();

                    if (!first.Check(res.pIndex, res.eIndex, res.HP)) continue;
                    if (!second.Check(next.pIndex, next.eIndex, next.HP)) continue;
                    if (hasSeen.Contains(next.seed)) continue;

                    hasSeen.Add(next.seed);
                    yield return next.seed;
                }
            }
        }
        private IEnumerable<uint> GetCandidateSeeds(uint key)
        {
            var slot = mph.Lookup(key);
            yield return seeds[slot];

            var ovSlots = XDDBIndex.OverflowSlots;
            var idx = LowerBound(ovSlots, (uint)slot);
            for (int i = idx; i < ovSlots.Length && ovSlots[i] == (uint)slot; i++)
                yield return XDDBIndex.OverflowSeeds[i];
        }
        private static int LowerBound(uint[] array, uint value)
        {
            int lo = 0, hi = array.Length;
            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (array[mid] < value) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    static class EVsExt
    {
        public static (uint pIndex, uint eIndex, uint HP, uint seed) GenerateQuickBattle(this uint seed, uint tsv = 0x10000)
        {
            seed.Advance(); // PlaynerName
            var playerTeamIndex = seed.GetRand() % 5;
            var enemyTeamIndex = seed.GetRand() % 5;

            seed.Advance();
            var enemyTSV = seed.GetRand() ^ seed.GetRand();

            // 相手1匹目
            seed.Advance(); // dummyPID
            seed.Advance(); // dummyPID
            var hp0 = seed.GetRand() & 0x1F;
            seed.Advance(); // SCD
            seed.Advance(); // Ability
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ enemyTSV) >= 8) break; }
            hp0 += seed.GenerateEVs() / 4;

            // 相手2匹目
            seed.Advance();
            seed.Advance();
            var hp1 = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ enemyTSV) >= 8) break; }
            hp1 += seed.GenerateEVs() / 4;

            seed.Advance(3);

            // プレイヤー1匹目
            seed.Advance();
            seed.Advance();
            var hp2 = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ tsv) >= 8) break; }
            hp2 += seed.GenerateEVs() / 4;

            // プレイヤー2匹目
            seed.Advance();
            seed.Advance();
            var hp3 = seed.GetRand() & 0x1F;
            seed.Advance();
            seed.Advance();
            while (true) { if ((seed.GetRand() ^ seed.GetRand() ^ tsv) >= 8) break; }
            hp3 += seed.GenerateEVs() / 4;

            return (playerTeamIndex, enemyTeamIndex, (hp0 << 24) + (hp1 << 16) + (hp2 << 8) + (hp3), seed);
        }

        public static void Shave(this byte[] evs)
        {
            var sum = evs.Sum(_ => _);

            var k = 0;
            while (sum > 510)
            {
                if (evs[k] != 0) { evs[k]--; sum--; }
                if (++k == 6) k = 0;
            }
        }
        public static void Fill(this byte[] evs)
        {
            var sum = evs.Sum(_ => _);

            var k = 0;
            while (sum < 510)
            {
                if (evs[k] < 255) { evs[k]++; sum++; }
                if (++k == 6) k = 0;
            }
        }

        private static readonly uint[] evsCache = new uint[0x1000000];

        // 努力値生成処理を通した後のseedを返します。
        public static uint AdvanceEVs(this uint seed)
        {
            var ini = seed;

            if (evsCache[seed & 0xFFFFFF] != 0) return ini.NextSeed(evsCache[seed & 0xFFFFFF] & 0xFFFFFF);

            var seeds = new List<(uint ini, uint diff)>() { (seed & 0xFFFFFF, 0) };

            var evs = new byte[6];
            int sumEV = 0;
            for (uint i = 0; i < 101; i++)
            {
                for (int j = 0; j < 6; j++) evs[j] += (byte)(seed.GetRand() & 0xFF);
                sumEV = evs.Sum(_ => _);

                if (sumEV == 510)
                {
                    var index = seed.GetIndex(ini);
                    foreach (var (s, diff) in seeds)
                        evsCache[s] = (index - diff) | ((uint)evs[0] << 24);

                    return seed;
                }

                else if (sumEV <= 490) continue;

                else if (sumEV < 530)
                {
                    evs.Fill();
                    evs.Shave();

                    var index = seed.GetIndex(ini);
                    foreach (var (s, diff) in seeds)
                        evsCache[s] = (index - diff) | ((uint)evs[0] << 24);

                    return seed;
                }

                else if (i != 100)
                {
                    Array.Clear(evs, 0, 6);
                    seeds.Add((seed & 0xFFFFFF, (i + 1) * 6u));
                }
            }

            evs.Fill();
            evs.Shave();

            evsCache[ini & 0xFFFFFF] = seed.GetIndex(ini) | ((uint)evs[0] << 24);
            return seed;
        }

        // 努力値生成処理を通し、HPの努力値を返します。
        public static uint GenerateEVs(this ref uint seed)
        {
            var ini = seed;

            if (evsCache[seed & 0xFFFFFF] != 0)
            {
                var val = evsCache[seed & 0xFFFFFF];
                seed.Advance(val & 0xFFFFFF);
                return val >> 24;
            }

            var seeds = new List<(uint ini, uint diff)>() { (seed & 0xFFFFFF, 0) };

            var evs = new byte[6];
            int sumEV = 0;
            for (uint i = 0; i < 101; i++)
            {
                for (int j = 0; j < 6; j++) evs[j] += (byte)(seed.GetRand() & 0xFF);
                sumEV = evs.Sum(_ => _);

                if (sumEV == 510)
                {
                    var index = seed.GetIndex(ini);
                    foreach (var (s, diff) in seeds)
                        evsCache[s] = (index - diff) | ((uint)evs[0] << 24);

                    return evs[0];
                }

                else if (sumEV <= 490) continue;

                else if (sumEV < 530)
                {
                    evs.Fill();
                    evs.Shave();
                    var index = seed.GetIndex(ini);
                    foreach (var (s, diff) in seeds)
                        evsCache[s] = (index - diff) | ((uint)evs[0] << 24);

                    return evs[0];
                }

                else if (i != 100)
                {
                    Array.Clear(evs, 0, 6);
                    seeds.Add((seed & 0xFFFFFF, (i + 1) * 6u));
                }
            }

            evs.Fill();
            evs.Shave();

            evsCache[ini & 0xFFFFFF] = seed.GetIndex(ini) | ((uint)evs[0] << 24);
            return evs[0];
        }
    }
}
