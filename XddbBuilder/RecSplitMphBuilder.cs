using System;
using System.Collections.Generic;

namespace XddbBuilder
{
    internal static class RecSplitMphBuilder
    {
        internal const int LeafSize = 8;
        internal const ulong BucketSeed = 0x00C0FFEEUL;
        internal const int AvgBucketSize = 1024;

        internal static ulong Mix(uint key, ulong seed)
        {
            ulong z = (ulong)key * 0xFF51AFD7ED558CCDUL + seed + 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 33)) * 0xFF51AFD7ED558CCDUL;
            z = (z ^ (z >> 33)) * 0xC4CEB9FE1A85EC53UL;
            return z ^ (z >> 33);
        }

        private static readonly Dictionary<int, int> _leafKCache = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> _splitKCache = new Dictionary<int, int>();

        private static double LnFactorial(int x)
        {
            double s = 0;
            for (int i = 2; i <= x; i++) s += Math.Log(i);
            return s;
        }

        internal static int LeafRiceK(int m)
        {
            if (_leafKCache.TryGetValue(m, out int k)) return k;
            if (m <= 1) { _leafKCache[m] = 0; return 0; }
            double mu = 1;
            for (int i = 1; i <= m; i++) mu *= (double)m / i;
            k = (int)Math.Floor(Math.Log(mu, 2));
            if (k < 0) k = 0;
            _leafKCache[m] = k;
            return k;
        }

        internal static int SplitRiceK(int m)
        {
            if (_splitKCache.TryGetValue(m, out int k)) return k;

            int left = (m + 1) / 2, right = m - left;
            double lnP = LnFactorial(m) - LnFactorial(left) - LnFactorial(right)
                       + left * Math.Log((double)left / m) + right * Math.Log((double)right / m);
            double mu = Math.Exp(-lnP);
            k = (int)Math.Floor(Math.Log(mu, 2));
            if (k < 0) k = 0;
            _splitKCache[m] = k;
            return k;
        }

        // ---- Build ----

        public readonly struct Result
        {
            public readonly int N;
            public readonly int[] BucketSize;
            public readonly long[] BitOffset;
            public readonly byte[] Bits;

            public Result(int n, int[] bucketSize, long[] bitOffset, byte[] bits)
            {
                N = n;
                BucketSize = bucketSize;
                BitOffset = bitOffset;
                Bits = bits;
            }
        }

        public static Result Build(uint[] codes)
        {
            int n = codes.Length;
            int numBuckets = Math.Max(1, n / AvgBucketSize);

            var buckets = new List<uint>[numBuckets];
            for (int b = 0; b < numBuckets; b++) buckets[b] = new List<uint>();
            foreach (var c in codes)
                buckets[(int)(Mix(c, BucketSeed) % (uint)numBuckets)].Add(c);

            var writer = new BitWriter();
            var sizes = new int[numBuckets];
            var bitlens = new long[numBuckets];
            for (int b = 0; b < numBuckets; b++)
            {
                sizes[b] = buckets[b].Count;
                long start = writer.BitLength;
                BuildNode(writer, buckets[b].ToArray(), sizes[b]);
                bitlens[b] = writer.BitLength - start;
            }

            var bitOffsets = new long[numBuckets + 1];
            for (int b = 0; b < numBuckets; b++)
                bitOffsets[b + 1] = bitOffsets[b] + bitlens[b];

            return new Result(n, sizes, bitOffsets, writer.ToArray());
        }

        private static void BuildNode(BitWriter writer, uint[] keys, int size)
        {
            if (size <= LeafSize)
            {
                int k = LeafRiceK(size);
                for (uint seed = 0; ; seed++)
                {
                    int used = 0;
                    bool ok = true;
                    for (int i = 0; i < size; i++)
                    {
                        int pos = (int)(Mix(keys[i], seed) % (uint)size);
                        int bit = 1 << pos;
                        if ((used & bit) != 0) { ok = false; break; }
                        used |= bit;
                    }
                    if (ok) { writer.WriteRice(seed, k); return; }
                }
            }

            int left = (size + 1) / 2, right = size - left;
            int sk = SplitRiceK(size);
            for (uint seed = 0; ; seed++)
            {
                int cnt = 0;
                for (int i = 0; i < size; i++)
                    if (Mix(keys[i], seed) % (uint)size < (uint)left) cnt++;
                if (cnt != left) continue;

                writer.WriteRice(seed, sk);
                var L = new uint[left];
                var R = new uint[right];
                int li = 0, ri = 0;
                for (int i = 0; i < size; i++)
                {
                    if (Mix(keys[i], seed) % (uint)size < (uint)left) L[li++] = keys[i];
                    else R[ri++] = keys[i];
                }
                BuildNode(writer, L, left);
                BuildNode(writer, R, right);
                return;
            }
        }

        // ---- Lookup (検証用) ----

        public static int Lookup(Result result, uint code)
        {
            int numBuckets = result.BucketSize.Length;
            int b = (int)(Mix(code, BucketSeed) % (uint)numBuckets);
            int size = result.BucketSize[b];
            if (size == 0) return 0;

            int keyOffset = 0;
            for (int i = 0; i < b; i++) keyOffset += result.BucketSize[i];

            var rd = new BitReader(result.Bits, result.BitOffset[b]);
            int local = EvalNode(rd, size, code);
            int slot = keyOffset + local;
            if (slot < 0 || slot >= result.N) return 0;
            return slot;
        }

        private static int EvalNode(BitReader rd, int size, uint key)
        {
            if (size <= LeafSize)
            {
                uint seed = rd.ReadRice(LeafRiceK(size));
                return (int)(Mix(key, seed) % (uint)size);
            }

            int left = (size + 1) / 2, right = size - left;
            uint splitSeed = rd.ReadRice(SplitRiceK(size));
            if (Mix(key, splitSeed) % (uint)size < (uint)left)
                return EvalNode(rd, left, key);

            SkipNode(rd, left);
            return left + EvalNode(rd, right, key);
        }

        private static void SkipNode(BitReader rd, int size)
        {
            if (size <= LeafSize)
            {
                rd.ReadRice(LeafRiceK(size));
                return;
            }
            int left = (size + 1) / 2, right = size - left;
            rd.ReadRice(SplitRiceK(size));
            SkipNode(rd, left);
            SkipNode(rd, right);
        }

        // ---- Bit I/O ----

        private sealed class BitWriter
        {
            private readonly List<byte> _bytes = new List<byte>();
            private int _cur, _nbits;

            public long BitLength { get { return (long)_bytes.Count * 8 + _nbits; } }

            public void WriteBit(int b)
            {
                _cur = (_cur << 1) | (b & 1);
                if (++_nbits == 8) { _bytes.Add((byte)_cur); _cur = 0; _nbits = 0; }
            }

            public void WriteRice(uint v, int k)
            {
                uint q = v >> k;
                for (uint i = 0; i < q; i++) WriteBit(1);
                WriteBit(0);
                for (int i = k - 1; i >= 0; i--) WriteBit((int)((v >> i) & 1));
            }

            public byte[] ToArray()
            {
                var arr = new byte[_bytes.Count + (_nbits > 0 ? 1 : 0)];
                _bytes.CopyTo(arr, 0);
                if (_nbits > 0) arr[_bytes.Count] = (byte)(_cur << (8 - _nbits));
                return arr;
            }
        }

        internal sealed class BitReader
        {
            private readonly byte[] _b;
            private long _pos;

            public BitReader(byte[] b, long startBit) { _b = b; _pos = startBit; }

            public int ReadBit()
            {
                int byteIdx = (int)(_pos >> 3);
                int bit = 7 - (int)(_pos & 7);
                _pos++;
                return (_b[byteIdx] >> bit) & 1;
            }

            public uint ReadRice(int k)
            {
                uint q = 0;
                while (ReadBit() == 1) q++;
                uint r = 0;
                for (int i = 0; i < k; i++) r = (r << 1) | (uint)ReadBit();
                return (q << k) | r;
            }
        }

        // ---- ソースコード生成 ----

        public static string GenerateSharedSource()
        {
            return @"    internal sealed partial class RecSplitMph
    {
        private const int LeafSize = 8;
        private const ulong BucketSeed = 0x00C0FFEEUL;

        internal static ulong Mix(uint key, ulong seed)
        {
            ulong z = (ulong)key * 0xFF51AFD7ED558CCDUL + seed + 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 33)) * 0xFF51AFD7ED558CCDUL;
            z = (z ^ (z >> 33)) * 0xC4CEB9FE1A85EC53UL;
            return z ^ (z >> 33);
        }

        private static readonly Dictionary<int, int> _leafKCache = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> _splitKCache = new Dictionary<int, int>();

        private static double LnFactorial(int x)
        {
            double s = 0;
            for (int i = 2; i <= x; i++) s += Math.Log(i);
            return s;
        }

        internal static int LeafRiceK(int m)
        {
            if (_leafKCache.TryGetValue(m, out int k)) return k;
            if (m <= 1) { _leafKCache[m] = 0; return 0; }
            double mu = 1;
            for (int i = 1; i <= m; i++) mu *= (double)m / i;
            k = (int)Math.Floor(Math.Log(mu, 2));
            if (k < 0) k = 0;
            _leafKCache[m] = k;
            return k;
        }

        internal static int SplitRiceK(int m)
        {
            if (_splitKCache.TryGetValue(m, out int k)) return k;

            int left = (m + 1) / 2, right = m - left;
            double lnP = LnFactorial(m) - LnFactorial(left) - LnFactorial(right)
                       + left * Math.Log((double)left / m) + right * Math.Log((double)right / m);
            double mu = Math.Exp(-lnP);
            k = (int)Math.Floor(Math.Log(mu, 2));
            if (k < 0) k = 0;
            _splitKCache[m] = k;
            return k;
        }

        private sealed class BitReader
        {
            private readonly byte[] _b;
            private long _pos;

            public BitReader(byte[] b, long startBit) { _b = b; _pos = startBit; }

            public int ReadBit()
            {
                int byteIdx = (int)(_pos >> 3);
                int bit = 7 - (int)(_pos & 7);
                _pos++;
                return (_b[byteIdx] >> bit) & 1;
            }

            public uint ReadRice(int k)
            {
                uint q = 0;
                while (ReadBit() == 1) q++;
                uint r = 0;
                for (int i = 0; i < k; i++) r = (r << 1) | (uint)ReadBit();
                return (q << k) | r;
            }
        }
    }
";
        }
    }
}
