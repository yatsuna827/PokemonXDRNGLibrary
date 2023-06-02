using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonXDRNGLibrary
{
    static class BinarySearch
    {
        // 参考: https://qiita.com/hamko/items/794a92c456164dcc04ad
        // item < list[i]となる最小のiを返す.
        internal static int UpperBound(this uint[] array, uint item)
        {
            var ng = -1;
            var ok = array.Length;

            while (ok - ng > 1)
            {
                var mid = (ng + ok) / 2;
                if (item < array[mid]) ok = mid; else ng = mid;
            }

            return ok == array.Length ? -1 : ok;
        }

        // item <= list[i]となる最小のiを返す.
        internal static int LowerBound(this uint[] array, uint item)
        {
            var ng = -1;
            var ok = array.Length;

            while (ok - ng > 1)
            {
                var mid = (ng + ok) / 2;
                if (item <= array[mid]) ok = mid; else ng = mid;
            }

            return ok == array.Length ? -1 : ok;
        }
    }
}
