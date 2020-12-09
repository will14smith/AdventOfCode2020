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
    public class Day9
    {
        private static readonly IReadOnlyList<long> Sample = "35\n20\n15\n25\n47\n40\n62\n55\n65\n95\n102\n117\n150\n182\n127\n219\n299\n277\n309\n576"
            .Split("\n")
            .Select(long.Parse)
            .ToList();

        private readonly ITestOutputHelper _output;

        public Day9(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day9");

            _output.Run("sample", () => FindFirstInconsistency(Sample, 5))
                .Should().Be(127);

            _output.Run("actual", () => FindFirstInconsistency(data, 25));
        }

        private long FindFirstInconsistency(IReadOnlyList<long> data, int size)
        {
            var d = new ConcurrentDictionary<long, int>();

            int i;

            for (i = 0; i < size; i++)
            {
                Add(d, data[i]);
            }

            for (; i < data.Count; i++)
            {
                if (!IsValid(d, data[i]))
                {
                    return data[i];
                }

                Add(d, data[i]);
                Remove(d, data[i-size]);
            }

            throw new Exception("no inconsistency");
        }

        private static bool IsValid(ConcurrentDictionary<long, int> d, long v)
        {
            foreach (var k in d.Keys)
            {
                if (v - k != k && d.ContainsKey(v - k))
                {
                    return true;
                }
            }

            return false;
        }

        private void Add(ConcurrentDictionary<long, int> d, long v)
        {
            d.AddOrUpdate(v, k => 1, (k, c) => c + 1);
        }

        private void Remove(ConcurrentDictionary<long, int> d, long v)
        {
            var newC = d.AddOrUpdate(v, k => 0, (k, c) => c - 1);
            if (newC == 0)
            {
                d.TryRemove(v, out _);
            }
        }

        private static IReadOnlyList<long> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName)).Select(long.Parse).ToList();
    }
}