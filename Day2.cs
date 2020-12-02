using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day2
    {
        private static readonly IEnumerable<string> Sample = new[] { "1-3 a: abcde", "1-3 b: cdefg", "2-9 c: ccccccccc" };

        private static readonly TextParser<(int Min, int Max)> RangeParser =
            from min in Numerics.IntegerInt32
            from _ in Character.EqualTo('-')
            from max in Numerics.IntegerInt32
            select (min, max);

        private static readonly TextParser<Input> InputParser =
            from range in RangeParser.IgnoreWhitespace()
            from target in Character.AnyChar
            from _ in Character.EqualTo(':').IgnoreWhitespace()
            from pwd in Character.AnyChar.Many()
            select new Input(range.Min, range.Max, target, new string(pwd));

        private readonly ITestOutputHelper _testOutputHelper;

        public Day2(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day2");

            _testOutputHelper.WriteLine("[*] Calculating for sample data");

            var result = CountValidPasswords(Sample, IsValidPart1);
            result.Should().Be(2);

            _testOutputHelper.WriteLine($"[*] Result = {result}");

            _testOutputHelper.WriteLine("[*] Calculating for actual data");
            _testOutputHelper.WriteLine($"[*] Result = {CountValidPasswords(data, IsValidPart1)}");

            static bool IsValidPart1(Input input)
            {
                var count = input.Pwd.Count(c => c == input.Target);
                return count >= input.Min && count <= input.Max;
            }
        }

        private static int CountValidPasswords(IEnumerable<string> data, Func<Input, bool> isValid)
        {
            return data.Select(x => InputParser.MustParse(x)).Count(isValid);
        }

        private static IReadOnlyCollection<string> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName));

        private class Input
        {
            public int Min { get; }
            public int Max { get; }
            public char Target { get; }
            public string Pwd { get; }

            public Input(in int min, in int max, in char target, string pwd)
            {
                Min = min;
                Max = max;
                Target = target;
                Pwd = pwd;
            }
        }
    }
}