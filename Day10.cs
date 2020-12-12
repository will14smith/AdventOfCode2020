using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Medallion.Collections;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day10 : Test
    {
        private const string Sample1 = "16\n10\n15\n5\n1\n11\n7\n19\n6\n12\n4";
        private const string Sample2 = "28\n33\n18\n42\n31\n14\n46\n20\n48\n47\n24\n23\n49\n45\n19\n38\n39\n11\n1\n32\n25\n35\n8\n17\n7\n9\n4\n2\n34\n10\n3";

        public Day10(ITestOutputHelper output) : base(10, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample1", Sample1, Parse, SolvePart1).Should().Be(7*5);
            Run("sample2", Sample2, Parse, SolvePart1).Should().Be(22*10);
            Run("actual", Parse, SolvePart1);
        }

        [Fact]
        public void Part2()
        {
            Run("sample1",Sample1, Parse, CountCombinations).Should().Be(8);
            Run("sample2", Sample2, Parse, CountCombinations).Should().Be(19208);
            Run("actual", Parse, CountCombinations);
        }

        private static long SolvePart1(IEnumerable<long> data)
        {
            var distribution = FindDistribution(data);

            var n1 = distribution.GetOrAdd(1, _ => 0);
            var n3 = distribution.GetOrAdd(3, _ => 0);

            return n1 * (n3 + 1);
        }

        private static ConcurrentDictionary<int, int> FindDistribution(IEnumerable<long> data)
        {
            var dist = new ConcurrentDictionary<int, int>();

            var previousValue = 0L;
            foreach (var value in data.OrderBy(x => x))
            {
                dist.AddOrUpdate((int) (value - previousValue), _ => 1, (_, c) => c + 1);
                previousValue = value;
            }

            return dist;
        }

        private static long CountCombinations(IEnumerable<long> data)
        {
            var orderedData = data.OrderBy(x => x).ToImmutableLinkedList();

            return CountCombinations(orderedData.Prepend(0), new Dictionary<int, long>());
        }

        private static long CountCombinations(in ImmutableLinkedList<long> data, Dictionary<int, long> memo)
        {
            // assume the head node is not skipped
            if (memo.TryGetValue(data.Count, out var memoResult)) return memoResult;

            if (data.Count == 0) return 1;

            var combinations = 0L;

            // 1, 2, 3, 4
            // ^
            // combines = 1 : 2 ...
            // combines = 1 : 3 ...
            // combines = 1 : 4 ...
            var skip0 = CanSkip(data, 0);
            var skip1 = CanSkip(data, 1);

            combinations += CountCombinations(data.Tail, memo);
            if(skip0) combinations += CountCombinations(data.Tail.Tail, memo);
            if(skip1) combinations += CountCombinations(data.Tail.Tail.Tail, memo);

            memo.Add(data.Count, combinations);

            return combinations;
        }

        private static bool CanSkip(ImmutableLinkedList<long> data, int count)
        {
            if (data.Count < count + 3)
            {
                return false;
            }

            var head = data.Head;
            data = Skip(data.Tail, count + 1);

            return data.Head - head <= 3;
        }

        private static ImmutableLinkedList<long> Skip(ImmutableLinkedList<long> data, int count)
        {
            while (count-- > 0) data = data.Tail;
            return data;
        }

        private static IReadOnlyList<long> Parse(string input) => input.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
    }
}