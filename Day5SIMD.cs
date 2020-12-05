using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day5SIMD
    {
        private static readonly byte[] Sample = Encoding.ASCII.GetBytes("BFFFBBFRRR\nFFFBBBFRRR\nBBFFBBFRLL\nBFFFBBFRRR\nFFFBBBFRRR");

        private static readonly Vector256<byte> CharMask = Vector256.Create(4774451407314113106, 4774469068483347010, 5931814938896122434, 4774451407313060434).AsByte();
        private static readonly Vector256<byte> ShuffleControl = Vector256.Create(1736447835066146335, 1157726452361532951, 579005069656919567, 283686952306183).AsByte();

        private readonly ITestOutputHelper _output;

        public Day5SIMD(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day5");

            _output.Run("sample", () => GetLargestSeatId(Sample))
                .Should().Be(820);

            _output.Run("actual", () => GetLargestSeatId(data));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day5");

            _output.Run("actual", () => FindMySeatId(data));
        }

        private static unsafe int GetLargestSeatId(byte[] input)
        {
            var max = 0;

            fixed (byte* addr = input)
            {
                int i;
                for (i = 0; i < input.Length; i += 33)
                {
                    var v = Load3(addr + i);
                    var a = Parse(v, CharMask, ShuffleControl);

                    var a1 = (a >> 22) & 0x3ff;
                    var a2 = (a >> 11) & 0x3ff;
                    var a3 = a & 0x3ff;

                    max = max > a1 ? max : a1;
                    max = max > a2 ? max : a2;
                    max = max > a3 ? max : a3;
                }

                if (i - 1 <= input.Length)
                {
                    return max;
                }

                // handling trailing records
                {
                    i -= 33;

                    var numRecords = input.Length - i >= 21 ? 2 : 1;
                    var v = numRecords == 2 ? Load2(addr + i) : Load1(addr + i);
                    var a = Parse(v, CharMask, ShuffleControl);

                    var a1 = (a >> 22) & 0x3ff;
                    var a2 = (a >> 11) & 0x3ff;

                    max = max > a1 ? max : a1;
                    if (numRecords == 2)
                    {
                        max = max > a2 ? max : a2;
                    }
                }
            }

            return max;
        }

        private static int FindMySeatId(byte[] input)
        {
            var seats = new List<int>();

            unsafe
            {
                fixed (byte* addr = input)
                {
                    int i;
                    for (i = 0; i < input.Length; i += 33)
                    {
                        var v = Load3(addr + i);
                        var a = Parse(v, CharMask, ShuffleControl);

                        seats.Add((a >> 22) & 0x3ff);
                        seats.Add((a >> 11) & 0x3ff);
                        seats.Add(a & 0x3ff);
                    }

                    // handle trailing records
                    if (i - 1 > input.Length)
                    {
                        i -= 33;

                        var numRecords = input.Length - i >= 21 ? 2 : 1;
                        var v = numRecords == 2 ? Load2(addr + i) : Load1(addr + i);
                        var a = Parse(v, CharMask, ShuffleControl);

                        seats.Add((a >> 22) & 0x3ff);
                        if (numRecords == 2)
                        {
                            seats.Add((a >> 11) & 0x3ff);
                        }
                    }
                }
            }

            seats.Sort();

            for (var i = 1; i < seats.Count; i++)
            {
                if (seats[i - 1] + 1 != seats[i])
                {
                    return seats[i - 1] + 1;
                }
            }

            throw new Exception("Couldn't find seat");
        }

        private static unsafe Vector256<byte> Load3(byte* input) => Avx.LoadVector256(input);
        private static unsafe Vector256<byte> Load2(byte* input) => Vector256.Create(input[0], input[1], input[2], input[3], input[4], input[5], input[6], input[7], input[8], input[9], input[10], input[11], input[12], input[13], input[14], input[15], input[16], input[17], input[18], input[19], input[20], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        private static unsafe Vector256<byte> Load1(byte* input) => Vector256.Create(input[0], input[1], input[2], input[3], input[4], input[5], input[6], input[7], input[8], input[9], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        private static int Parse(Vector256<byte> v, in Vector256<byte> c, in Vector256<byte> s)
        {
            v = Avx2.Shuffle(v, s);
            v = Avx2.Permute4x64(v.AsInt64(), 78).AsByte();
            v = Avx2.CompareEqual(v, c);
            return Avx2.MoveMask(v);
        }

        private static byte[] LoadData(string fileName) =>
            File.ReadAllBytes(Path.Combine("Inputs", fileName));
    }
}