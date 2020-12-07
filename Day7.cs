using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day7
    {
        private static readonly string Sample1 = @"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.";

        private static readonly string Sample2 = @"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.";

        private readonly ITestOutputHelper _output;

        public Day7(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day7");

            _output.Run("sample", () => CountOuterBags(Sample1, "shiny gold"))
                .Should().Be(4);

            _output.Run("actual", () => CountOuterBags(data, "shiny gold"));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day7");

            _output.Run("sample", () => CountInnerBags(Sample2, "shiny gold"))
                .Should().Be(126);

            _output.Run("actual", () => CountInnerBags(data, "shiny gold"));
        }

        private static int CountOuterBags(string data, string target)
        {
            var rules = Rules.MustParse(Tokenizer, data);

            var outers = new HashSet<string>();
            var workList = new Queue<string>();

            outers.Add(target);
            workList.Enqueue(target);

            while (workList.Count > 0)
            {
                var bag = workList.Dequeue();

                var outerRules = rules.Where(rule => rule.CanContain(bag));
                foreach (var rule in outerRules)
                {
                    if (outers.Add(rule.Outer))
                    {
                        workList.Enqueue(rule.Outer);
                    }
                }
            }

            // ignore the original target
            return outers.Count - 1;
        }

        private static int CountInnerBags(string data, string target)
        {
            var rules = Rules.MustParse(Tokenizer, data).ToDictionary(x => x.Outer);
            return CountInner(rules, target) - 1;

            static int CountInner(IReadOnlyDictionary<string, Rule> rules, string target)
            {
                return 1 + rules[target].Inner.Sum(x => x.Value * CountInner(rules, x.Key));
            }
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}