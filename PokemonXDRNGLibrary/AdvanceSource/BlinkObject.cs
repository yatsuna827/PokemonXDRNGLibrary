using System;
using System.Linq;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary.AdvanceSource
{
    public class BlinkObject
    {
        private readonly int coolTime;
        private int remainCoolTime = 0;
        private int blinkCounter;
        public int Counter { get => blinkCounter; }

        public void Initialize(int initialCounter)
        {
            blinkCounter = initialCounter; 
            remainCoolTime = 0;
        }
        public bool CountUp(ref uint seed, ref uint index)
        {
            if (remainCoolTime-- > 0) return false; // マイナスに振り切るぶんには問題ないので.
            if ((blinkCounter ++) < 10) return false;

            index++;
            if (seed.GetRand() >= BlinkConst.blinkThresholds[blinkCounter - 10]) return false;

            blinkCounter = 0;
            remainCoolTime = coolTime;

            return true;
        }
        public BlinkObject(int cool, int initCounter = 0)
        {
            coolTime = cool;
            Initialize(initCounter);
        }
    }

    public class BlinkObjectEnumeratorHanlder : ISeedEnumeratorHandler, IActionSequenceEnumeratorHandler
    {
        private uint _index;
        private readonly int[] _initialCounterValues;
        private readonly BlinkObject[] _blinkObjects;
        private readonly Func<bool[], bool> _actionSelector;

        public BlinkObjectEnumeratorHanlder(params BlinkObject[] blinkObjects)
        {
            _blinkObjects = blinkObjects;
            _initialCounterValues = blinkObjects.Select(_ => _.Counter).ToArray();
            _actionSelector = (_) => _[0];
        }
        public BlinkObjectEnumeratorHanlder(Func<bool[], bool> actionSelector, params BlinkObject[] blinkObjects)
        {
            _blinkObjects = blinkObjects;
            _initialCounterValues = blinkObjects.Select(_ => _.Counter).ToArray();
            _actionSelector = actionSelector;
        }

        public uint SelectCurrent(uint seed) => seed;

        public uint Advance(uint seed)
        {
            RollForAction(ref seed);
            return seed;
        }

        public bool RollForAction(ref uint seed)
        {
            var actions = new bool[_blinkObjects.Length];
            for (var i = 0; i < _blinkObjects.Length; i++)
                actions[i] = _blinkObjects[i].CountUp(ref seed, ref _index);

            return _actionSelector(actions);
        }

        public uint Initialize(uint seed)
        {
            foreach (var (obj, cnt) in _blinkObjects.Zip(_initialCounterValues, (obj, cnt) => (obj, cnt)))
                obj.Initialize(cnt);

            return seed;
        }

        public static BlinkObjectEnumeratorHanlder ResultScene(BlinkObject pokemon, bool enemyBlinking, int initCounter = 0)
            => enemyBlinking ?
                new BlinkObjectEnumeratorHanlder((actions) => actions[2], new BlinkObject(10, initCounter), new BlinkObject(10, initCounter), pokemon) :
                new BlinkObjectEnumeratorHanlder((actions) => actions[1], new BlinkObject(10, initCounter), pokemon);
    }
}
