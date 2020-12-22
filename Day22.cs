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
        private const string Sample = "Player 1:\n9\n2\n6\n3\n1\n\nPlayer 2:\n5\n8\n4\n7\n10\n";
        
        public Day22(ITestOutputHelper output) : base(22, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(306);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }

        private long SolvePart1(Input input)
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

        private int SumDeck(IReadOnlyCollection<int> deck) => deck.Select((x, i) => x * (deck.Count - i)).Sum();
    }
}