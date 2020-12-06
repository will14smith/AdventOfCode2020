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
    public class Day6
    {
        private static readonly string Sample = "abc\n\na\nb\nc\n\nab\nac\n\na\na\na\na\n\nb";

        private static readonly TextParser<char> Answer = Character.Lower;
        private static readonly TextParser<HashSet<char>> PersonAnswer = Answer.AtLeastOnce().Select(x => x.ToHashSet());
        private static readonly TextParser<HashSet<char>[]> AnswerGroup = PersonAnswer.Try().AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine);
        private static readonly TextParser<HashSet<char>[][]> AnswerGroups = AnswerGroup.Try().AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine.IgnoreThen(SuperpowerExtensions.NewLine));

        private readonly ITestOutputHelper _output;

        public Day6(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day6");

            _output.Run("sample", () => GetUniqueAnswers(Sample))
                .Should().Be(11);

            _output.Run("actual", () => GetUniqueAnswers(data));
        }

        [Fact]
        public void Part2()
        {
            var data = LoadData("day6");

            _output.Run("sample", () => GetDistinctAnswers(Sample))
                .Should().Be(6);

            _output.Run("actual", () => GetDistinctAnswers(data));
        }

        private static int GetUniqueAnswers(string data)
        {
            var answerGroups = AnswerGroups.MustParse(data);

            return answerGroups.Sum(CountGroup);

            static int CountGroup(HashSet<char>[] people) => people.SelectMany(x => x).ToHashSet().Count;
        }

        private static int GetDistinctAnswers(string data)
        {
            var answerGroups = AnswerGroups.MustParse(data);

            return answerGroups.Sum(CountGroup);

            int CountGroup(HashSet<char>[] people)
            {
                return people.SelectMany(x => x).GroupBy(x => x).Count(x => x.Count() == people.Length);
            }
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}