using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonXDRNGLibrary.QuickBattle
{
    public partial class QuickBattleSeedSearcher
    {
        private readonly XDDBClient _client;
        private readonly uint _tsv;
        private State _state = firstState;

        public QuickBattleSeedSearcher(XDDBClient client, uint tsv = 0x10000)
            => (_client, _tsv) = (client, tsv);

        public IEnumerable<uint> Next(in XDQuickBattleArguments arg)
        {
            var (res, next) = _state.Next(arg, _client, _tsv);
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
            public abstract (IEnumerable<uint> Result, State NextState) Next(in XDQuickBattleArguments arg, XDDBClient client, uint tsv);
        }

        class FirstState : State
        {
            public override (IEnumerable<uint> Result, State NextState) Next(in XDQuickBattleArguments arg, XDDBClient client, uint tsv)
                => (Array.Empty<uint>(), new SecondState(arg));
        }
        class SecondState : State
        {
            private readonly XDQuickBattleArguments _first;
            public SecondState(in XDQuickBattleArguments args)
                => _first = args;

            public override (IEnumerable<uint> Result, State NextState) Next(in XDQuickBattleArguments arg, XDDBClient client, uint tsv)
            {
                var res = client.Search(_first, arg);
                return (res, new ExtState(res));
            }
        }
        class ExtState : State
        {
            private readonly IEnumerable<uint> _seeds;
            public ExtState(IEnumerable<uint> seeds)
                => _seeds = seeds;

            public override (IEnumerable<uint> Result, State NextState) Next(in XDQuickBattleArguments arg, XDDBClient client, uint tsv)
            {
                var next = Filter(_seeds, arg, tsv);
                return (next, new ExtState(next));
            }

            private static IEnumerable<uint> Filter(IEnumerable<uint> seeds, XDQuickBattleArguments arg, uint tsv)
            {
                foreach (var seed in seeds)
                {
                    var (p, e, code, s) = seed.GenerateQuickBattle(tsv);
                    if (arg.PlayerTeam != p || arg.EnemyTeam != e || arg.HPCode != code) continue;

                    yield return s;
                }
            }
        }
    }

}
