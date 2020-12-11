using System;
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

        private static readonly IReadOnlyCollection<(int DeltaRow, int DeltaCol)> AdjacencyVectors = new[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

        private readonly ITestOutputHelper _output;

        public Day11(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day11");

            _output.Run("sample", () => SolvePart1(Sample))
                .Should().Be(37);

            _output.Run("actual", () => SolvePart1(data));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day11");

            _output.Run("sample", () => SolvePart2(Sample))
                .Should().Be(26);

            _output.Run("actual", () => SolvePart2(data));
        }

        private long SolvePart1(string input)
        {
            var seats = Room.MustParse(input);
            var room = new RoomState(seats, CalculateAdj(seats, CalculateAdjPart1), 4);

            return CountStableOccupiedSeats(room);
        }

        private static (int, int)[] CalculateAdjPart1(State[][] _, int r, int c)
        {
            return AdjacencyVectors.Select(x => (r + x.DeltaRow, c + x.DeltaCol)).ToArray();
        }

        private long SolvePart2(string input)
        {
            var seats = Room.MustParse(input);
            var room = new RoomState(seats, CalculateAdj(seats, CalculateAdjPart2), 5);

            return CountStableOccupiedSeats(room);
        }

        private static List<(int R, int C)> CalculateAdjPart2(State[][] seats, int r, int c)
        {
            var cellAdj = new List<(int R, int C)>();

            foreach (var (deltaRow, deltaCol) in AdjacencyVectors)
            {
                TryFindAdjSeat2(seats, r, c, deltaRow, deltaCol, cellAdj);
            }

            return cellAdj;
        }

        private static void TryFindAdjSeat2(State[][] seats, int r, int c, int dr, int dc, List<(int R, int C)> res)
        {
            r += dr;
            c += dc;

            while (r >= 0 && r < seats.Length && c >= 0 && c < seats[r].Length)
            {
                if (seats[r][c] != State.NoSeat)
                {
                    res.Add((r, c));
                    break;
                }

                r += dr;
                c += dc;
            }
        }


        private IReadOnlyDictionary<(int R, int C), IReadOnlyCollection<(int R, int C)>> CalculateAdj(State[][] seats, Func<State[][], int, int, IReadOnlyCollection<(int R, int C)>> calculateAdj)
        {
            var adj = new Dictionary<(int R, int C), IReadOnlyCollection<(int R, int C)>>();

            for (var r = 0; r < seats.Length; r++)
            {
                for (var c = 0; c < seats[r].Length; c++)
                {
                    adj.Add((r, c), calculateAdj(seats, r, c));
                }
            }

            return adj;
        }

        private static long CountStableOccupiedSeats(RoomState room)
        {
            var changed = true;
            while (changed)
            {
                (room, changed) = Next(room);
            }

            return room.Seats.SelectMany(x => x).Count(x => x == State.Occupied);
        }

        private static (RoomState Next, bool Changed) Next(RoomState input)
        {
            var changed = false;
            var next = new State[input.Seats.Length][];

            for (var r = 0; r < input.Seats.Length; r++)
            {
                next[r] = new State[input.Seats[r].Length];

                for (var c = 0; c < input.Seats[r].Length; c++)
                {
                    next[r][c] = Next(input, r, c);
                    changed |= input.Seats[r][c] != next[r][c];
                }
            }

            return (input.WithSeats(next), changed);
        }

        private static State Next(in RoomState input, in int r, in int c)
        {
            var seats = input.Seats;
            if (seats[r][c] == State.NoSeat) return State.NoSeat;

            var occ = 0;
            foreach (var (rAdj, cAdj) in input.Adjacent[(r, c)])
            {
                if (rAdj >= 0 && rAdj < seats.Length && cAdj >= 0 && cAdj < seats[rAdj].Length)
                {
                    occ += seats[rAdj][cAdj] == State.Occupied ? 1 : 0;
                }
            }

            if (occ == 0) return State.Occupied;
            if (occ >= input.Tolerance) return State.Empty;
            return seats[r][c];
        }

        private class RoomState
        {
            public RoomState(State[][] seats, IReadOnlyDictionary<(int R, int C), IReadOnlyCollection<(int R, int C)>> adjacent, int tolerance)
            {
                Seats = seats;
                Adjacent = adjacent;
                Tolerance = tolerance;
            }

            public State[][] Seats { get; }
            public IReadOnlyDictionary<(int R, int C), IReadOnlyCollection<(int R, int C)>> Adjacent { get; }
            public int Tolerance { get; }

            public RoomState WithSeats(State[][] seats)
            {
                return new RoomState(seats, Adjacent, Tolerance);
            }
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