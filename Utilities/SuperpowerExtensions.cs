using System;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace AdventOfCode2020.Utilities
{
    public static class SuperpowerExtensions
    {
        public static readonly TextParser<string> NewLine = Character.EqualTo('\n').Select(_ => "\n").Or(Character.EqualTo('\r').Then(_ => Character.EqualTo('\n').Select(_ => "\n")));

        public static T MustParse<T>(this TextParser<T> parser, string input)
        {
            var result = parser(new TextSpan(input));
            if(!result.HasValue) throw new Exception(result.ToString());

            return result.Value;
        }

        public static TextParser<T> IgnoreWhitespace<T>(this TextParser<T> parser)
        {
            return
                from _1 in Character.WhiteSpace.Many()
                from result in parser
                from _2 in Character.WhiteSpace.Many()
                select result;
        }
    }
}