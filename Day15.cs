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

        private static long SolvePart1(IEnumerable<long> input)
        {
            var tracker = new Dictionary<long, long>();

            var i = 0L;
            var last = 0L;

            foreach (var arg in input)
            {
                if (i > 0)
                {
                    tracker.Add(last, i);
                }

                last = arg;
                i++;
            }

            for (; i < 2020; i++)
            {
                var num = 0L;
                if (tracker.ContainsKey(last))
                {
                    num = i - tracker[last];
                }

                tracker[last] = i;
                last = num;
            }

            return last;
        }

        private static IEnumerable<long> Parse(string input) => input.Split(',').Select(long.Parse);
    }
}