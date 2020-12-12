using System.Collections.Generic;
using System.Linq;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day07
    {
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
    }
}