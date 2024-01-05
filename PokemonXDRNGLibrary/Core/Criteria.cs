using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonStandardLibrary;

namespace PokemonXDRNGLibrary
{
    public class Criteria
    {
        public uint TSV;
        public bool checkShiny;
        bool checkIV => checkIVs.Any();
        public bool[] checkIVs;
        public bool checkNature;
        public bool checkAbility;
        public bool checkGender;
        public bool checkHiddenPowerType;

        public uint[] MinIVs;
        public uint[] MaxIVs;
        public uint MinHiddenPowerPower;
        public string targetAbility;
        public Nature targetNature;
        public Gender targetGender;
        public PokeType targetHiddenPowerType;

        public bool CheckShiny(bool isShiny) => !checkShiny || isShiny;
        public bool CheckIVs(uint[] IVs)
        {
            if (!checkIV) return true;

            for (int i = 0; i < 6; i++)
                if (checkIVs[i] && (IVs[i] < MinIVs[i] || MaxIVs[i] < IVs[i])) return false;

            return true;
        }
        public bool CheckAbility(string ability) => !checkAbility || ability == targetAbility;
        public bool CheckGender(Gender gender) => !checkGender || gender == targetGender;
        public bool CheckNature(Nature nature) => !checkNature || nature == targetNature;
        public bool CheckHiddenPowerType(PokeType pokeType) => !checkHiddenPowerType || pokeType == targetHiddenPowerType;
        public bool CheckHiddenPowerPower(uint power) => MinHiddenPowerPower <= power;


        public Criteria() { MinIVs = new uint[6]; MaxIVs = new uint[6]; checkIVs = new bool[6]; }
    }
    public class XDStarterCriteria : Criteria
    {
        public bool CheckTID;
        public uint[] targetTID = new uint[0];
        public bool CheckTSV;
        public uint[] targetTSV = new uint[0];
        public XDStarterCriteria() : base() { }
    }
    public class RNGTargetCriteria
    {
        public bool[] nature;
        public string ability="";
        public Gender gender = Gender.Genderless;

        public PokeType HiddenPowerType = PokeType.None;
        public uint MinHiddenPower = 30;
    }
}
