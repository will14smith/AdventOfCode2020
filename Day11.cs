using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day11
    {
        private static readonly string Sample = "L.LL.LL.LL\nLLLLLLL.LL\nL.L.L..L..\nLLLL.LL.LL\nL.LL.LL.LL\nL.LLLLL.LL\n..L.L.....\nLLLLLLLLLL\nL.LLLLLL.L\nL.LLLLL.LL\n";

        private static readonly TextParser<State> Seat = Character.In('L','.').Select(x => x == 'L' ? State.Empty : State.NoSeat);
        private static readonly TextParser<State[]> Row = Seat.AtLeastOnce().ThenIgnore(SuperpowerExtensions.NewLine);
        private static readonly TextParser<State[][]> Room = Row.AtLeastOnce();

        private readonly ITestOutputHelper _output;

        public Day11(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day11");

            _output.Run("sample1", () => SolvePart1(Sample))
                .Should().Be(37);

            _output.Run("actual", () => SolvePart1(data));
        }

        private long SolvePart1(string input)
        {
            var room = Room.MustParse(input);

            var changed = true;
            var i = 0;
            while (changed)
            {
                (room, changed) = Next(room);
            }

            return room.SelectMany(x => x).Count(x => x == State.Occupied);
        }

        private (State[][] Next, bool Changed) Next(State[][] input)
        {
            var changed = false;
            var next = new State[input.Length][];

            for (var r = 0; r < input.Length; r++)
            {
                next[r] = new State[input[r].Length];

                for (var c = 0; c < input[r].Length; c++)
                {
                    next[r][c] = Next(input, r, c);
                    changed |= input[r][c] != next[r][c];
                }
            }

            return (next, changed);
        }

        private static State Next(in State[][] input, in int r, in int c)
        {
            if (input[r][c] == State.NoSeat) return State.NoSeat;

            var adj = new[]
            {
                // TL, T, TR, L, R, BL, B, BL
                (r-1, c-1),(r-1, c), (r-1, c+1),
                (r, c-1),(r, c+1),
                (r+1, c-1),(r+1, c), (r+1, c+1),
            };

            var occ = 0;
            foreach (var (rAdj, cAdj) in adj)
            {
                if (rAdj >= 0 && rAdj < input.Length && cAdj >= 0 && cAdj < input[rAdj].Length)
                {
                    occ += input[rAdj][cAdj] == State.Occupied ? 1 : 0;
                }
            }

            if (occ == 0) return State.Occupied;
            if (occ >= 4) return State.Empty;
            return input[r][c];
        }

        private enum State
        {
            NoSeat,
            Empty,
            Occupied
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}