using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day01 : Test
    {
        private static readonly string Sample = "1721\n979\n366\n299\n675\n1456";

        public Day01(ITestOutputHelper output) : base(1, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Parse, SolvePart1).Should().Be(514579);
            Run("actual", Parse, SolvePart1);
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Parse, SolvePart2).Should().Be(241861950);
            Run("actual", Parse, SolvePart2);
        }

        private static int SolvePart1(IEnumerable<int> data)
        {
            var set = data.ToHashSet();

            foreach (var item in set)
            {
                var pair = 2020 - item;
                if (set.Contains(pair))
                {
                    return item * pair;
                }
            }

            throw new Exception("Failed to find a matching pair");
        }

        private static int SolvePart2(IEnumerable<int> data)
        {
            var set = data.ToHashSet();

            foreach (var item1 in set)
            foreach (var item2 in set)
            {
                var item3 = 2020 - item1 - item2;
                if (set.Contains(item3))
                {
                    return item1 * item2 * item3;
                }
            }

            throw new Exception("Failed to find a matching pair");
        }

        private static IEnumerable<int> Parse(string input) => input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
    }
}