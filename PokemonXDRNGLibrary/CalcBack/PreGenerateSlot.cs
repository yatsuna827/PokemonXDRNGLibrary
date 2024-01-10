using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PokemonPRNG.LCG32.GCLCG;
using PokemonStandardLibrary;
using PokemonStandardLibrary.CommonExtension;
using static PokemonStandardLibrary.Gen3.Pokemon;

namespace PokemonXDRNGLibrary
{
    public class PreGenerateSlot : GCSlot
    {
        internal PreGenerateSlot(string p, Gender g = Gender.Genderless, Nature n = Nature.other) : base(p, g, n) { }
        internal PreGenerateSlot(string p, uint lv, Gender g = Gender.Genderless, Nature n = Nature.other) : base(p, lv, g, n) { }

        internal virtual IEnumerable<CalcBackCell> CalcBack(CalcBackCell cell)
        {
            var seed = cell.Seed.PrevSeed(); // 辻褄合わせ
            var TSV = cell.ConditionedTSV;

            // 逆算
            while (true)
            {
                yield return new CalcBackCell(seed.PrevSeed(6), TSV);

                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var lid = seed.Back() >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;
                if (pid % 25 == (uint)FixedNature && pid.GetGender(Species.GenderRatio) == FixedGender)
                {
                    // 性格・性別が一致するPIDに当たったら
                    if (TSV < 0x10000 && (lid ^ hid ^ TSV) >= 8) yield break; // TSV指定済みで色回避が発生しないなら終了.
                    if (TSV == 0x10000)
                    {
                        TSV = lid ^ hid; // TSVが指定されていない場合はTSVを指定して続行. (色回避を発生させた場合のみ出現する個体を探す)
                    }
                }
            }
        }

        internal virtual IEnumerable<uint> CalcBack(uint seed, uint tsv)
        {
            // PIDのチェック.
            // PIDが条件を満たしていなければyield break.
            {
                var lid = seed >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;

                if (pid % 25 != (uint)FixedNature) yield break; // 性格不一致
                if (pid.GetGender(Species.GenderRatio) != FixedGender) yield break; // 性別不一致
                if ((lid ^ hid ^ tsv) < 8) yield break; // 色回避に引っかかってしまう場合
            }

            // 逆算
            while (true)
            {
                yield return seed.PrevSeed(6);

                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var lid = seed.Back() >> 16;
                var hid = seed.Back() >> 16;
                var pid = hid << 16 | lid;

                if (pid % 25 == (uint)FixedNature && pid.GetGender(Species.GenderRatio) == FixedGender && ((lid ^ hid ^ tsv) >= 8)) yield break;
            }
        }

        public virtual bool CanGeneratedBy(uint seed, uint tsv)
        {
            var _seed = seed;

            var lid = seed >> 16;
            var hid = seed.Back() >> 16;
            var pid = hid << 16 | lid;

            if (pid % 25 != (uint)FixedNature) return false; // 性格不一致
            if (pid.GetGender(Species.GenderRatio) != FixedGender) return false; // 性別不一致
            if ((lid ^ hid ^ tsv) < 8) return false; // 色回避に引っかかってしまう場合

            return true;
        }
    }

    public class PreGenerateDarkPokemon : PreGenerateSlot
    {
        public bool isFixed;
        public uint[] FixedIVs = new uint[6];
        public uint FixedPID;
        internal PreGenerateDarkPokemon(string p, uint lv = 60) : base(p, lv, Gender.Genderless, Nature.other) { }

        public override void Use(ref uint seed)
        {
            if (isFixed) 
                seed.Advance(3); // dummyPID, ability
            else 
                seed.Advance(5); // dummyPID, IVs, ability

            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();
                if (FixedGender != Gender.Genderless && pid.GetGender(Species.GenderRatio) != FixedGender)
                    continue;
                if (FixedNature != Nature.other && (Nature)(pid % 25) != FixedNature)
                    continue;
                break;
            }
        }
        public override uint ComputeConsumption(uint seed)
        {
            if (isFixed)
                seed.Advance(3); // dummyPID, ability
            else
                seed.Advance(5); // dummyPID, IVs, ability

            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();
                if (FixedGender != Gender.Genderless && pid.GetGender(Species.GenderRatio) != FixedGender)
                    continue;
                if (FixedNature != Nature.other && (Nature)(pid % 25) != FixedNature)
                    continue;
                break;
            }

            return seed;
        }

        public override void Use(ref uint seed, uint TSV)
        {
            if (isFixed)
                seed.Advance(3); // dummyPID, ability
            else
                seed.Advance(5); // dummyPID, IVs, ability

            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();
                if (FixedGender != Gender.Genderless && pid.GetGender(Species.GenderRatio) != FixedGender)
                    continue;
                if (FixedNature != Nature.other && (Nature)(pid % 25) != FixedNature)
                    continue;
                if (pid.IsShiny(TSV))
                    continue;

                break;
            }
        }
        public override uint ComputeConsumption(uint seed, uint TSV)
        {
            if (isFixed)
                seed.Advance(3); // dummyPID, ability
            else
                seed.Advance(5); // dummyPID, IVs, ability

            while (true)
            {
                var pid = (seed.GetRand() << 16) | seed.GetRand();
                if (FixedGender != Gender.Genderless && pid.GetGender(Species.GenderRatio) != FixedGender)
                    continue;
                if (FixedNature != Nature.other && (Nature)(pid % 25) != FixedNature)
                    continue;
                if (pid.IsShiny(TSV))
                    continue;

                break;
            }

            return seed;
        }

        internal override IEnumerable<CalcBackCell> CalcBack(CalcBackCell cell)
        {
            var seed = cell.Seed.PrevSeed();
            var TSV = cell.ConditionedTSV;

            // 逆算
            while (true)
            {
                yield return new CalcBackCell(seed.PrevSeed(isFixed ? 4u : 6u), TSV);
                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var lid = seed.Back() >> 16;
                var hid = seed.Back() >> 16;

                // TSV指定済みで色回避が発生しないなら終了.
                if (TSV < 0x10000 && (lid ^ hid ^ TSV) >= 8) yield break;
                if (TSV == 0x10000)
                {
                    // TSVが指定されていない場合はTSVを指定して続行. (色回避を発生させた場合のみ出現する個体を探す)
                    TSV = lid ^ hid;
                }
            }
        }

        internal override IEnumerable<uint> CalcBack(uint seed, uint tsv)
        {
            // PIDのチェック.
            // PIDが条件を満たしていなければyield break.
            {
                var psv = (seed >> 16) ^ (seed.Back() >> 16);

                if ((psv ^ tsv) < 8) yield break; // 色回避に引っかかる
            }

            // 逆算
            while (true)
            {
                yield return seed.PrevSeed(isFixed ? 4u : 6u);

                // 条件を満たすPIDに当たるまで, seedを返し続ける.
                var psv = (seed.Back() >> 16) ^ (seed.Back() >> 16);

                if ((psv ^ tsv) >= 8) yield break; // 色回避が発生しないなら終了.
            }
        }

        public override bool CanGeneratedBy(uint seed, uint tsv)
        {
            var lid = seed >> 16;
            var hid = seed.Back() >> 16;

            // 色回避に引っかかる
            if ((lid ^ hid ^ tsv) < 8) return false;

            return true;
        }
    }

}
