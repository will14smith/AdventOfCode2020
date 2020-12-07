using System.Collections.Generic;
using System.IO;
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
    public class Day7
    {
        private static readonly string Sample = @"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.";

        private static readonly Tokenizer<Day7Token> Tokenizer = new TokenizerBuilder<Day7Token>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.EqualTo('.'), Day7Token.RuleEnd)
            .Match(Character.EqualTo(','), Day7Token.ListSep)
            .Match(Numerics.Integer, Day7Token.Number)
            .Match(Span.EqualTo("bag").Then(_ => Character.EqualTo('s').Optional()), Day7Token.Bags)
            .Match(Span.EqualTo("contain"), Day7Token.Contain)
            .Match(Span.EqualTo("no other"), Day7Token.NoOther)
            .Match(Character.Letter.AtLeastOnce(), Day7Token.Identifier)
            .Build();

        private static readonly TokenListParser<Day7Token, string> Bag =
            from ident in Token.EqualTo(Day7Token.Identifier).AtLeastOnce()
            from _bags in Token.EqualTo(Day7Token.Bags)
            select string.Join(" ", ident.Select(x => x.ToStringValue()));
        private static readonly TokenListParser<Day7Token, (int Count, string Bag)> CountedBag =
            from count in Token.EqualTo(Day7Token.Number).Apply(Numerics.IntegerInt32)
            from bag in Bag
            select (count, bag);

        private static readonly TokenListParser<Day7Token, Dictionary<string, int>> Inner =
            CountedBag.AtLeastOnceDelimitedBy(Token.EqualTo(Day7Token.ListSep)).Select(xs => xs.ToDictionary(x => x.Bag, x => x.Count))
                .Or(Token.Sequence(Day7Token.NoOther, Day7Token.Bags).Select(_ => new Dictionary<string, int>()));

        private static readonly TokenListParser<Day7Token, Rule> RuleParser =
            from outer in Bag
            from _contain in Token.EqualTo(Day7Token.Contain)
            from inner in Inner
            from _end in Token.EqualTo(Day7Token.RuleEnd)
            select new Rule(outer, inner);

        private static readonly TokenListParser<Day7Token, Rule[]> Rules = RuleParser.AtLeastOnce();

        private readonly ITestOutputHelper _output;

        public Day7(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day7");

            _output.Run("sample", () => CountOuterBags(Sample, "shiny gold"))
                .Should().Be(4);

            _output.Run("actual", () => CountOuterBags(data, "shiny gold"));
        }

        private static int CountOuterBags(string data, string target)
        {
            var rules = Rules.MustParse(Tokenizer, data);

            var outers = new HashSet<string>();
            var workList = new Queue<string>();

            outers.Add(target);
            workList.Enqueue(target);

            while (workList.Count > 0)
            {
                var bag = workList.Dequeue();

                var outerRules = rules.Where(rule => rule.CanContain(bag));
                foreach (var rule in outerRules)
                {
                    if (outers.Add(rule.Outer))
                    {
                        workList.Enqueue(rule.Outer);
                    }
                }
            }

            // ignore the original target
            return outers.Count - 1;
        }

        private enum Day7Token
        {
            RuleEnd,
            ListSep,
            Number,
            Bags,
            Contain,
            NoOther,
            Identifier,
        }

        private class Rule
        {
            public string Outer { get; }
            public IReadOnlyDictionary<string, int> Inner { get; }

            public Rule(string outer, IReadOnlyDictionary<string, int> inner)
            {
                Outer = outer;
                Inner = inner;
            }

            public bool CanContain(string bag) => Inner.ContainsKey(bag);
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}