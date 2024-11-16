using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonXDRNGLibrary.AdvanceSource
{
    class CountDownValue
    {
        private readonly int value;
        private CountDownValue(int value) => this.value = value;

        public bool IsZero => value == 0;
        public CountDownValue CountDown() => values[this.value - 1];

        private static readonly CountDownValue[] values = Enumerable.Range(0, 200).Select(_ => new CountDownValue(_)).ToArray();
        public static CountDownValue CreateValue(int i) => values[i];
    }
}
