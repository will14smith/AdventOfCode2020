using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day14 : Test
    {
        private static readonly string Sample1 = "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X\nmem[8] = 11\nmem[7] = 101\nmem[8] = 0\n";
        private static readonly string Sample2 = "mask = 000000000000000000000000000000X1001X\nmem[42] = 100\nmask = 00000000000000000000000000000000X0XX\nmem[26] = 1\n";

        public Day14(ITestOutputHelper output) : base(14, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample1, Tokenizer, Program, SolvePart1).Should().Be(165);
            Run("actual", Tokenizer, Program, SolvePart1);
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample2, Tokenizer, Program, SolvePart2).Should().Be(208);
            Run("actual", Tokenizer, Program, SolvePart2);
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

        private static long SolvePart2(Op[] program)
        {
            var mem = new Dictionary<long, long>();
            var mask = Op.SetMask.Clear;

            foreach (var op in program)
            {
                switch (op)
                {
                    case Op.SetMask maskOp: mask = maskOp; break;
                    case Op.SetMem memOp:

                        var baseAddr = (memOp.Addr & ~mask.AndMask) | mask.OrMask;

                        var floatingMask = mask.AndMask & ~mask.OrMask & ((1L << 36) - 1);

                        foreach (var combination in EnumerateCombinations(floatingMask, 36))
                        {
                            var addr = baseAddr | combination;
                            mem[addr] = memOp.Value;
                        }

                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(op));
                }
            }

            return mem.Sum(x => x.Value);
        }

        private static IEnumerable<long> EnumerateCombinations(long floatingMask, int i)
        {
            if (i == 0)
            {
                return new[] { 0L };
            }

            var sub = EnumerateCombinations(floatingMask, i - 1).ToList();
            var iMask = 1L << (i - 1);

            if ((floatingMask & iMask) == 0)
            {
                return sub;
            }

            return sub.Concat(sub.Select(x => x | iMask));
        }
    }
}