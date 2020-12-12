using System;
using System.IO;
using Superpower;
using Xunit.Abstractions;

namespace AdventOfCode2020.Utilities
{
    public abstract class Test
    {
        public int Day { get; }
        protected ITestOutputHelper Output { get; }

        protected Test(int day, ITestOutputHelper output)
        {
            Day = day;
            Output = output;
        }

        protected T Run<T>(string name, Func<T> fn)
        {
            Output.WriteLine($"[*] Calculating for {name} data");
            var result = fn();
            Output.WriteLine($"[*] Result = {result}");
            return result;
        }

        protected TResult Run<TModel, TResult>(string name, TModel model, Func<TModel, TResult> fn) => Run(name, () => fn(model));
        protected TResult Run<TModel, TResult>(string name, string input, Func<string, TModel> parseFn, Func<TModel, TResult> fn) => Run(name, parseFn(input), fn);
        protected TResult Run<TModel, TResult>(string name, string input, TextParser<TModel> parser, Func<TModel, TResult> fn) => Run(name, parser.MustParse(input), fn);
        protected TResult Run<TModel, TToken, TResult>(string name, string input, Tokenizer<TToken> tokenizer, TokenListParser<TToken, TModel> parser, Func<TModel, TResult> fn) => Run(name, parser.MustParse(tokenizer, input), fn);

        protected TResult Run<TModel, TResult>(string name, Func<string, TModel> parseFn, Func<TModel, TResult> fn) => Run(name, parseFn(LoadInput()), fn);
        protected TResult Run<TModel, TResult>(string name, TextParser<TModel> parser, Func<TModel, TResult> fn) => Run(name, parser.MustParse(LoadInput()), fn);
        protected TResult Run<TModel, TToken, TResult>(string name, Tokenizer<TToken> tokenizer, TokenListParser<TToken, TModel> parser, Func<TModel, TResult> fn) => Run(name, parser.MustParse(tokenizer, LoadInput()), fn);

        protected string LoadInput() => File.ReadAllText(Path.Combine("Inputs", $"day{Day}"));
        protected byte[] LoadRawInput() => File.ReadAllBytes(Path.Combine("Inputs", $"day{Day}"));
        protected string[] LoadInputLines() => File.ReadAllLines(Path.Combine("Inputs", $"day{Day}"));
    }

}