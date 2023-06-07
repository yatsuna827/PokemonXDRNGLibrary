using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonXDRNGLibrary.XDDB
{
    public partial class QuickBattleSeedSearcher
    {
        private readonly XDDBClient _client;
        private readonly uint _tsv;
        private State _state = firstState;

        public QuickBattleSeedSearcher(XDDBClient client, uint tsv = 0x10000)
            => (_client, _tsv) = (client, tsv);

        public IEnumerable<uint> Next(in QuickBattleInput input)
        {
            var (res, next) = _state.Next(input, _client, _tsv);
            _state = next;

            return res;
        }

        public void Reset()
            => _state = firstState;
    }

    public partial class QuickBattleSeedSearcher
    {
        private static readonly FirstState firstState = new FirstState();

        abstract class State
        {
            public abstract (IEnumerable<uint> Result, State NextState) Next(in QuickBattleInput input, XDDBClient client, uint tsv);
        }

        class FirstState : State
        {
            public override (IEnumerable<uint> Result, State NextState) Next(in QuickBattleInput input, XDDBClient client, uint tsv)
                => (Array.Empty<uint>(), new SecondState(input));
        }
        class SecondState : State
        {
            private readonly QuickBattleInput _first;
            public SecondState(in QuickBattleInput inputs)
                => _first = inputs;

            public override (IEnumerable<uint> Result, State NextState) Next(in QuickBattleInput input, XDDBClient client, uint tsv)
            {
                var res = client.Search(_first, input);
                return (res, new ExtState(res));
            }
        }
        class ExtState : State
        {
            private readonly IEnumerable<uint> _seeds;
            public ExtState(IEnumerable<uint> seeds)
                => _seeds = seeds;

            public override (IEnumerable<uint> Result, State NextState) Next(in QuickBattleInput input, XDDBClient client, uint tsv)
            {
                var next = Filter(_seeds, input, tsv);
                return (next, new ExtState(next));
            }

            private static IEnumerable<uint> Filter(IEnumerable<uint> seeds, QuickBattleInput input, uint tsv)
            {
                foreach (var seed in seeds)
                {
                    var (p, e, code, s) = seed.GenerateQuickBattle(tsv);
                    if (!input.Check(p, e, code)) continue;

                    yield return s;
                }
            }
        }
    }

}
