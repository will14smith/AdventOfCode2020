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
        private static readonly TextParser<char[]> PersonAnswer = Answer.AtLeastOnce();
        private static readonly TextParser<HashSet<char>> AnswerGroup = PersonAnswer.Try().AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine).Select(x => x.SelectMany(x => x).ToHashSet());
        private static readonly TextParser<HashSet<char>[]> AnswerGroups = AnswerGroup.Try().AtLeastOnceDelimitedBy(SuperpowerExtensions.NewLine.IgnoreThen(SuperpowerExtensions.NewLine));

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

        private static int GetUniqueAnswers(string data)
        {
            var answerGroups = AnswerGroups.MustParse(data);
            return answerGroups.Sum(x => x.Count);
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}