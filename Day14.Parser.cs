﻿using System.Linq;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day14
    {
        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Span.EqualTo("mask"), TokenType.Mask)
            .Match(Span.EqualTo("mem"), TokenType.Mem)
            .Match(Numerics.Integer, TokenType.Num)
            .Match(Character.EqualTo('X'), TokenType.X)
            .Match(Character.EqualTo('='), TokenType.Equal)
            .Match(Character.EqualTo('['), TokenType.OpenSquare)
            .Match(Character.EqualTo(']'), TokenType.CloseSquare)
            .Build();

        private static readonly TokenListParser<TokenType, char[]> MaskChar = Token.EqualTo(TokenType.X).Or(Token.EqualTo(TokenType.Num)).Select(token => token.ToStringValue().ToCharArray());
        private static readonly TokenListParser<TokenType, Op> Mask = MaskChar.AtLeastOnce().Select(x => x.SelectMany(y => y).ToArray()).Select(Op.SetMask.FromChars);
        private static readonly TokenListParser<TokenType, Op> MaskOp = Token.EqualTo(TokenType.Mask).IgnoreThen(Token.EqualTo(TokenType.Equal)).IgnoreThen(Mask);
        private static readonly TokenListParser<TokenType, Op> MemOp =
            from _mem in Token.EqualTo(TokenType.Mem)
            from _ob in Token.EqualTo(TokenType.OpenSquare)
            from addr in Token.EqualTo(TokenType.Num).Apply(Numerics.IntegerInt64)
            from _cb in Token.EqualTo(TokenType.CloseSquare)
            from _eq in Token.EqualTo(TokenType.Equal)
            from val in Token.EqualTo(TokenType.Num).Apply(Numerics.IntegerInt64)
            select (Op) new Op.SetMem(addr, val);

        private static readonly TokenListParser<TokenType, Op[]> Program = MaskOp.Or(MemOp).AtLeastOnce();

        private enum TokenType
        {
            Mask,
            Mem,
            Num,
            X,
            Equal,
            OpenSquare,
            CloseSquare,
        }

        private abstract class Op
        {
            public class SetMask : Op
            {
                public static SetMask Clear { get; } = new SetMask(long.MaxValue, 0);

                public SetMask(long andMask, long orMask)
                {
                    AndMask = andMask;
                    OrMask = orMask;
                }

                public long AndMask { get; }
                public long OrMask { get; }

                public static Op FromChars(char[] chars)
                {
                    var andMask = Clear.AndMask;
                    var orMask = Clear.OrMask;

                    for (var i = 0; i < chars.Length; i++)
                    {
                        var charMask = 1L << (35 - i);
                        switch (chars[i])
                        {
                            case '0':
                                andMask ^= charMask;
                                break;
                            case '1':
                                orMask ^= charMask;
                                break;
                        }
                    }

                    return new SetMask(andMask, orMask);
                }
            }

            public class SetMem : Op
            {
                public SetMem(long addr, long value)
                {
                    Addr = addr;
                    Value = value;
                }

                public long Addr { get; }
                public long Value { get; }
            }
        }
    }
}