using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rogero.SchedulingLibrary
{
    public static class RegexExtensions
    {
        public static GroupCollection GetMatches(this string input, string regexPattern)
        {
            var regex = new Regex(regexPattern);
            return input.GetMatches(regex);
        }
        public static GroupCollection GetMatches(this string input, Regex regex)
        {
            var matches = regex.Match(input);
            return matches.Groups;
        }
        public static string MatchRegex(this string input, Regex regex, string groupName)
        {
            var group = input.GetMatches(regex)[groupName];
            if (string.IsNullOrWhiteSpace(group.Value))
                return "";
            else
                return group.Value;
        }

        public static IEnumerable<string> MatchRegex(this IEnumerable<string> inputStrings, string regex, string groupName)
        {
            return inputStrings.Select(input => GetMatches(input, regex)[groupName]).WhereMatchSuccessful();
        }

        public static IEnumerable<string> MatchRegex(this IEnumerable<string> inputStrings, Regex regex, string groupName)
        {
            return inputStrings.Select(input => regex.Match(input).Groups[groupName]).WhereMatchSuccessful();
        }

        public static IEnumerable<string> WhereMatchSuccessful(this IEnumerable<Group> groups)
        {
            foreach (var group in groups)
            {
                if (string.IsNullOrWhiteSpace(group.Value))
                {
                    continue;
                }
                yield return group.Value;
            }
        }
    }
}