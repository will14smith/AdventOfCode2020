using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2020
{
    public partial class Day23
    {
        [Fact]
        public void Part2Opt()
        {
            Run("sample", Sample, Parse, SolvePart2Opt).Should().Be("149245887792");
            Run("actual", Actual, Parse, SolvePart2Opt);
        }
        
        private static string SolvePart2Opt(IReadOnlyList<int> input)
        {
            var nodes = new int[1_000_001];
            nodes[0] = input[0];
            for (var index = 0; index < input.Count; index++)
            {
                if (index == 0)
                {
                    nodes[1_000_000] = input[index];
                }
                else
                {
                    nodes[input[index - 1]] = input[index];
                }
            }

            nodes[input[^1]] = input.Count + 1;

            for (var index = input.Count + 2; index < nodes.Length; index++)
            {
                nodes[index - 1] = index;
            }
            
            FindNode1AfterNIterations(nodes, 10_000_000);

            var first = nodes[1];
            var second = nodes[first];
            
            return $"{(long) first * second}";
        }
        
        private static void FindNode1AfterNIterations(Span<int> nodes, int iterations)
        {
            var max = nodes.Length - 1;

            var current = nodes[0];
            for (var t = 0; t < iterations; t++)
            {
                var pickup1 = nodes[current];
                var pickup2 = nodes[pickup1];
                var pickup3 = nodes[pickup2];
                
                nodes[current] = nodes[pickup3];

                var dst = current;
                do
                {
                    dst = dst == 1 ? max : dst - 1;
                } while (dst == pickup1 || dst == pickup2 || dst == pickup3);
                
                nodes[pickup3] = nodes[dst];
                nodes[dst] = pickup1;

                current = nodes[current];
            }
        }

    }
}