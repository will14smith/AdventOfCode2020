using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Parse, SolvePart2).Should().Be(1068781);
            Run("sample2", "0\n17,x,13,19\n", Parse, SolvePart2).Should().Be(3417);
            Run("sample3", "0\n67,7,59,61\n", Parse, SolvePart2).Should().Be(754018);
            Run("sample4", "0\n67,x,7,59,61\n", Parse, SolvePart2).Should().Be(779210);
            Run("sample5", "0\n67,7,x,59,61\n", Parse, SolvePart2).Should().Be(1261476);
            Run("sample6", "0\n1789,37,47,1889\n", Parse, SolvePart2).Should().Be(1202161486);
            Run("actual", Parse, SolvePart2);
        }

        private static long SolvePart1((long TimeToWait, IReadOnlyList<long> Buses) input)
        {
            var bestBus = 0L;
            var bestTime = long.MaxValue;

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

        private static long SolvePart2((long _, IReadOnlyList<long> Buses) input)
        {
            var schedule = input.Buses.Select((x, i) => (Bus: x, Offset: (long)i)).Where(x => x.Bus != -1).ToList();

            return SolvePart2(schedule);
        }

        private static long SolvePart2(IReadOnlyList<(long Bus, long Offset)> schedule)
        {
            var (bus0, offset0) = schedule[0];

            for (var index = 1; index < schedule.Count; index++)
            {
                var (busI, offsetI) = schedule[index];

                // bus0 and busI will meet once in the busN period
                var busN = bus0 * busI;

                // calculate how many times bus0 has to visit before we line up
                var offsetN = offset0;
                while (offsetN < busN && (offsetN + offsetI) % busI != 0)
                {
                    offsetN += bus0;
                }

                if (offsetN > busN)
                {
                    throw new Exception("buses never align");
                }

                bus0 = busN;
                offset0 = offsetN;
            }

            return offset0;
        }

        private static (long TimeToWait, IReadOnlyList<long> Buses) Parse(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var timeToWait = long.Parse(lines[0]);
            var buses = lines[1].Split(',').Select(x => x != "x" ? long.Parse(x) : -1).ToList();

            return (timeToWait, buses);
        }
    }
}