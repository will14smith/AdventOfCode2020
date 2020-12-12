using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day09 : Test
    {
        private const string Sample = "35\n20\n15\n25\n47\n40\n62\n55\n65\n95\n102\n117\n150\n182\n127\n219\n299\n277\n309\n576";

        public Day09(ITestOutputHelper output) : base(9, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Parse, data => FindFirstInconsistency(data, 5)).Should().Be(127);
            Run("actual", Parse, data => FindFirstInconsistency(data, 25));
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Parse, data => FindInconsistencyRangeSum(data, 127)).Should().Be(62);
            Run("actual", Parse, data => FindInconsistencyRangeSum(data, 400480901));
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

        private static void Add(ConcurrentDictionary<long, int> d, long v)
        {
            d.AddOrUpdate(v, k => 1, (k, c) => c + 1);
        }

        private static void Remove(ConcurrentDictionary<long, int> d, long v)
        {
            var newC = d.AddOrUpdate(v, k => 0, (k, c) => c - 1);
            if (newC == 0)
            {
                d.TryRemove(v, out _);
            }
        }

        private static long FindInconsistencyRangeSum(IReadOnlyList<long> data, int target)
        {
            var (start, end) = FindInconsistencyRange(data, target);
            return start + end;
        }

        private static (long, long) FindInconsistencyRange(IReadOnlyList<long> data, int target)
        {
            for (var startIndex = 0; startIndex < data.Count; startIndex++)
            {
                var sum = data[startIndex];

                for (var endIndex = startIndex + 1; endIndex < data.Count; endIndex++)
                {
                    sum += data[endIndex];
                    if (sum == target)
                    {
                        var range = data.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();
                        return (range.Min(), range.Max());
                    }
                }
            }

            throw new Exception("no range");
        }

        private static IReadOnlyList<long> Parse(string input) => input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
    }
}