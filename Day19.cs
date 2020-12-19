using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day19 : Test
    {
        private const string Sample = "0: 4 1 5\n1: 2 3 | 3 2\n2: 4 4 | 5 5\n3: 4 5 | 5 4\n4: \"a\"\n5: \"b\"\n\nababbb\nbababa\nabbbab\naaabbb\naaaabbb\n";
        
        public Day19(ITestOutputHelper output) : base(19, output) { }
        
        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(2);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }

        private static int SolvePart1(Spec input)
        {
            var (rules, messages) = input;
            var parser = BuildParser(rules[0], rules).AtEnd();

            return messages.Count(message => IsValid(parser, message));
        }
        
        private static TextParser<string> BuildParser(Rule rule, IReadOnlyDictionary<int, Rule> rules)
        {
            return rule switch
            {
                Rule.Match match => Span.EqualTo(match.Value).Select(x => x.ToStringValue()),
                Rule.Reference reference => BuildParser(rules[reference.Rule], rules),
                Rule.Alternative alternative => alternative.Rules.Select(r => BuildParser(r, rules)).Aggregate((a, x) => a.Try().Or(x)),
                Rule.Sequence sequence => sequence.Rules.Select(r => BuildParser(r, rules)).Aggregate((a, x) => a.Then(x, (s1, s2) => s1 + s2)),
                _ => throw new ArgumentOutOfRangeException(nameof(rule))
            };
        }
        
        private static bool IsValid(TextParser<string> parser, string message) => parser(new TextSpan(message)).HasValue;
    }
}