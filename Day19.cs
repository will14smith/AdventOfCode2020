﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day19 : Test
    {
        private const string Sample1 = "0: 4 1 5\n1: 2 3 | 3 2\n2: 4 4 | 5 5\n3: 4 5 | 5 4\n4: \"a\"\n5: \"b\"\n\nababbb\nbababa\nabbbab\naaabbb\naaaabbb\n";
        private const string Sample2 = "42: 9 14 | 10 1\n9: 14 27 | 1 26\n10: 23 14 | 28 1\n1: \"a\"\n11: 42 31\n5: 1 14 | 15 1\n19: 14 1 | 14 14\n12: 24 14 | 19 1\n16: 15 1 | 14 14\n31: 14 17 | 1 13\n6: 14 14 | 1 14\n2: 1 24 | 14 4\n0: 8 11\n13: 14 3 | 1 12\n15: 1 | 14\n17: 14 2 | 1 7\n23: 25 1 | 22 14\n28: 16 1\n4: 1 1\n20: 14 14 | 1 15\n3: 5 14 | 16 1\n27: 1 6 | 14 18\n14: \"b\"\n21: 14 1 | 1 14\n25: 1 1 | 1 14\n22: 14 14\n8: 42\n26: 14 22 | 1 20\n18: 15 15\n7: 14 5 | 1 21\n24: 14 1\n\nabbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa\nbbabbbbaabaabba\nbabbbbaabbbbbabbbbbbaabaaabaaa\naaabbbbbbaaaabaababaabababbabaaabbababababaaa\nbbbbbbbaaaabbbbaaabbabaaa\nbbbababbbbaaaaaaaabbababaaababaabab\nababaaaaaabaaab\nababaaaaabbbaba\nbaabbaaaabbaaaababbaababb\nabbbbabbbbaaaababbbbbbaaaababb\naaaaabbaabaaaaababaa\naaaabbaaaabbaaa\naaaabbaabbaaaaaaabbbabbbaaabbaabaaa\nbabaaabbbaaabaababbaabababaaab\naabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba\n";
        
        public Day19(ITestOutputHelper output) : base(19, output) { }
        
        [Fact]
        public void Part1()
        {
            Run("sample", "0: 1 2\n1: \"a\"\n2: \"b\"\n\na\nb\nab\nabb\n", Tokenizer, Parser, SolvePart1).Should().Be(1);
            Run("sample", "0: 1 | 2\n1: \"a\"\n2: \"b\"\n\na\nb\nab\nabb\n", Tokenizer, Parser, SolvePart1).Should().Be(2);
            Run("sample", Sample1, Tokenizer, Parser, SolvePart1).Should().Be(2);
            Run("sample", Sample2, Tokenizer, Parser, SolvePart1).Should().Be(3);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }
        
        [Fact]
        public void Part2()
        {
            Run("sample", "0: 2 0 3 | 2 3\n1: \"a\"\n2: 1 | 1 1\n3: \"b\"\n\naab\naaabb\naaaabb\naaaabbbb\naaabbbb\n".Replace("\r", ""), Tokenizer, Parser, SolvePart2).Should().Be(4);
            Run("sample", Sample2, Tokenizer, Parser, SolvePart2).Should().Be(12);
            Run("actual", Tokenizer, Parser, SolvePart2);
        }

        private static int SolvePart1(Spec input)
        {
            var (rules, messages) = input;
           
            return messages.Count(message => IsMatch(rules, message));
        }
        
        private static int SolvePart2(Spec input)
        {
            var (rules, messages) = input;
            rules = FixRules(rules);
            
            return messages.Count(message => IsMatch(rules, message));
        }

        private static bool IsMatch(IReadOnlyDictionary<int,Rule> rules, string message)
        {
            var start = new Rule.Sequence(new Rule[] {new Rule.Reference(0), new Rule.Match("$")});

            return IsMatch(start, rules, message + "$").Any(x => x.Success);
        }

        private static IEnumerable<(bool Success, string Remaining)> IsMatch(Rule rule, IReadOnlyDictionary<int,Rule> rules, string message)
        {
            return rule switch
            {
                Rule.Match match => new [] { (message.StartsWith(match.Value), message[match.Value.Length..]) },
                Rule.Reference reference => IsMatch(rules[reference.Rule], rules, message),
                Rule.Sequence sequence => IsMatch(sequence, rules, message),
                Rule.Alternative alternative => IsMatch(alternative, rules, message),
                _ => throw new ArgumentOutOfRangeException(nameof(rule))
            };
        }

        private static IEnumerable<(bool Success, string Remaining)> IsMatch(Rule.Sequence sequence, IReadOnlyDictionary<int, Rule> rules, string message)
        {
            IEnumerable<string> inputs = new[] { message };

            inputs = sequence.Rules.Aggregate(inputs, (current, rule) => current
                .SelectMany(input => IsMatch(rule, rules, input))
                .Where(x => x.Success)
                .Select(x => x.Remaining));

            return inputs.Select(x => (true, x));
        } 
        private static IEnumerable<(bool Success, string Remaining)> IsMatch(Rule.Alternative alternative, IReadOnlyDictionary<int, Rule> rules, string message)
        {
            return alternative.Rules.SelectMany(rule => IsMatch(rule, rules, message));
        }

        private static IReadOnlyDictionary<int, Rule> FixRules(IReadOnlyDictionary<int,Rule> rules)
        {
            var newRules = rules.ToDictionary(x => x.Key, x => x.Value);
            
            newRules[8] = new Rule.Alternative(new Rule[]
            {
                new Rule.Sequence(new[] {new Rule.Reference(42), new Rule.Reference(8)}),
                new Rule.Reference(42),
            });
            newRules[11] = new Rule.Alternative(new[]
            {
                new Rule.Sequence(new[] {new Rule.Reference(42), new Rule.Reference(11), new Rule.Reference(31)}),
                new Rule.Sequence(new[] {new Rule.Reference(42), new Rule.Reference(31)}),
            });
            
            return newRules;
        }


    }
}