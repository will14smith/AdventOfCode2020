using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day8
    {
        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Character.Lower.AtLeastOnce(), TokenType.OpCode)
            .Match(Numerics.Integer, TokenType.Number)
            .Build();

        private static readonly TokenListParser<TokenType, int> Num = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32);

        private static readonly TokenListParser<TokenType, Op> Nop = Token.EqualToValue(TokenType.OpCode, "nop").IgnoreThen(Num).Select(n => (Op) new Op.Nop(n));
        private static readonly TokenListParser<TokenType, Op> Acc = Token.EqualToValue(TokenType.OpCode, "acc").IgnoreThen(Num).Select(n => (Op) new Op.Acc(n));
        private static readonly TokenListParser<TokenType, Op> Jmp = Token.EqualToValue(TokenType.OpCode, "jmp").IgnoreThen(Num).Select(n => (Op) new Op.Jmp(n));

        private static readonly TokenListParser<TokenType, Op> Instruction = Nop.Or(Acc).Or(Jmp);
        private static readonly TokenListParser<TokenType, Op[]> Instructions = Instruction.AtLeastOnce();

        private abstract class Op
        {
            public class Nop : Op
            {
                public int Value { get; }
                public Nop(in int value) => Value = value;
            }

            public class Acc : Op
            {
                public int Value { get; }
                public Acc(in int value) => Value = value;
            }

            public class Jmp : Op
            {
                public int Value { get; }
                public Jmp(in int value) => Value = value;
            }
        }


        private enum TokenType
        {
            OpCode,
            Number,
        }
    }
}