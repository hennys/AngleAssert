using System;
using Xunit;

namespace AngleAssert
{
    public class HtmlTextComparerTest
    {
        private static StringComparer Comparer => HtmlTextComparer.Ordinal;

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("", "   ")]
        [InlineData(" ", "   ")]
        [InlineData(" ", " \t")]
        [InlineData("\t ", " \t")]
        [InlineData(" some text ", "   some   text   ")]
        public void Equals_WithMatchingTextStrings_ShouldReturnTrue(string expected, string candidate)
        {
            Assert.True(Comparer.Equals(expected, candidate));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", null)]
        [InlineData("text", "te xt")]
        [InlineData("text", " text")]
        [InlineData("text", "text ")]
        [InlineData("some text", " some text ")]
        public void Equals_WithNonMatchingTextStrings_ShouldReturnFalse(string expected, string candidate)
        {
            Assert.False(Comparer.Equals(expected, candidate));
        }

    }
}
