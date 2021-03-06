﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Superpower;
using Superpower.Parsers;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public class Day04 : Test
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

        public Day04(ITestOutputHelper output) : base(4, output) { }

        [Fact]
        public void Part1()
        {
            Run("sample", Sample, CountValidPassports1).Should().Be(2);
            Run("actual", LoadInput(), CountValidPassports1);
        }

        [Fact]
        public void Part2()
        {
            Run("sample", Sample, CountValidPassports2).Should().Be(2);
            Run("actual", LoadInput(), CountValidPassports2);
        }

        private static int CountValidPassports1(string input)
        {
            var passports = PassportsParser.MustParse(input);
            return passports.Count(x => x.HasRequiredFields());
        }

        private static int CountValidPassports2(string input)
        {
            var passports = PassportsParser.MustParse(input);
            return passports.Count(x => x.IsValid());
        }

        private class Passport
        {
            private static readonly ISet<string> ValidEyeColours = new HashSet<string> { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            private static readonly IReadOnlyDictionary<string, Func<string, bool>> RequiredFieldValidators = new Dictionary<string, Func<string, bool>>
            {
                { "byr", s => int.TryParse(s, out var x) && x >= 1920 && x <= 2002 },
                { "iyr", s => int.TryParse(s, out var x) && x >= 2010 && x <= 2020 },
                { "eyr", s => int.TryParse(s, out var x) && x >= 2020 && x <= 2030 },
                { "hgt", ValidateHeight },
                { "hcl", s => s.Length == 7 && s[0] == '#' && s.Skip(1).All(x => x.IsHexChar()) },
                { "ecl", s => ValidEyeColours.Contains(s) },
                { "pid", s => s.Length == 9 && s.All(x => x.IsDigit()) },
                // "cid",
            };

            private readonly IReadOnlyDictionary<string, string> _fields;

            public Passport(IReadOnlyDictionary<string, string> fields)
            {
                _fields = fields;
            }

            public bool HasRequiredFields()
            {
                foreach (var fieldName in RequiredFieldValidators.Keys)
                {
                    if (!_fields.ContainsKey(fieldName))
                    {
                        return false;
                    }
                }

                return true;
            }
            public bool IsValid()
            {
                foreach (var (fieldName, validator) in RequiredFieldValidators)
                {
                    if (!_fields.TryGetValue(fieldName, out var fieldValue))
                    {
                        return false;
                    }

                    if (!validator(fieldValue))
                    {
                        return false;
                    }
                }

                return true;
            }

            private static bool ValidateHeight(string s)
            {
                if(!int.TryParse(s[..^2], out var num))
                {
                    return false;
                }

                var units = s[^2..];
                return units switch
                {
                    "cm" => num >= 150 && num <= 193,
                    "in" => num >= 59 && num <= 76,
                    _ => false,
                };
            }
        }
    }
}