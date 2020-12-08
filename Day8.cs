using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day8
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

        private readonly ITestOutputHelper _output;

        public Day8(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day8");

            _output.Run("sample", () => EvaluateUntilFirstRepeat(Sample).Acc)
                .Should().Be(5);

            _output.Run("actual", () => EvaluateUntilFirstRepeat(data).Acc);
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day8");

            _output.Run("sample", () => FixAndEvaluate(Sample))
                .Should().Be((8, 7));

            _output.Run("actual", () => FixAndEvaluate(data));
        }

        private static State EvaluateUntilFirstRepeat(string input)
        {
            var instructions = Instructions.MustParse(Tokenizer, input).ToImmutableList();
            return EvaluateUntilFirstRepeat(instructions);
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

        private static (int FinalAcc, int BrokenIP) FixAndEvaluate(string input)
        {
            var instructions = Instructions.MustParse(Tokenizer, input).ToImmutableList();

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

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}