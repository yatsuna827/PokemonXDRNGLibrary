namespace PokemonXDRNGLibrary.XDDB
{
    internal sealed partial class RecSplitMph
    {
        private readonly int _n;
        private readonly int _numBuckets;
        private readonly int[] _bucketSize;
        private readonly int[] _keyOffset;
        private readonly long[] _bitOffset;
        private readonly byte[] _bits;

        public int Count { get { return _n; } }

        public RecSplitMph(int n, int[] bucketSize, long[] bitOffset, byte[] bits)
        {
            _n = n;
            _numBuckets = bucketSize.Length;
            _bucketSize = bucketSize;
            _keyOffset = new int[_numBuckets + 1];
            for (int b = 0; b < _numBuckets; b++)
                _keyOffset[b + 1] = _keyOffset[b] + _bucketSize[b];
            _bitOffset = bitOffset;
            _bits = bits;
        }

        public int Lookup(uint code)
        {
            int b = (int)(Mix(code, BucketSeed) % (uint)_numBuckets);
            int size = _bucketSize[b];
            if (size == 0) return 0;

            var rd = new BitReader(_bits, _bitOffset[b]);
            int local = EvalNode(rd, size, code);
            int slot = _keyOffset[b] + local;
            if (slot < 0 || slot >= _n) return 0;
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
    }
}
