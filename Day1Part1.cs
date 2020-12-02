using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day1Part1
    {
        private static readonly IEnumerable<int> Sample = new[] { 1721, 979, 366, 299, 675, 1456 };

        private readonly ITestOutputHelper _output;

        public Day1Part1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Main()
        {
            var data = LoadData("day1part1");

            _output.Run("sample", () => Calculate(Sample))
                .Should().Be(514579);

            _output.Run("actual", () => Calculate(data));
        }

        private static int Calculate(IEnumerable<int> data)
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

        private static IEnumerable<int> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName)).Select(int.Parse);
    }
}