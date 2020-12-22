using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day22 : Test
    {
        private const string Sample1 = "Player 1:\n9\n2\n6\n3\n1\n\nPlayer 2:\n5\n8\n4\n7\n10\n";
        private const string Sample2 = "Player 1:\n43\n19\n\nPlayer 2:\n2\n29\n14\n";
        
        public Day22(ITestOutputHelper output) : base(22, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample1, Tokenizer, Parser, SolvePart1).Should().Be(306);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample1, Tokenizer, Parser, SolvePart2).Should().Be(291);
            Run("sample", Sample2, Tokenizer, Parser, SolvePart2).Should().Be(105);
            Run("actual", Tokenizer, Parser, SolvePart2);
        }

        private static long SolvePart1(Input input)
        {
            var player1 = new Queue<int>(input.Player1);
            var player2 = new Queue<int>(input.Player2);

            while (player1.Count > 0 && player2.Count > 0)
            {
                var top1 = player1.Dequeue();
                var top2 = player2.Dequeue();

                if (top1 > top2)
                {
                    player1.Enqueue(top1);
                    player1.Enqueue(top2);
                }
                else
                {
                    player2.Enqueue(top2);
                    player2.Enqueue(top1);
                }
            }

            return SumDeck(player1.Count > 0 ? player1 : player2);
        }

        private static int SolvePart2(Input input)
        {
            return PlayPart2(input.Player1, input.Player2).Score;
        }

        private static (int Score, bool Player1Won) PlayPart2(IEnumerable<int> player1Input, IEnumerable<int> player2Input)
        {
            var seen = new HashSet<string>();
            
            var player1 = new Queue<int>(player1Input);
            var player2 = new Queue<int>(player2Input);
            
            while (player1.Count > 0 && player2.Count > 0)
            {
                var hash = HashDecks(player1, player2);
                if (!seen.Add(hash))
                {
                    return (SumDeck(player1), true);
                }
                
                var top1 = player1.Dequeue();
                var top2 = player2.Dequeue();

                bool player1Won;
                if (top1 <= player1.Count && top2 <= player2.Count)
                {
                    (_, player1Won) = PlayPart2(player1.Take(top1), player2.Take(top2));
                }
                else
                {
                    player1Won = top1 > top2;
                }
                
                if (player1Won)
                {
                    player1.Enqueue(top1);
                    player1.Enqueue(top2);
                }
                else
                {
                    player2.Enqueue(top2);
                    player2.Enqueue(top1);
                }
            }

            return (SumDeck(player1.Count > 0 ? player1 : player2), player1.Count > 0);
        }

        private static string HashDecks(IEnumerable<int> player1, IEnumerable<int> player2) => $"{HashDeck(player1)};{HashDeck(player2)}";
        private static string HashDeck(IEnumerable<int> deck) => string.Join(",", deck);

        private static int SumDeck(IReadOnlyCollection<int> deck) => deck.Select((x, i) => x * (deck.Count - i)).Sum();
    }
}