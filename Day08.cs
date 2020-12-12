using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day08 : Test
    {
        private const string Sample = @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6";

        public Day08(ITestOutputHelper output) : base(8, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Instructions, x => EvaluateUntilFirstRepeat(x).Acc).Should().Be(5);
            Run("actual", Tokenizer, Instructions, x => EvaluateUntilFirstRepeat(x).Acc);
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Tokenizer, Instructions, FixAndEvaluate).Should().Be((8, 7));
            Run("actual", Tokenizer, Instructions, FixAndEvaluate);
        }

        private static State EvaluateUntilFirstRepeat(ImmutableList<Op> instructions)
        {
            var state = State.InitialFrom(instructions);
            var nextState = state;

            var visited = new HashSet<int>();
            do
            {
                state = nextState;
                nextState = Step(state);
            } while (visited.Add(state.IP));

            return state;
        }

        private static (int FinalAcc, int BrokenIP) FixAndEvaluate(ImmutableList<Op> instructions)
        {
            for (var i = 0; i < instructions.Count; i++)
            {
                var state = TryFixAndEvaluate(instructions, i);
                if (IsComplete(state))
                {
                    return (state.Acc, i);
                }
            }

            throw new Exception("Failed to fix it");

            static bool IsComplete(State state) => state.IP >= state.Instructions.Count;
        }

        private static State TryFixAndEvaluate(ImmutableList<Op> instructions, int ip)
        {
            var op = instructions[ip] switch
            {
                Op.Nop nop => (Op) new Op.Jmp(nop.Value),
                Op.Acc acc => acc,
                Op.Jmp jmp => new Op.Nop(jmp.Value),

                _ => throw new ArgumentOutOfRangeException()
            };

            return EvaluateUntilFirstRepeat(instructions.SetItem(ip, op));
        }
    }
}