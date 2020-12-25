using System;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day25 : Test
    {
        private const string Sample = "5764801\n17807724\n";
        
        public Day25(ITestOutputHelper output) : base(25, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Parse, SolvePart1).Should().Be(14897079);
            Run("actual", Parse, SolvePart1);
        }  
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Parse, SolvePart2).Should().Be(0);
            Run("actual", Parse, SolvePart2);
        }

        private long SolvePart1((long, long) input)
        {
            var (cardPublicKey, doorPublicKey) = input;

            var cardLoopSize = 0;
            var cardValue = 1;
            while (true)
            {
                if (cardPublicKey == cardValue) { break; }
                cardValue = cardValue * 7 % 20201227;
                cardLoopSize++;
            }

            var doorLoopSize = 0;
            var doorValue = 1;
            while (true)
            {
                if (doorPublicKey == doorValue) { break; }
                doorValue = doorValue * 7 % 20201227;
                doorLoopSize++;
            }

            var key1 = Transform(doorLoopSize, cardPublicKey);
            var key2 = Transform(cardLoopSize, doorPublicKey);

            if (key1 != key2)
            {
                throw new InvalidOperationException("keys don't match");
            }

            return key1;
        }

        private long Transform(long loopSize, long subjectNumber)
        {
            var value = 1L;

            for (var i = 0; i < loopSize; i++)
            {
                value = value * subjectNumber % 20201227;
            }

            return value;
        }
        
        private long SolvePart2((long, long) input)
        {
            throw new System.NotImplementedException();
        }

        private static (long, long) Parse(string input)
        {
            var parts = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return (long.Parse(parts[0]), long.Parse(parts[1]));
        }
    }
}