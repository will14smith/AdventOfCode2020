using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day15 : Test
    {
        private const string Input = "10,16,6,0,1,17";
        
        public Day15(ITestOutputHelper output) : base(15, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", "0,3,6", Parse, SolvePart1).Should().Be(436);
            Run("sample", "1,3,2", Parse, SolvePart1).Should().Be(1);
            Run("sample", "2,1,3", Parse, SolvePart1).Should().Be(10);
            Run("sample", "1,2,3", Parse, SolvePart1).Should().Be(27);
            Run("actual", Input, Parse, SolvePart1);
        }     
        
        [Fact]
        public void Part2()
        {
            Run("sample", "0,3,6", Parse, SolvePart2).Should().Be(175594);
            Run("sample", "1,3,2", Parse, SolvePart2).Should().Be(2578);
            Run("sample", "2,1,3", Parse, SolvePart2).Should().Be(3544142);
            Run("sample", "1,2,3", Parse, SolvePart2).Should().Be(261214);
            Run("actual", Input, Parse, SolvePart2);
        }

        private int SolvePart1(IEnumerable<int> input) => Solve(input, 2020);
        private int SolvePart2(IEnumerable<int> input) => Solve(input, 30000000);

        private static int Solve(IEnumerable<int> input, int count)
        {
            var tracker = ArrayPool<int>.Shared.Rent(count);
            Array.Clear(tracker, 0, count);

            var i = 0;
            var last = 0;
            
            foreach (var arg in input)
            {
                if (i > 0)
                {
                    tracker[last] = i;
                }

                last = arg;
                i++;
            }

            for (; i < count; i++)
            {
                var num = 0;
                var lastIndex = tracker[last];
                if (lastIndex != 0)
                {
                    num = i - lastIndex;
                }

                tracker[last] = i;
                last = num;
            }

            ArrayPool<int>.Shared.Return(tracker);
            
            return last;
        }

        private static IEnumerable<int> Parse(string input) => input.Split(',').Select(int.Parse);
    }
}