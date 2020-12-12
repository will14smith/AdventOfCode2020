using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day06 : Test
    {
        private static readonly string Sample = "abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb";

        private static readonly TextParser<char> Answer = Character.Lower;
        private static readonly TextParser<Person> PersonAnswer = Answer.AtLeastOnce().Select(xs => new Person(xs));
        private static readonly TextParser<Group> AnswerGroup = PersonAnswer.ThenIgnoreOptional(SuperpowerExtensions.NewLine).AtLeastOnce().Select(ps => new Group(ps));
        private static readonly TextParser<Group[]> AnswerGroups = AnswerGroup.AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine).ThenIgnoreOptional(SuperpowerExtensions.NewLine);

        public Day06(ITestOutputHelper output) : base(6, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, GetUniqueAnswers).Should().Be(11);
            Run("actual", LoadInput(), GetUniqueAnswers);
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, GetDistinctAnswers).Should().Be(6);
            Run("actual", LoadInput(), GetDistinctAnswers);
        }

        private static int GetUniqueAnswers(string data)
        {
            var answerGroups = AnswerGroups.MustParse(data);

            return answerGroups.Sum(g => g.CountUnique());
        }

        private static int GetDistinctAnswers(string data)
        {
            var answerGroups = AnswerGroups.MustParse(data);

            return answerGroups.Sum(g => g.CountAll());
        }

        private class Person
        {
            public ImmutableHashSet<char> Answers { get; }
            public Person(IEnumerable<char> answers) => Answers = answers.ToImmutableHashSet();
        }

        private class Group
        {
            public IReadOnlyCollection<Person> People { get; }
            public Group(IReadOnlyCollection<Person> people) => People = people;

            public int CountUnique() => People.Select(x => x.Answers).Aggregate((a, p) => a.Union(p)).Count;
            public int CountAll() => People.Select(x => x.Answers).Aggregate((a, p) => a.Intersect(p)).Count;
        }
    }
}