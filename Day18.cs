using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2020.Utilities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode2020
{
    public partial class Day18 : Test
    {
        public Day18(ITestOutputHelper output) : base(18, output) { }
        
        [Fact]
        public void Part1()
        {
            Run("sample", "2 * 3 + (4 * 5)", Tokenizer, ExpressionsPart1, Solve).Should().Be(26);
            Run("sample", "2 * 3 + (4 * 5)\n2 * 3 + (4 * 5)\n", Tokenizer, ExpressionsPart1, Solve).Should().Be(2*26);
            Run("sample", "5 + (8 * 3 + 9 + 3 * 4 * 3)", Tokenizer, ExpressionsPart1, Solve).Should().Be(437);
            Run("sample", "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", Tokenizer, ExpressionsPart1, Solve).Should().Be(12240);
            Run("sample", "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", Tokenizer, ExpressionsPart1, Solve).Should().Be(13632);
            Run("actual", Tokenizer, ExpressionsPart1, Solve);
        }   
        
        [Fact]
        public void Part2()
        {
            Run("sample", "1 + (2 * 3) + (4 * (5 + 6))", Tokenizer, ExpressionsPart2, Solve).Should().Be(51);
            Run("sample", "2 * 3 + (4 * 5)", Tokenizer, ExpressionsPart2, Solve).Should().Be(46);
            Run("sample", "5 + (8 * 3 + 9 + 3 * 4 * 3)", Tokenizer, ExpressionsPart2, Solve).Should().Be(1445);
            Run("sample", "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", Tokenizer, ExpressionsPart2, Solve).Should().Be(669060);
            Run("sample", "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", Tokenizer, ExpressionsPart2, Solve).Should().Be(23340);
            Run("actual", Tokenizer, ExpressionsPart2, Solve);
        }
        
        private static long Solve(IEnumerable<Expr> arg) => arg.Select(Evaluate).Sum();
        private static long Evaluate(Expr expr) =>
            expr switch
            {
                Expr.Number number => number.Value,
                Expr.Plus plus => Evaluate(plus.Left) + Evaluate(plus.Right),
                Expr.Star star => Evaluate(star.Left) * Evaluate(star.Right),

                _ => throw new ArgumentOutOfRangeException(nameof(expr))
            };
    }
}