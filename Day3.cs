using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day3
    {
        private static readonly IEnumerable<string> Sample = new[]
        {
            "..##.......",
            "#...#...#..",
            ".#....#..#.",
            "..#.#...#.#",
            ".#...##..#.",
            "..#.##.....",
            ".#.#.#....#",
            ".#........#",
            "#.##...#...",
            "#...##....#",
            ".#..#...#.#",
        };

        private static readonly TextParser<bool> TreeParser = Character.In('.', '#').Select(c => c == '#');
        private static readonly TextParser<Line> LineParser = TreeParser.Many().Select(x => new Line(x));

        private readonly ITestOutputHelper _output;

        public Day3(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day3");

            _output.Run("sample", () => CountTreesHit(Sample))
                .Should().Be(7);

            _output.Run("actual", () => CountTreesHit(data));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day3");
            var slopes = new[] {(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)};

            _output.Run("sample", () => CountTreesHitAllSlopes(Sample, slopes))
                .Should().Be(336);

            _output.Run("actual", () => CountTreesHitAllSlopes(data, slopes));
        }

        private static int CountTreesHitAllSlopes(IEnumerable<string> lineStrings, IEnumerable<(int X, int Y)> slopes)
        {
            var lines = lineStrings.Select(x => LineParser.MustParse(x)).ToList();

            var treesHit = slopes.Select(slope => CountTreesHit(lines, slope.X, slope.Y)).ToList();
            return treesHit.Aggregate(1, (a, x) => a * x);
        }

        private static int CountTreesHit(IEnumerable<string> lineStrings, int dx = 3, int dy = 1)
        {
            var lines = lineStrings.Select(x => LineParser.MustParse(x)).ToList();
            return CountTreesHit(lines, dx, dy);
        }

        private static int CountTreesHit(IReadOnlyList<Line> lines, int dx = 3, int dy = 1)
        {
            var treesHit = 0;

            var x = 0;

            for (var y = 0; y < lines.Count; y += dy)
            {
                if (lines[y].HasTreeAtX(x))
                {
                    treesHit++;
                }

                x += dx;
            }

            return treesHit;
        }

        private class Line
        {
            private readonly IReadOnlyList<bool> _trees;

            public Line(IReadOnlyList<bool> trees)
            {
                _trees = trees;
            }

            public bool HasTreeAtX(int x) => _trees[x % _trees.Count];
        }

        private static IReadOnlyCollection<string> LoadData(string fileName) =>
            File.ReadAllLines(Path.Combine("Inputs", fileName));
    }
}