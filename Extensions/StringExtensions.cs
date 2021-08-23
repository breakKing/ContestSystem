using System.Text.RegularExpressions;

namespace ContestSystem.Extensions
{
    public static class StringExtensions
    {
        public static string StripHTMLTags(this string str)
        {
            return Regex.Replace(str, @"<[^>]+>", "").Trim();
        }
    }
}