using Xunit;
using Xunit.Sdk;

namespace AngleAssert
{
    public class EqualityAssertTest
    {
        [Fact]
        public void Equal_WhenHtmlIsIdentical_ShouldNotThrow()
        {
            Assert.Equal("<p>text</p>", "<p>text</p>", HtmlComparer.Default);
        }

        [Fact]
        public void Equal_WhenHtmlIsDifferent_ShouldThrow()
        {
            Assert.Throws<EqualException>(() => Assert.Equal("<p>one</p>", "<div>two</div>", HtmlComparer.Default));
        }
    }
}
