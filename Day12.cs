using System;
using System.IO;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day12
    {
        private static readonly string Sample = "F10\nN3\nF7\nR90\nF11\n";

        private readonly ITestOutputHelper _output;

        public Day12(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day12");

            _output.Run("sample", () => SolvePart1(Sample))
                .Should().Be(25);

            _output.Run("actual", () => SolvePart1(data));
        }

        private int SolvePart1(string input)
        {
            var instructions = Instructions.MustParse(Tokenizer, input);

            var state = StatePart1.Initial;
            foreach (var instruction in instructions)
            {
                state = Apply(state, instruction);
            }

            return Math.Abs(state.X) + Math.Abs(state.Y);
        }

        private StatePart1 Apply(StatePart1 state, Op instruction)
        {
            return instruction.Type switch
            {
                'F' => state.Heading switch
                {
                    0 => state.IncY(instruction.Value),
                    90 => state.IncX(instruction.Value),
                    180 => state.IncY(-instruction.Value),
                    270 => state.IncX(-instruction.Value),

                    _ => throw new ArgumentOutOfRangeException($"heading: {state.Heading}")
                },

                'L' => state.Rotate(-instruction.Value),
                'R' => state.Rotate(instruction.Value),

                'N' => state.IncY(instruction.Value),
                'E' => state.IncX(instruction.Value),
                'S' => state.IncY(-instruction.Value),
                'W' => state.IncX(-instruction.Value),

                _ => throw new ArgumentOutOfRangeException($"instruction: {instruction.Type}")
            };
        }

        private class StatePart1
        {
            public static readonly StatePart1 Initial = new StatePart1(0, 0, 90);

            private StatePart1(int x, int y, int heading)
            {
                X = x;
                Y = y;
                Heading = heading;
            }

            public int X { get; }
            public int Y { get; }
            public int Heading { get; }

            public StatePart1 IncX(in int delta) => new StatePart1(X + delta, Y, Heading);
            public StatePart1 IncY(in int delta) => new StatePart1(X, Y + delta, Heading);
            public StatePart1 Rotate(int delta) => new StatePart1(X, Y, NormaliseHeading(Heading + delta));

            internal static int NormaliseHeading(int heading) => heading < 0 ? heading % 360 + 360 : heading % 360;
        }


        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}