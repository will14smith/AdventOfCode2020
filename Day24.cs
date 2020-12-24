using System;
using System.Collections.Generic;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day24 : Test
    {
        private const string Sample = "sesenwnenenewseeswwswswwnenewsewsw\nneeenesenwnwwswnenewnwwsewnenwseswesw\nseswneswswsenwwnwse\nnwnwneseeswswnenewneswwnewseswneseene\nswweswneswnenwsewnwneneseenw\neesenwseswswnenwswnwnwsewwnwsene\nsewnenenenesenwsewnenwwwse\nwenwwweseeeweswwwnwwe\nwsweesenenewnwwnwsenewsenwwsesesenwne\nneeswseenwwswnwswswnw\nnenwswwsewswnenenewsenwsenwnesesenew\nenewnwewneswsewnwswenweswnenwsenwsw\nsweneswneswneneenwnewenewwneswswnese\nswwesenesewenwneswnwwneseswwne\nenesenwswwswneneswsenwnewswseenwsese\nwnwnesenesenenwwnenwsewesewsesesew\nnenewswnwewswnenesenwnesewesw\neneswnwswnwsenenwnwnwwseeswneewsenese\nneswnwewnwnwseenwseesewsenwsweewe\nwseweeenwnesenwwwswnew\n"; 
        
        public Day24(ITestOutputHelper output) : base(24, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(10);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }
        
        private int SolvePart1(Direction[][] input)
        {
            var tiles = new HashSet<(int, int, int)>();

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

            return tiles.Count;
        }
        
        private static (int, int, int) Calculate(IEnumerable<Direction> line)
        {
            var (x, y, z) = (0, 0, 0);

            foreach (var direction in line)
            {
                (x, y, z) = direction switch
                {
                    Direction.East => (x-1, y+1, z),
                    Direction.SouthEast => (x-1, y, z+1),
                    Direction.SouthWest => (x, y-1, z+1),
                    Direction.West => (x+1, y-1, z),
                    Direction.NorthWest => (x+1, y, z-1),
                    Direction.NorthEast => (x, y+1, z-1),
                };
            }

            return (x, y, z);
        }
    }
}