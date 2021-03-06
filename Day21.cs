﻿using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day21 : Test
    {
        private const string Sample = "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)\ntrh fvjkl sbzzf mxmxvkd (contains dairy)\nsqjhc fvjkl (contains soy)\nsqjhc mxmxvkd sbzzf (contains fish)\n";
        
        public Day21(ITestOutputHelper output) : base(21, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart1).Should().Be(5);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample, Tokenizer, Parser, SolvePart2).Should().Be("mxmxvkd,sqjhc,fvjkl");
            Run("actual", Tokenizer, Parser, SolvePart2);
        }

        private static long SolvePart1(IReadOnlyList<Line> input)
        {
            // some or all of the allergens the food contains (allergens aren't always marked)
            // each allergen is found in exactly one ingredient
            // each ingredient contains zero or one allergen
            
            var grouped = GroupByAllergen(input);

            var possibleIngredients = grouped.SelectMany(x => x.Value).ToHashSet();
            var allIngredients = input.SelectMany(x => x.Ingredients).Where(x => !possibleIngredients.Contains(x)).ToList();

            return allIngredients.Count;
        }
        
        private static string SolvePart2(IReadOnlyList<Line> input)
        {
            var assigned = new Dictionary<string, string>();
            var grouped = GroupByAllergen(input);

            while (grouped.Count > 0)
            {
                var singleAssigned = grouped.Where(x => x.Value.Count == 1);
                foreach (var (k, i) in singleAssigned)
                {
                    assigned.Add(k, i.Single());
                    grouped.Remove(k);
                    foreach (var (_, gi) in grouped) gi.RemoveAll(x => x == i.Single());
                }
            }

            return string.Join(",", assigned.OrderBy(x => x.Key).Select(x => x.Value));
        }

        private static Dictionary<string, List<string>> GroupByAllergen(IEnumerable<Line> input)
        {
            return input
                .SelectMany(x => x.Allergens.Select(a => (a, x.Ingredients)))
                .GroupBy(x => x.a)
                .ToDictionary(x => x.Key, x => x.Select(l => l.Ingredients).Aggregate((a, i) => a.Intersect(i).ToList()).ToList());
        }
    }
}