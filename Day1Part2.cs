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
    public class Day1Part2
    {
        private static readonly IEnumerable<int> Sample = new[] { 1721, 979, 366, 299, 675, 1456 };

        private readonly ITestOutputHelper _output;

        public Day1Part2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Main()
        {
            var data = LoadData("day1part2");

            _output.Run("sample", () => Calculate(Sample))
                .Should().Be(241861950);

            _output.Run("actual", () => Calculate(data));
        }

        private static int Calculate(IEnumerable<int> data)
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

        private static IEnumerable<int> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName)).Select(int.Parse);
    }
}