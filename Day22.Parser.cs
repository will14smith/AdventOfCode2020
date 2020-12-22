using System.Collections.Generic;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day22
    {
        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Ignore(Span.WhiteSpace)
            .Match(Span.EqualTo("Player"), TokenType.Player)
            .Match(Numerics.Integer, TokenType.Number)
            .Match(Character.EqualTo(':'), TokenType.Colon)
            .Build();
        
        private static readonly TokenListParser<TokenType, int> PlayerHeader = Token.Sequence(TokenType.Player, TokenType.Number, TokenType.Colon).Select(t => t[1]).Apply(Numerics.IntegerInt32);
        private static readonly TokenListParser<TokenType, byte[]> Cards = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32).Select(x => (byte)x).AtLeastOnce();
        private static readonly TokenListParser<TokenType, Input> Parser = PlayerHeader.IgnoreThen(Cards).Repeat(2).Select(x => new Input(x[0], x[1]));
            
        private record Input(IReadOnlyList<byte> Player1, IReadOnlyList<byte> Player2);
        
        private enum TokenType
        {
            Player,
            Number,
            Colon
        }
    }
}