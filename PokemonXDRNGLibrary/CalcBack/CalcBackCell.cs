using System;
using System.Collections.Generic;
using System.Linq;
using PokemonPRNG.LCG32.GCLCG;

namespace PokemonXDRNGLibrary
{
    class CalcBackCell
    {
        public uint Seed { get; }
        public uint ConditionedTSV { get; }

        public CalcBackCell(uint seed) 
        { 
            Seed = seed; 
            ConditionedTSV = 0x10000;
        }
        public CalcBackCell(uint seed, uint tsv)
        {
            Seed = seed;
            ConditionedTSV = tsv == 0x10000 ? tsv : tsv & 0xFFF8;
        }

    }

    class PregenerateNode
    {
        private readonly PregenerateNode _parent;
        private readonly List<PregenerateNode> _children = new List<PregenerateNode>();

        private bool _hasPath;
        private bool _pidSkipped;

        public uint GeneratedPSV { get; }

        public PregenerateNode CreateChild(uint seed, bool pidSkipped)
        {
            var child = pidSkipped
                ? new PregenerateNode(this)
                : new PregenerateNode(seed, this);

            _children.Add(child);

            return child;
        }
        public void Feedback()
        {
            _hasPath = true;
            _parent?.Feedback();
        }

        public void Print()
        {
            Console.WriteLine("DarkPokemon");
            foreach (var c in _children)
            {
                c.Print(" ");
            }
        }
        private void Print(string tab)
        {
            var mark = _hasPath ? "☆" : "";
            Console.WriteLine($"{tab}{GeneratedPSV}{mark}");

            foreach (var c in _children)
            {
                c.Print(tab + " ");
            }
        }

        // 固定済みダークポケモンのPIDは無視する
        public List<uint> GetContraindicatedTSVs(int depth)
        {
            var result = new List<uint>();

            var tower = new List<PregenerateNode>[depth];
            for (int i = 0; i < depth; i++)
                tower[i] = new List<PregenerateNode>();

            var queue = new Queue<(int Depth, PregenerateNode Node)>();
            foreach (var c in _children)
                queue.Enqueue((0, c));

            while (queue.Count > 0)
            {
                var (d, node) = queue.Dequeue();
                if (!node._pidSkipped)
                    tower[d].Add(node);

                foreach (var c in node._children)
                    queue.Enqueue((d + 1, c));
            }

            foreach (var floor in tower)
                if (floor.Count(_ => _._hasPath) == 1 && floor[0]._hasPath)
                    result.Add(floor[0].GeneratedPSV);

            return result;
        }

        public bool CheckTSV(uint tsv)
        {
            if ((tsv ^ GeneratedPSV) < 8) return false;

            if (_parent != null) return _parent.CheckTSV(tsv);

            return true;
        }

        public PregenerateNode() { }
        public PregenerateNode(PregenerateNode parent)
        {
            _parent = parent;

            _pidSkipped = true;
        }
        public PregenerateNode(uint seed, PregenerateNode parent)
        {
            _parent = parent;
 
            GeneratedPSV = ((seed >> 19) ^ (seed.Back() >> 19)) << 3;
        }
    }
}
