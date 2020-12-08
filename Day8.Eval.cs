using System;
using System.Collections.Immutable;

namespace AdventOfCode2020
{
    public partial class Day8
    {
        private static State Step(State state)
        {
            if (state.IP >= state.Instructions.Count)
            {
                return state;
            }

            var instruction = state.Instructions[state.IP];

            return instruction switch
            {
                Op.Nop nop => state.IncIP(1),
                Op.Acc acc => state.IncAcc(acc.Value).IncIP(1),
                Op.Jmp jmp => state.IncIP(jmp.Value),

                _ => throw new ArgumentOutOfRangeException(nameof(instruction))
            };
        }

        private class State
        {
            private State(ImmutableList<Op> instructions, int ip, int acc)
            {
                Instructions = instructions;
                IP = ip;
                Acc = acc;
            }

            public ImmutableList<Op> Instructions { get; }

            public int IP { get; }
            public int Acc { get; }


            public static State InitialFrom(ImmutableList<Op> instructions) => new State(instructions, 0, 0);

            public State IncIP(int delta) => new State(Instructions, IP + delta, Acc);
            public State IncAcc(int delta) => new State(Instructions, IP, Acc + delta);
        }
    }
}