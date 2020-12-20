using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace AdventOfCode2020
{
    public partial class Day20
    {
        private static readonly Tokenizer<TokenType> Tokenizer = new TokenizerBuilder<TokenType>()
            .Match(Character.EqualTo('\n'), TokenType.NewLine)
            .Ignore(Span.WhiteSpace)
            .Match(Numerics.Integer, TokenType.Number)
            .Match(Span.EqualTo("Tile"), TokenType.Tile)
            .Match(Character.EqualTo(':'), TokenType.Colon)
            .Match(Character.EqualTo('.'), TokenType.Pixel)
            .Match(Character.EqualTo('#'), TokenType.Pixel)
            .Build();

        private static readonly TokenListParser<TokenType, long> Number = Token.EqualTo(TokenType.Number).Apply(Numerics.IntegerInt64); 
        private static readonly TokenListParser<TokenType, long> TileHeader = Token.EqualTo(TokenType.Tile).IgnoreThen(Number).ThenIgnore(Token.Sequence(TokenType.Colon, TokenType.NewLine)); 
        private static readonly TokenListParser<TokenType, bool> Pixel = Token.EqualTo(TokenType.Pixel).Select(x => x.Span.Source[x.Position.Absolute] == '#'); 
        private static readonly TokenListParser<TokenType, bool[]> ImageLine = Pixel.AtLeastOnce().ThenIgnore(Token.EqualTo(TokenType.NewLine));
        private static readonly TokenListParser<TokenType, bool[,]> ImageLines = ImageLine.AtLeastOnce().Select(CombineImageLines);
        private static readonly TokenListParser<TokenType, Tile> TileParser = TileHeader.Then(id => ImageLines.Select(image => new Tile(id, image))).ThenIgnore(Token.EqualTo(TokenType.NewLine));
        private static readonly TokenListParser<TokenType, Model> Parser = TileParser.AtLeastOnce().Select(tile => new Model(tile.ToDictionary(x => x.Id)));

        private record Tile(long Id, bool[,] Image);
        private record Model(IReadOnlyDictionary<long, Tile> Tiles);
        
        private static unsafe bool[,] CombineImageLines(bool[][] input)
        {
            var output = new bool[input.Length, input[0].Length];
            var length = output.GetLength(0) * output.GetLength(1);
            
            fixed (bool* p = output)
            {
                var span = new Span<bool>(p, length);
            
                var offset = 0;
                foreach (var line in input)
                {
                    line.CopyTo(span.Slice(offset));
                    offset += line.Length;
                }
            }

            return output;
        }
        
        private enum TokenType
        {
            Number,
            Tile,
            
            Colon,
            Pixel,
            NewLine,
        }
    }
}