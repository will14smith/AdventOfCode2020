namespace AdventOfCode2020.Utilities
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char c) => (c >= '0' && c <= '9');
        public static bool IsHexChar(this char c) => c.IsDigit() || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }
}