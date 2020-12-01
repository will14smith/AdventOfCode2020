using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day1Part1
    {
        private static readonly IEnumerable<int> Sample = new[] { 1721, 979, 366, 299, 675, 1456 };

        private readonly ITestOutputHelper _testOutputHelper;

        public Day1Part1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Main()
        {
            _testOutputHelper.WriteLine("[*] Calculating for sample data");

            var result = Calculate(Sample);
            result.Should().Be(514579);

            _testOutputHelper.WriteLine($"[*] Result = {result}");

            var data = LoadData("day1part1");
            _testOutputHelper.WriteLine("[*] Calculating for actual data");
            _testOutputHelper.WriteLine($"[*] Result = {Calculate(data)}");
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