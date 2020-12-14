using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day14 : Test
    {
        private static readonly string Sample = "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X\nmem[8] = 11\nmem[7] = 101\nmem[8] = 0\n";

        public Day14(ITestOutputHelper output) : base(14, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Program, SolvePart1).Should().Be(165);
            Run("actual", Tokenizer, Program, SolvePart1);
        }

        private static long SolvePart1(Op[] program)
        {
            var mem = new Dictionary<long, long>();
            var mask = Op.SetMask.Clear;

            foreach (var op in program)
            {
                switch (op)
                {
                    case Op.SetMask maskOp: mask = maskOp; break;
                    case Op.SetMem memOp: mem[memOp.Addr] = (memOp.Value & mask.AndMask) | mask.OrMask; break;
                    default: throw new ArgumentOutOfRangeException(nameof(op));
                }
            }

            return mem.Sum(x => x.Value);
        }
    }
}