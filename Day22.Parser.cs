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
        private static readonly TokenListParser<TokenType, int[]> Cards = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt32).AtLeastOnce();
        private static readonly TokenListParser<TokenType, Input> Parser = PlayerHeader.IgnoreThen(Cards).Repeat(2).Select(x => new Input(x[0], x[1]));
            
        private record Input(IReadOnlyList<int> Player1, IReadOnlyList<int> Player2);
        
        private enum TokenType
        {
            Player,
            Number,
            Colon
        }
    }
}