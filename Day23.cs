using System;
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

        private string SolvePart1(IReadOnlyList<byte> input)
        {
            var nodes = input.Select(x => new L(x)).ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                if (i == 0) nodes[^1].Next = nodes[i];
                else nodes[i - 1].Next = nodes[i];
            }

            var current = nodes[0];

            for (var t = 0; t < 100; t++)
            {
                var pickup = current.Next;
                var pickupValues = new [] { pickup.Value, pickup.Next.Value, pickup.Next.Next.Value };

                current.Next = pickup.Next.Next.Next;

                var dst = current.Value;
                do
                {
                    dst = dst == 1 ? 9 : (byte) (dst - 1);
                } while (pickupValues.Contains(dst));
                
                var target = current;
                while (target.Value != dst)
                {
                    target = target.Next;
                }

                pickup.Next.Next.Next = target.Next;
                target.Next = pickup;

                current = current.Next;
            }

            while (current.Value != 1)
            {
                current = current.Next;
            }

            return current.Print()[2..].Replace(" ", "");
        }

        private class L
        {
            public L(byte value)
            {
                Value = value;
            }

            public byte Value { get; }
            public L Next { get; set; }

            public string Print()
            {
                var output = new StringBuilder();
                output.Append($"{Value}");

                var node = Next;
                while (node != this)
                {
                    output.Append($" {node.Value}");
                    node = node.Next;
                }

                return output.ToString();
            }
        }
        
        private static IReadOnlyList<byte> Parse(string input) => input.Select(x => (byte) (x - (byte) '0')).ToList();
    }
}