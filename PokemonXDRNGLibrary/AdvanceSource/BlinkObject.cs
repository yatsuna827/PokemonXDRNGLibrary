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
        public bool CountUp(ref uint seed)
        {
            if (_remainCoolTime-- > 0) return false; // マイナスに振り切るぶんには問題ないので.
            if (_blinkCounter++ < 10) return false;

            var rand = seed.GetRand();
            if (_blinkCounter < 180 && rand >= BlinkConst.blinkThresholds[_blinkCounter - 10]) return false;
            // 実際は停滞によってカウンタが満期までが延びているので、その分を遅らせる
            if (180 <= _blinkCounter && _blinkCounter < 180 + _delayAtMaturity) return false;

            _blinkCounter = 0;
            _remainCoolTime = _coolTime;

            return true;
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
        private readonly int _initialCounterValue;
        private readonly BlinkObject _blinkObject;

        public BlinkObjectEnumeratorHanlder(BlinkObject blinkObjects)
        {
            _blinkObject = blinkObjects;
            _initialCounterValue = blinkObjects.Counter;
        }
       
        public uint SelectCurrent(uint seed) => seed;

        public uint Advance(uint seed)
        {
            RollForAction(ref seed);
            return seed;
        }

        public bool RollForAction(ref uint seed)
        {
            return _blinkObject.CountUp(ref seed, ref _index);
        }

        public uint Initialize(uint seed)
        {
            _blinkObject.Initialize(_initialCounterValue);
            return seed;
        }
    }
}
