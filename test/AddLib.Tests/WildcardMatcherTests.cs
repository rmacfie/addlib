using Xunit;

namespace AddLib.Tests;

public class WildcardMatcherTests
{
    [Theory]
    [InlineData("*bar", "bar", true)]
    [InlineData("*bar", "BAR", true)]
    [InlineData("*bar", "foobar", true)]
    [InlineData("*bar", "FOoBaR", true)]
    [InlineData("*bar", "f..bar", true)]
    [InlineData("*bar", "f--bar", true)]
    [InlineData("*bar", "fooba", false)]
    [InlineData("*bar", "foobarr", false)]
    [InlineData("*bar", "foo", false)]
    public void It_handles_leading_asterisk(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }

    [Theory]
    [InlineData("foo*", "foo", true)]
    [InlineData("foo*", "FOO", true)]
    [InlineData("foo*", "foobar", true)]
    [InlineData("foo*", "FOObar", true)]
    [InlineData("foo*", "foo..r", true)]
    [InlineData("foo*", "foo--r", true)]
    [InlineData("foo*", "fobar", false)]
    [InlineData("foo*", "ffoobar", false)]
    [InlineData("foo*", "bar", false)]
    public void It_handles_ending_asterisk(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }

    [Theory]
    [InlineData("f*bar", "fobar", true)]
    [InlineData("f*bar", "FobAr", true)]
    [InlineData("f*bar", "fooobar", true)]
    [InlineData("f*bar", "FOOObar", true)]
    [InlineData("f*bar", "f..bar", true)]
    [InlineData("f*bar", "f->bar", true)]
    [InlineData("f*bar", "fbar", true)]
    [InlineData("f*bar", "foba", false)]
    [InlineData("f*bar", "fooar", false)]
    [InlineData("f*bar", "bfoobar", false)]
    [InlineData("f*bar", "foobarb", false)]
    public void It_handles_middle_asterisk(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }

    [Theory]
    [InlineData("?foo", "bfoo", true)]
    [InlineData("?foo", "bFOO", true)]
    [InlineData("?foo", ".foo", true)]
    [InlineData("?foo", "-foo", true)]
    [InlineData("?foo", "foo", false)]
    [InlineData("?foo", "bfo", false)]
    [InlineData("?foo", "bfoob", false)]
    [InlineData("?foo", "bar", false)]
    public void It_handles_leading_question_mark(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }

    [Theory]
    [InlineData("foo?", "foob", true)]
    [InlineData("foo?", "FOOb", true)]
    [InlineData("foo?", "foo.", true)]
    [InlineData("foo?", "foo-", true)]
    [InlineData("foo?", "foo", false)]
    [InlineData("foo?", "fob", false)]
    [InlineData("foo?", "ffoob", false)]
    [InlineData("foo?", "bar", false)]
    public void It_handles_ending_question_mark(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }

    [Theory]
    [InlineData("f?bar", "fobar", true)]
    [InlineData("f?bar", "FobAr", true)]
    [InlineData("f?bar", "f.bar", true)]
    [InlineData("f?bar", "f>bar", true)]
    [InlineData("f?bar", "fbar", false)]
    [InlineData("f?bar", "foba", false)]
    [InlineData("f?bar", "foar", false)]
    [InlineData("f?bar", "bfobar", false)]
    [InlineData("f?bar", "fobarb", false)]
    public void It_handles_middle_question_mark(string pattern, string input, bool expectedMatch)
    {
        Assert.Equal(expectedMatch, new WildcardMatcher(pattern).IsMatch(input));
    }
}
