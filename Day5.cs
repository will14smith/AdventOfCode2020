using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day5
    {
        private static readonly string Sample = "BFFFBBFRRR\nFFFBBBFRRR\nBBFFBBFRLL";

        private static readonly TextParser<int> RowChar = Character.In('B','F').Select(x => x == 'B' ? 1 : 0);
        private static readonly TextParser<int> ColChar = Character.In('L','R').Select(x => x == 'R' ? 1 : 0);
        private static readonly TextParser<int> Row = RowChar.Repeat(7).Select(x => x.Aggregate(0, (a, x) => (a << 1) | x));
        private static readonly TextParser<int> Col = ColChar.Repeat(3).Select(x => x.Aggregate(0, (a, x) => (a << 1) | x));
        private static readonly TextParser<(int Row, int Col)> Seat = Row.Then(r => Col.Select(c => (r, c)));
        private static readonly TextParser<(int Row, int Col)[]> Seats = Seat.ManyDelimitedBy(SuperpowerExtensions.NewLine);

        private readonly ITestOutputHelper _output;

        public Day5(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day5");

            _output.Run("sample", () => GetLargestSeatId(Sample))
                .Should().Be(820);

            _output.Run("actual", () => GetLargestSeatId(data));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day5");

            _output.Run("actual", () => FindMySeatId(data));
        }

        private static int GetLargestSeatId(string input)
        {
            var seats = Seats.MustParse(input);
            return seats.Max(GetSeatId);
        }

        private static int FindMySeatId(string input)
        {
            var seats = Seats.MustParse(input).Select(GetSeatId).ToHashSet();

            var minSeatId = seats.Min();
            var maxSeatId = seats.Max();

            for (var seatId = minSeatId; seatId <= maxSeatId; seatId++)
            {
                if (seats.Contains(seatId))
                {
                    continue;
                }

                if (seats.Contains(seatId - 1) && seats.Contains(seatId + 1))
                {
                    return seatId;
                }
            }

            throw new Exception("Couldn't find seat");
        }

        private static int GetSeatId((int Row, int Col) pos) => pos.Row * 8 + pos.Col;

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}