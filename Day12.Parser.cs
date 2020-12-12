using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day12
    {
        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.Upper, TokenType.OpCode)
            .Match(Numerics.Integer, TokenType.Number)
            .Build();

        private static readonly TokenListParser<TokenType, int> Num = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32);

        private static readonly TokenListParser<TokenType, Op> Instruction = Token.EqualTo(TokenType.OpCode).Then(opCode => Num.Select(n => new Op(opCode.ToStringValue()[0], n)));
        private static readonly TokenListParser<TokenType, Op[]> Instructions = Instruction.AtLeastOnce();

        private class Op
        {
            public Op(char type, int value)
            {
                Type = type;
                Value = value;
            }

            public char Type { get; }
            public int Value { get; }
        }


        private enum TokenType
        {
            OpCode,
            Number,
        }
    }
}