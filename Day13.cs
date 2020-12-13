using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day13 : Test
    {
        private static readonly string Sample = "939\n7,13,x,x,59,x,31,19\n";

        public Day13(ITestOutputHelper output) : base(13, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Parse, SolvePart1).Should().Be(295);
            Run("actual", Parse, SolvePart1);
        }

        private static long SolvePart1((int TimeToWait, IReadOnlyCollection<int> Buses) input)
        {
            var bestBus = 0;
            var bestTime = int.MaxValue;

            foreach (var bus in input.Buses.Where(x => x != -1))
            {
                var timeForBus = bus - input.TimeToWait % bus;
                if (timeForBus < bestTime)
                {
                    bestBus = bus;
                    bestTime = timeForBus;
                }
            }

            return bestBus * bestTime;
        }

        private static (int TimeToWait, IReadOnlyCollection<int> Buses) Parse(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var timeToWait = int.Parse(lines[0]);
            var buses = lines[1].Split(',').Select(x => x != "x" ? int.Parse(x) : -1).ToList();

            return (timeToWait, buses);
        }
    }
}