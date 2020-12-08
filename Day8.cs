using System.Collections.Generic;
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

        private static State EvaluateUntilFirstRepeat(string input)
        {
            var instructions = Instructions.MustParse(Tokenizer, input);
            var state = State.InitialFrom(instructions);

            var visited = new HashSet<int>();
            while (visited.Add(state.IP))
            {
                state = Step(state);
            }

            return state;
        }



        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}