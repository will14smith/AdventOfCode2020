using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day16 : Test
    {
        private const string Sample = "class: 1-3 or 5-7\nrow: 6-11 or 33-44\nseat: 13-40 or 45-50\n\nyour ticket:\n7,1,14\n\nnearby tickets:\n7,3,47\n40,4,50\n55,2,20\n38,6,12\n";
        
        public Day16(ITestOutputHelper output) : base(16, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(71);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }

        private static long SolvePart1(Spec spec) =>
            spec.Nearby.SelectMany(ticket => ticket.Values)
                .Where(field =>!IsValid(spec.Rules, field))
                .Sum();
        
        private static bool IsValid(IEnumerable<Rule> rules, long value) => rules.Any(rule => IsValid(rule, value));
        private static bool IsValid(Rule rule, long value) => rule.ValidRanges.Any(range => range.Item1 <= value && value <= range.Item2);
    }
}