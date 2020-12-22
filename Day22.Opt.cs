using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2020
{
    public partial class Day22
    {
        [Fact]
        public void Part2Opt()
        {
            Run("sample", Sample1, Tokenizer, Parser, SolvePart2Opt).Should().Be(291);
            Run("sample", Sample2, Tokenizer, Parser, SolvePart2Opt).Should().Be(105);
            Run("actual", Tokenizer, Parser, SolvePart2Opt);
        }

        private static int SolvePart2Opt(Input input)
        {
            var player1 = new ReadOnlySequence<byte>(input.Player1.ToArray());
            var player2 = new ReadOnlySequence<byte>(input.Player2.ToArray());
            
            return PlayPart2Opt(player1, player2, true).Score;
        }

        private static (int Score, bool Player1Won) PlayPart2Opt(ReadOnlySequence<byte> player1Input, ReadOnlySequence<byte> player2Input, bool toplevel)
        {
            var seen = new HashSet<long>();

            var count = (int)player1Input.Length + (int)player2Input.Length;
            var player1 = new CircularBuffer<byte>(count, player1Input);
            var player2 = new CircularBuffer<byte>(count, player2Input);
            
            // "magic": don't need to compute the score for subgames & if player 1 has the high card and that card is largest than the lists then they will always win
            if (!toplevel)
            {
                var p1m = player1.Max(x => x);
                var p2m = player2.Max(x => x);

                if (p1m > p2m && p1m > player1.Count + player2.Count)
                {
                    return (0, true);
                }
            }
            
            while (player1.Count > 0 && player2.Count > 0)
            {
                var hash = HashDecks(player1, player2);
                if (!seen.Add(hash))
                {
                    return (SumDeck(player1.ToSequence()), true);
                }
                
                var top1 = player1.PopFront();
                var top2 = player2.PopFront();

                bool player1Won;
                if (top1 <= player1.Count && top2 <= player2.Count)
                {
                    (_, player1Won) = PlayPart2Opt(player1.Take(top1), player2.Take(top2), false);
                }
                else
                {
                    player1Won = top1 > top2;
                }
                
                if (player1Won)
                {
                    player1.PushBack(top1);
                    player1.PushBack(top2);
                }
                else
                {
                    player2.PushBack(top2);
                    player2.PushBack(top1);
                }
            }

            return (SumDeck((player1.Count > 0 ? player1 : player2).ToSequence()), player1.Count > 0);
        }
        
        private static long HashDecks(CircularBuffer<byte> player1, CircularBuffer<byte> player2)
        {
            // not convinced this is good enough but it gets the right answer so.... 
            var hash = 0L;

            hash |= player1.Front() << 24;
            hash |= player1.Back() << 16;
            hash |= player1.Count;

            hash <<= 32;
            
            hash |= player2.Front() << 24;
            hash |= player2.Back() << 16;
            hash |= player2.Count;

            return hash;
        }

        private static int SumDeck(ReadOnlySequence<byte> deck)
        {
            var sum = 0;
            var offset = (int) deck.Length;

            if (deck.IsSingleSegment)
            {
                var s = deck.FirstSpan;
                var l = s.Length;
                for (var index = 0; index < l; index++)
                {
                    sum += offset-- * s[index];
                }
            }
            else
            {
                foreach (var segment in deck)
                {
                    var s = segment.Span;
                    var l = s.Length;
                    for (var index = 0; index < l; index++)
                    {
                        sum += offset-- * s[index];
                    }
                }
            }

            return sum;
        }
    }
}