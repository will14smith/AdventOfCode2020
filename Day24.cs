using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day24 : Test
    {
        private const string Sample = "sesenwnenenewseeswwswswwnenewsewsw\nneeenesenwnwwswnenewnwwsewnenwseswesw\nseswneswswsenwwnwse\nnwnwneseeswswnenewneswwnewseswneseene\nswweswneswnenwsewnwneneseenw\neesenwseswswnenwswnwnwsewwnwsene\nsewnenenenesenwsewnenwwwse\nwenwwweseeeweswwwnwwe\nwsweesenenewnwwnwsenewsenwwsesesenwne\nneeswseenwwswnwswswnw\nnenwswwsewswnenenewsenwsenwnesesenew\nenewnwewneswsewnwswenweswnenwsenwsw\nsweneswneswneneenwnewenewwneswswnese\nswwesenesewenwneswnwwneseswwne\nenesenwswwswneneswsenwnewswseenwsese\nwnwnesenesenenwwnenwsewesewsesesew\nnenewswnwewswnenesenwnesewesw\neneswnwswnwsenenwnwnwwseeswneewsenese\nneswnwewnwnwseenwseesewsenwsweewe\nwseweeenwnesenwwwswnew\n";

        private static readonly ReadOnlyMemory<(int, int)> Offsets = new[]
        {
            (-1, 0),
            (-1, 1),
            (0, 1),
            (1, 0),
            (1, -1),
            (0, -1),
        };
        
        public Day24(ITestOutputHelper output) : base(24, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(10);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart2).Should().Be(2208);
            Run("actual", Tokenizer, Parser, SolvePart2);
        }
        
        private int SolvePart1(Direction[][] input) => CalculateTiles(input).Count;

        private int SolvePart2(Direction[][] input)
        {
            var tiles = CalculateTiles(input);
            
            for (var day = 0; day < 100; day++)
            {
                var neighbours = new Dictionary<(int, int), int>();
                foreach (var tile in tiles)
                {
                    for (var index = 0; index < Offsets.Span.Length; index++)
                    {
                        var offset = Offsets.Span[index];
                        var neighbour = Add(tile, offset);

                        if (!neighbours.TryAdd(neighbour, 1))
                        {
                            neighbours[neighbour] = neighbours[neighbour] + 1;
                        }
                    }
                }

                var next = new HashSet<(int, int)>();
                foreach (var (neighbour, count) in neighbours)
                {
                    if (count == 2 || count == 1 && tiles.Contains(neighbour))
                    {
                        next.Add(neighbour);
                    }
                }                

                tiles = next;
            }

            return tiles.Count;
        }

        private static HashSet<(int, int)> CalculateTiles(Direction[][] input)
        {
            var tiles = new HashSet<(int, int)>();

            foreach (var line in input)
            {
                var loc = Calculate(line);
                if (tiles.Contains(loc))
                {
                    tiles.Remove(loc);
                }
                else
                {
                    tiles.Add(loc);
                }
            }

            return tiles;
        }

        private static (int, int) Calculate(IEnumerable<Direction> line) => line.Aggregate((0, 0), (current, direction) => Add(current, Offsets.Span[(int) direction]));
        private static (int, int) Add((int, int) a, (int, int) b) => (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }
}