﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2020
{
    public partial class Day12
    {
        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Tokenizer, Instructions, SolvePart2).Should().Be(286);
            Run("actual", Tokenizer, Instructions, SolvePart2);
        }

        private static int SolvePart2(Op[] instructions)
        {
            var state = instructions.Aggregate(StatePart2.Initial, Apply);

            return Math.Abs(state.Ship.X) + Math.Abs(state.Ship.Y);
        }

        private static StatePart2 Apply(StatePart2 state, Op instruction)
        {
            return instruction.Type switch
            {
                'F' => state.MoveShipToWaypoint(instruction.Value),

                'L' => state.RotateWaypoint(-instruction.Value),
                'R' => state.RotateWaypoint(instruction.Value),

                'N' => state.IncWaypointY(instruction.Value),
                'E' => state.IncWaypointX(instruction.Value),
                'S' => state.IncWaypointY(-instruction.Value),
                'W' => state.IncWaypointX(-instruction.Value),

                _ => throw new ArgumentOutOfRangeException($"instruction: {instruction.Type}")
            };
        }

        private class StatePart2
        {
            public static readonly StatePart2 Initial = new StatePart2((0, 0), (10, 1));

            private static readonly IReadOnlyDictionary<int, (int, int)> Trig = new Dictionary<int, (int Cos, int Sin)>
            {
                { 0, (1, 0) },
                { 90, (0, -1) },
                { 180, (-1, 0) },
                { 270, (0, 1) },
            };

            private StatePart2((int X, int Y) ship, (int X, int Y) waypoint)
            {
                Ship = ship;
                Waypoint = waypoint;
            }

            public (int X, int Y) Ship { get; }
            public (int X, int Y) Waypoint { get; }

            public StatePart2 IncWaypointX(in int delta) => new StatePart2(Ship, (Waypoint.X + delta, Waypoint.Y));
            public StatePart2 IncWaypointY(in int delta) => new StatePart2(Ship, (Waypoint.X, Waypoint.Y + delta));

            public StatePart2 RotateWaypoint(int delta)
            {
                var (cosDelta, sinDelta) = Trig[StatePart1.NormaliseHeading(delta)];

                return new StatePart2(Ship, (Waypoint.X * cosDelta - Waypoint.Y * sinDelta, Waypoint.X * sinDelta + Waypoint.Y * cosDelta));
            }

            public StatePart2 MoveShipToWaypoint(in int count) => new StatePart2((Ship.X + Waypoint.X * count, Ship.Y + Waypoint.Y * count), Waypoint);
        }
    }
}