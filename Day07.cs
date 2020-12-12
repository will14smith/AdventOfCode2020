using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day07 : Test
    {
        private const string Sample1 = @"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.";

        private const string Sample2 = @"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.";

        public Day07(ITestOutputHelper output) : base(7, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample1, Tokenizer, Rules, rules => CountOuterBags(rules, "shiny gold")).Should().Be(4);
            Run("actual", Tokenizer, Rules, rules => CountOuterBags(rules, "shiny gold"));
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample2, Tokenizer, Rules, rules => CountInnerBags(rules, "shiny gold")).Should().Be(126);
            Run("actual", Tokenizer, Rules, rules => CountInnerBags(rules, "shiny gold"));
        }

        private static int CountOuterBags(Rule[] rules, string target)
        {
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

        private static int CountInnerBags(Rule[] rules, string target)
        {
            var indexedRules = rules.ToDictionary(x => x.Outer);
            return CountInner(indexedRules, target) - 1;

            static int CountInner(IReadOnlyDictionary<string, Rule> rules, string target)
            {
                return 1 + rules[target].Inner.Sum(x => x.Value * CountInner(rules, x.Key));
            }
        }
    }
}