using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day20 : Test
    {
        private const string Sample = "Tile 2311:\n..##.#..#.\n##..#.....\n#...##..#.\n####.#...#\n##.##.###.\n##...#.###\n.#.#.#..##\n..#....#..\n###...#.#.\n..###..###\n\nTile 1951:\n#.##...##.\n#.####...#\n.....#..##\n#...######\n.##.#....#\n.###.#####\n###.##.##.\n.###....#.\n..#.#..#.#\n#...##.#..\n\nTile 1171:\n####...##.\n#..##.#..#\n##.#..#.#.\n.###.####.\n..###.####\n.##....##.\n.#...####.\n#.##.####.\n####..#...\n.....##...\n\nTile 1427:\n###.##.#..\n.#..#.##..\n.#.##.#..#\n#.#.#.##.#\n....#...##\n...##..##.\n...#.#####\n.#.####.#.\n..#..###.#\n..##.#..#.\n\nTile 1489:\n##.#.#....\n..##...#..\n.##..##...\n..#...#...\n#####...#.\n#..#.#.#.#\n...#.#.#..\n##.#...##.\n..##.##.##\n###.##.#..\n\nTile 2473:\n#....####.\n#..#.##...\n#.##..#...\n######.#.#\n.#...#.#.#\n.#########\n.###.#..#.\n########.#\n##...##.#.\n..###.#.#.\n\nTile 2971:\n..#.#....#\n#...###...\n#.#.###...\n##.##..#..\n.#####..##\n.#..####.#\n#..#.#..#.\n..####.###\n..#.#.###.\n...#.#.#.#\n\nTile 2729:\n...#.#.#.#\n####.#....\n..#.#.....\n....#..#.#\n.##..##.#.\n.#.####...\n####.#.#..\n##.####...\n##..#.##..\n#.##...##.\n\nTile 3079:\n#.#.#####.\n.#..######\n..#.......\n######....\n####.#..#.\n.#...#.##.\n#.#####.##\n..#.###...\n..#.......\n..#.###...\n\n";

        /*
         * ---0---
         * |      |
         * 3      1
         * |      |
         * ---2---|
         */
        

        public Day20(ITestOutputHelper output) : base(20, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(20899048083289);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }

        private static long SolvePart1(Model input)
        {
            var potentialMatches = new ConcurrentDictionary<ushort, List<long>>();

            foreach (var (_, tile) in input.Tiles)
            {
                foreach (var edge in GetEdges(new TransformedTile(tile, Flip.None, Rotate.CW0)))
                {
                    potentialMatches.GetOrAdd(EdgeToNum(edge), _ => new List<long>()).Add(tile.Id);
                    potentialMatches.GetOrAdd(EdgeToNum(edge.Reverse().ToList()), _ => new List<long>()).Add(tile.Id);
                }
            }

            var pairs = potentialMatches.Where(x => x.Value.Count > 1).Select(x => x.Value).Select(x =>
            {
                if (x.Count != 2) throw new InvalidOperationException();

                return (x[0], x[1]);
            }).ToList();

            var neighbours = input.Tiles.ToDictionary(x => x.Key, x => pairs.Count(p => p.Item1 == x.Key || p.Item2 == x.Key));

            var corners = neighbours.Where(x => x.Value == 4).Select(x => x.Key).ToList();
            if (corners.Count != 4) throw new InvalidOperationException();

            return corners.Aggregate((a, x) => a * x);
        }

        private static IEnumerable<TransformedTile> TransformTile(Tile tile) => Enum.GetValues<Flip>().SelectMany(_ => Enum.GetValues<Rotate>(), (flip, rotate) => new TransformedTile(tile, flip, rotate));
        private static ushort EdgeToNum(IReadOnlyList<bool> edge)
        {
            if (edge.Count > 16) throw new ArgumentOutOfRangeException(nameof(edge));

            ushort num = 0;

            for (var i = 0; i < edge.Count; i++)
            {
                num = (ushort) (num | (edge[i] ? 1 << i : 0));
            }
            
            return num;
        }
        
        private static IEnumerable<IReadOnlyList<bool>> GetEdges(TransformedTile tile)
        {
            var edges = new IReadOnlyList<bool>[4];

            for (var i = 0; i < edges.Length; i++)
            {
                edges[i] = tile.GetEdge(i);
            }
            
            return edges;
        }

        private record TransformedTile(Tile Tile, Flip Flip, Rotate Rotate)
        {
            public IReadOnlyList<bool> GetEdge(int edgeId)
            {
                bool shouldReverse;
                (edgeId, shouldReverse) = GetTransformedEdge(edgeId);
                
                var (_, image) = Tile;
                var isH = edgeId is 0 or 2;
                var isM = edgeId is 0 or 3;
                var edge = new bool[isH ? image.GetLength(0) : image.GetLength(1)];

                for (var i = 0; i < edge.Length; i++)
                {
                    var (x, y) = isH ? (i, isM ? 0 : edge.Length - 1) : (isM ? 0 : edge.Length - 1, i);
                    edge[shouldReverse ? edge.Length - i - 1 : i] = image[y, x];
                }
            
                return edge;
            }

            private static readonly IReadOnlyDictionary<Flip, int[]> EdgeOrder = new Dictionary<Flip, int[]>
            {
                { Flip.None, new []{ 0, 1, 2, 3 } },
                { Flip.FlipX, new []{ 2, 1, 0, 3 } },
                { Flip.FlipY, new []{ 0, 3, 2, 1 } },
            };
            private static readonly IReadOnlyDictionary<Rotate, int> RotateOffset = new Dictionary<Rotate, int>
            {
                { Rotate.CW0, 0 },
                { Rotate.CW90, 1 },
                { Rotate.CW180, 2 },
                { Rotate.CW270, 3 },
            };
            private (int TransformedEdgeId, bool NeedsReversed) GetTransformedEdge(int edgeId)
            {
                var transformedEdgeId = EdgeOrder[Flip][(edgeId + RotateOffset[Rotate]) % 4];

                var isH = edgeId is 0 or 2;
                var reverse = isH ? Rotate is Rotate.CW90 or Rotate.CW180 : Rotate is Rotate.CW180 or Rotate.CW270;
                reverse = isH && Flip == Flip.FlipY || !isH && Flip == Flip.FlipX ? !reverse : reverse;
                
                return (transformedEdgeId, reverse);
            }
        }
        private enum Flip
        {
            None,
            FlipX,
            FlipY,
        }  
        private enum Rotate
        {
            CW0,
            CW90,
            CW180,
            CW270
        }
    }
}