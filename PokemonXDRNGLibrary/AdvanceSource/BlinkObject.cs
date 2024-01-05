using System;
using System.Linq;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary.AdvanceSource
{
    public class BlinkObject
    {
        private readonly int _coolTime;
        private readonly int _delayAtMaturity;
        private int _remainCoolTime = 0;
        private int _blinkCounter;
        public int Counter { get => _blinkCounter; }

        public void Initialize(int initialCounter)
        {
            _blinkCounter = initialCounter; 
            _remainCoolTime = 0;
        }
        public bool CountUp(ref uint seed, ref uint index)
        {
            if (_remainCoolTime-- > 0) return false; // マイナスに振り切るぶんには問題ないので.
            if (_blinkCounter++ < 10) return false;

            index++;
            var rand = seed.GetRand();
            if (_blinkCounter < 180 && rand >= BlinkConst.blinkThresholds[_blinkCounter - 10]) return false;
            // 実際は停滞によってカウンタが満期までが延びているので、その分を遅らせる
            if (180 <= _blinkCounter && _blinkCounter < 180 + _delayAtMaturity) return false;

            _blinkCounter = 0;
            _remainCoolTime = _coolTime;

            return true;
        }
        public BlinkObject(int cool, int initCounter = 0, int delayAtMaturity = 0)
        {
            _coolTime = cool;
            _delayAtMaturity = delayAtMaturity;
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
    }
}
