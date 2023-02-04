using System.Text.RegularExpressions;

namespace AddLib;

internal class WildcardMatcher
{
    private readonly Regex _regex;

    public WildcardMatcher(string wildcardPattern)
    {
        var regexPattern = Regex.Escape(wildcardPattern).Replace("\\?", ".").Replace("\\*", ".*");

        _regex = new Regex($"^{regexPattern}$", RegexOptions.IgnoreCase);
    }

    public bool IsMatch(string input)
    {
        return _regex.IsMatch(input);
    }
}
