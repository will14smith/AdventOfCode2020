using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day23 : Test
    {
        private const string Sample = "389125467";
        private const string Actual = "219748365";
        
        public Day23(ITestOutputHelper output) : base(23, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Parse, SolvePart1).Should().Be("67384529");
            Run("actual", Actual, Parse, SolvePart1);
        } 
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Parse, SolvePart2).Should().Be("149245887792");
            Run("actual", Actual, Parse, SolvePart2);
        }

        private string SolvePart1(IReadOnlyList<int> input)
        {
            var nodes = input.Select(x => new L(x)).ToList();
            var finalNode = FindNode1AfterNIterations(nodes, 100);

            var output = new StringBuilder();

            var node = finalNode.Next;
            while (node != finalNode)
            {
                output.Append(node.Value);
                node = node.Next;
            }
            
            return output.ToString();
        }
        
        private static string SolvePart2(IReadOnlyList<int> input)
        {
            var nodes = input.Concat(Enumerable.Range(10, 1_000_000-10+1)).Select(x => new L(x)).ToList();
            var finalNode = FindNode1AfterNIterations(nodes, 10_000_000);

            return $"{(long) finalNode.Next.Value * finalNode.Next.Next.Value}";
        }

        private static L FindNode1AfterNIterations(IReadOnlyList<L> nodes, int iterations)
        {
            var max = nodes.Max(x => x.Value);
            var indexedNodes = nodes.ToDictionary(x => x.Value);
            for (var i = 0; i < nodes.Count; i++)
            {
                if (i == 0) nodes[^1].Next = nodes[i];
                else nodes[i - 1].Next = nodes[i];
            }

            var current = nodes[0];
            for (var t = 0; t < iterations; t++)
            {
                var pickup = current.Next;
                var pickupValues = new[] {pickup.Value, pickup.Next.Value, pickup.Next.Next.Value};

                current.Next = pickup.Next.Next.Next;

                var dst = current.Value;
                do
                {
                    dst = dst == 1 ? max : dst - 1;
                } while (pickupValues.Contains(dst));

                var target = indexedNodes[dst];

                pickup.Next.Next.Next = target.Next;
                target.Next = pickup;

                current = current.Next;
            }

            while (current.Value != 1)
            {
                current = current.Next;
            }

            return current;
        }

        private class L
        {
            public L(int value)
            {
                Value = value;
            }

            public int Value { get; }
            public L Next { get; set; }
        }
        
        private static IReadOnlyList<int> Parse(string input) => input.Select(x => x - (byte) '0').ToList();
    }
}