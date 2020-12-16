using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day16 : Test
    {
        private const string Sample1 = "class: 1-3 or 5-7\nrow: 6-11 or 33-44\nseat: 13-40 or 45-50\n\nyour ticket:\n7,1,14\n\nnearby tickets:\n7,3,47\n40,4,50\n55,2,20\n38,6,12\n";
        private const string Sample2 = "class: 0-1 or 4-19\nrow: 0-5 or 8-19\nseat: 0-13 or 16-19\n\nyour ticket:\n11,12,13\n\nnearby tickets:\n3,9,18\n15,1,5\n5,14,9\n";
        
        public Day16(ITestOutputHelper output) : base(16, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample1, Tokenizer, Parser, SolvePart1).Should().Be(71);
            Run("actual", Tokenizer, Parser, SolvePart1);
        }     
        
        [Fact]
        public void Part2()
        {
            Run("sample", Sample2, Tokenizer, Parser, SolvePart2Assignments).Should().BeEquivalentTo(new Dictionary<string, int> { {"row", 0}, {"class", 1}, {"seat", 2} });
            Run("actual", Tokenizer, Parser, SolvePart2);
        }
        
        private static long SolvePart1(Spec spec) =>
            spec.Nearby.SelectMany(ticket => ticket.Values)
                .Where(field =>!IsValid(spec.Rules, field))
                .Sum();

        private static long SolvePart2(Spec spec) => 
            SolvePart2Assignments(spec)
                .Where(x => x.Key.StartsWith("departure"))
                .Aggregate(1L, (a, b) => a * spec.Your.Values[b.Value]);

        private static IReadOnlyDictionary<string, int> SolvePart2Assignments(Spec spec)
        {
            var (rules, your, tickets) = spec;
            var validTickets = tickets.Where(ticket => IsValid(rules, ticket)).Prepend(your).ToList();

            var ruleCandidates = rules.ToDictionary(rule => rule.Name, rule => ValidAssignments(validTickets, rule).ToList());

            var assignments = new Dictionary<string, int>();
            var used = new HashSet<int>();
            
            while (ruleCandidates.Count > 0)
            {
                var (rule, candidates) = ruleCandidates.Select(x => (Rule: x.Key, Candidates: StillValidAssignments(x.Value))).First(x => x.Candidates.Count == 1);
                
                var index = candidates[0];
                assignments[rule] = index;
                used.Add(index);
                        
                ruleCandidates.Remove(rule);
            }

            return assignments;

            IReadOnlyList<int> StillValidAssignments(IEnumerable<int> assignments) => assignments.Where(i => !used.Contains(i)).ToList(); 
        }
        
        private static bool IsValid(IEnumerable<Rule> rules, Ticket ticket) => ticket.Values.All(value => IsValid(rules, value));

        private static bool IsValid(IEnumerable<Rule> rules, long value) => rules.Any(rule => IsValid(rule, value));
        private static bool IsValid(Rule rule, long value) => rule.ValidRanges.Any(range => range.Item1 <= value && value <= range.Item2);

        private static IEnumerable<int> ValidAssignments(IReadOnlyList<Ticket> tickets, Rule rule) => Enumerable.Range(0, tickets[0].Values.Count).Where(i => AssignmentIsValid(tickets, rule, i));
        private static bool AssignmentIsValid(IEnumerable<Ticket> tickets, Rule rule, int index) => tickets.Select(ticket => ticket.Values[index]).All(value => IsValid(rule, value));
    }
}