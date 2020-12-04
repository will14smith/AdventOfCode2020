using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day4
    {
        private static readonly string Sample = @"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in";

        private static readonly TextParser<string> FieldName = Character.Lower.AtLeastOnce().Select(x => new string(x));
        private static readonly TextParser<string> FieldValue = Character.Except(char.IsWhiteSpace, "Non-whitespace char").AtLeastOnce().Select(x => new string(x));
        private static readonly TextParser<(string Name, string Value)> Field =
            from name in FieldName
            from _ in Character.EqualTo(':')
            from value in FieldValue
            select (name, value);
        private static readonly TextParser<string> FieldSep = Character.EqualTo(' ').Select(_ => " ").Or(SuperpowerExtensions.NewLine);
        private static readonly TextParser<Passport> PassportParser = Field.Try().AtLeastOnceDelimitedBy(FieldSep)
            .Select(xs => xs.ToDictionary(x => x.Name, x => x.Value))
            .Select(x => new Passport(x));
        private static readonly TextParser<string> PassportSep = SuperpowerExtensions.NewLine.Then(_ => SuperpowerExtensions.NewLine);
        private static readonly TextParser<Passport[]> PassportsParser = PassportParser.ManyDelimitedBy(PassportSep);

        private readonly ITestOutputHelper _output;

        public Day4(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Part1()
        {
            var data = LoadData("day4");

            _output.Run("sample", () => CountValidPassports(Sample))
                .Should().Be(2);

            _output.Run("actual", () => CountValidPassports(data));
        }

        private static int CountValidPassports(string input)
        {
            var passports = PassportsParser.MustParse(input);
            return passports.Count(x => x.IsValid());
        }

        private class Passport
        {
            private static readonly HashSet<string> RequiredFields = new HashSet<string>
            {
                "byr",
                "iyr",
                "eyr",
                "hgt",
                "hcl",
                "ecl",
                "pid",
                // "cid",
            };

            private readonly IReadOnlyDictionary<string, string> _fields;

            public Passport(IReadOnlyDictionary<string, string> fields)
            {
                _fields = fields;
            }

            public bool IsValid()
            {
                foreach (var fieldName in RequiredFields)
                {
                    if (!_fields.ContainsKey(fieldName))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static string LoadData(string fileName) =>
            File.ReadAllText(Path.Combine("Inputs", fileName));
    }
}