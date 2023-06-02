using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;

namespace PokemonXDRNGLibrary
{
    public class PreGenerateSlot : GCSlot
    {
        internal PreGenerateSlot(string p, Gender g = Gender.Genderless, Nature n = Nature.other) : base(p, g, n) { }
        internal PreGenerateSlot(string p, uint lv, Gender g = Gender.Genderless, Nature n = Nature.other) : base(p, lv, g, n) { }

        // cell \mapsto "そのcellを生成可能なcell"の列
        internal virtual IEnumerable<CalcBackCell> CalcBack(CalcBackCell cell)
        {
            var seed = cell.seed;
            var TSV = cell.TSV;
            List<uint> psvList;
            // PIDのチェック.
            // PIDが条件を満たしていなければyield break.
            {
                var lid = seed >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;

                if (pid % 25 != (uint)FixedNature) yield break; // 性格不一致
                if (pid.GetGender(Pokemon.GenderRatio) != FixedGender) yield break; // 性別不一致
                if (TSV < 0x10000 && (lid ^ hid ^ TSV) < 8) yield break; // TSVに条件がついているが, そのTSVだと色回避に引っかかってしまう場合

                psvList = new List<uint>(cell.preGeneratedPSVList) { lid ^ hid };
            }

            // 逆算
            while (true)
            {
                yield return new CalcBackCell(seed.PrevSeed(6), TSV, psvList);

                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var lid = seed.Back() >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;
                if (pid % 25 == (uint)FixedNature && pid.GetGender(Pokemon.GenderRatio) == FixedGender)
                {
                    // 性格・性別が一致するPIDに当たったら
                    if (TSV < 0x10000 && (lid ^ hid ^ TSV) >= 8) yield break; // TSV指定済みで色回避が発生しないなら終了.
                    if (TSV == 0x10000)
                    {
                        TSV = lid ^ hid; // TSVが指定されていない場合はTSVを指定して続行. (色回避を発生させた場合のみ出現する個体を探す)
                        if (psvList.Any(_ => ((_ & 0xFFFF) ^ (_ >> 16) ^ TSV) < 8)) yield break; // 既に固定されているPIDが光ってしまうTSVはアウト
                    }
                }
            }
        }
    }

    class PreGenerateDarkPokemon : PreGenerateSlot
    {
        public bool isFixed;
        public uint[] FixedIVs = new uint[6];
        public uint FixedPID;
        internal PreGenerateDarkPokemon(string p, uint lv = 60) : base(p, lv, Gender.Genderless, Nature.other) { }

        public override GCIndividual Generate(uint seed, out uint finSeed)
        {
            if (!isFixed) return base.Generate(seed, out finSeed);

            var rep = seed;
            seed.Advance(2); // dummyPID
            // 個体値は生成されない
            var AbilityIndex = seed.GetRand(2);
            uint PID;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));
                break;
            }

            finSeed = seed;
            return Pokemon.GetIndividual(FixedPID, Lv, FixedIVs, AbilityIndex).SetRepSeed(rep);
        }
        public override GCIndividual Generate(uint seed, out uint finSeed, uint TSV)
        {
            if (!isFixed) return base.Generate(seed, out finSeed, TSV);

            var rep = seed;
            seed.Advance(2); // dummyPID
            uint AbilityIndex = seed.GetRand(2);
            uint PID;
            while (true)
            {
                PID = seed.GetPID(_ => (FixedGender == Gender.Genderless || _.GetGender(Pokemon.GenderRatio) == FixedGender) && (FixedNature == Nature.other || (Nature)(_ % 25) == FixedNature));

                if (!PID.IsShiny(TSV)) break;
            }

            finSeed = seed;
            return Pokemon.GetIndividual(FixedPID, Lv, FixedIVs, AbilityIndex).SetRepSeed(rep);
        }

        internal override IEnumerable<CalcBackCell> CalcBack(CalcBackCell cell)
        {
            var seed = cell.seed;
            var TSV = cell.TSV;
            List<uint> psvList;
            // PIDのチェック.
            // PIDが条件を満たしていなければyield break.
            {
                var lid = seed >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;

                if (TSV < 0x10000 && (lid ^ hid ^ TSV) < 8) yield break; // 色回避に引っかかる

                psvList = new List<uint>(cell.preGeneratedPSVList) { lid ^ hid };
            }

            // 逆算
            while (true)
            {
                yield return new CalcBackCell(seed.PrevSeed(6), TSV, psvList);
                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var lid = seed.Back() >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;

                if (TSV < 0x10000 && (lid ^ hid ^ TSV) >= 8) yield break; // TSV指定済みで色回避が発生しないなら終了.
                if (TSV == 0x10000)
                {
                    TSV = lid ^ hid; // TSVが指定されていない場合はTSVを指定して続行. (色回避を発生させた場合のみ出現する個体を探す)
                    if (psvList.Any(_ => ((_ & 0xFFFF) ^ (_ >> 16) ^ TSV) < 8)) yield break; // 既に固定されているPIDが光ってしまうTSVはアウト
                }
            }
        }
    }
}
