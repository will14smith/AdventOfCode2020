using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day10
    {
        private static readonly IReadOnlyList<long> Sample1 = "16\n10\n15\n5\n1\n11\n7\n19\n6\n12\n4"
            .Split("\n").Select(long.Parse).ToList();
        private static readonly IReadOnlyList<long> Sample2 = "28\n33\n18\n42\n31\n14\n46\n20\n48\n47\n24\n23\n49\n45\n19\n38\n39\n11\n1\n32\n25\n35\n8\n17\n7\n9\n4\n2\n34\n10\n3"
            .Split("\n").Select(long.Parse).ToList();

        private readonly ITestOutputHelper _output;

        public Day10(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day10");

            _output.Run("sample1", () => SolvePart1(Sample1))
                .Should().Be(7*5);
            _output.Run("sample2", () => SolvePart1(Sample2))
                .Should().Be(22*10);

            _output.Run("actual", () => SolvePart1(data));
        }

        private long SolvePart1(IReadOnlyList<long> data)
        {
            var distribution = FindDistribution(data);

            var n1 = distribution.GetOrAdd(1, _ => 0);
            var n3 = distribution.GetOrAdd(3, _ => 0);

            return n1 * (n3 + 1);
        }

        private ConcurrentDictionary<int, int> FindDistribution(IReadOnlyList<long> data)
        {
            var dist = new ConcurrentDictionary<int, int>();

            var previousValue = 0l;
            foreach (var value in data.OrderBy(x => x))
            {
                dist.AddOrUpdate((int) (value - previousValue), _ => 1, (_, c) => c + 1);
                previousValue = value;
            }

            return dist;
        }

        private static IReadOnlyList<long> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName)).Select(long.Parse).ToList();
    }
}