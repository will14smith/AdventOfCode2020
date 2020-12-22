using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day02 : Test
    {
        private static readonly string Sample = "1-3 a: abcde\n1-3 b: cdefg\n2-9 c: ccccccccc\n";

        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Numerics.Natural, TokenType.Number)
            .Match(Character.Letter.AtLeastOnce(), TokenType.String)
            .Match(Character.EqualTo('-'), TokenType.Dash)
            .Match(Character.EqualTo(':'), TokenType.Colon)
            .Build();

        private enum TokenType
        {
            Number,
            String,
            
            Dash,
            Colon,
        }

        private static readonly TokenListParser<TokenType, int> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32);
        private static readonly TokenListParser<TokenType, (int Min, int Max)> Range = Number.ThenIgnore(Token.EqualTo(TokenType.Dash)).Then(Number, (a, b) => (a, b));

        private static readonly TokenListParser<TokenType, Input> InputParser = Range
            .Then(Token.EqualTo(TokenType.String), (a, b) => (Range: a, Target: b.Span.Source[b.Position.Absolute]))
            .ThenIgnore(Token.EqualTo(TokenType.Colon))
            .Then(Token.EqualTo(TokenType.String), (a, b) => new Input(a.Range.Min, a.Range.Max, a.Target, b.ToStringValue()));

        private static readonly TokenListParser<TokenType, Input[]> Parser = InputParser.AtLeastOnce();

        public Day02(ITestOutputHelper output) : base(2, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, data => CountValidPasswords(data, IsValidPart1)).Should().Be(2);
            Run("actual", Tokenizer, Parser,  data => CountValidPasswords(data, IsValidPart1));

            static bool IsValidPart1(Input input)
            {
                var count = input.Pwd.Count(c => c == input.Target);
                return count >= input.Min && count <= input.Max;
            }
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Tokenizer, Parser, data => CountValidPasswords(data, IsValidPart2)).Should().Be(1);
            Run("actual", Tokenizer, Parser,  data => CountValidPasswords(data, IsValidPart2));

            static bool IsValidPart2(Input input)
            {
                var a = input.Pwd[input.Min - 1];
                var b = input.Pwd[input.Max - 1];

                return (a == input.Target) ^ (b == input.Target);
            }
        }

        private static int CountValidPasswords(IEnumerable<Input> inputs, Func<Input, bool> isValid)
        {
            return inputs.Count(isValid);
        }

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