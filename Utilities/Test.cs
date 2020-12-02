using System;
using Xunit.Abstractions;

namespace AdventOfCode2020.Utilities
{
    public static class Test
    {
        public static T Run<T>(this ITestOutputHelper output, string name, Func<T> fn)
        {
            output.WriteLine($"[*] Calculating for {name} data");
            var result = fn();
            output.WriteLine($"[*] Result = {result}");
            return result;
        }
    }
}