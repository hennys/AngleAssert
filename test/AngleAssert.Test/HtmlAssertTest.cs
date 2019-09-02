using Xunit;
using Xunit.Sdk;

namespace AngleAssert
{
    public class HtmlAssertTest
    {
        [Fact]
        public void HtmlFragment_WhenHtmlIsIdentical_ShouldNotThrow()
        {
            Assert.HtmlFragment("<p>text</p>", "<p>text</p>");
        }

        [Fact]
        public void HtmlFragment_WhenHtmlIsDifferent_ShouldThrow()
        {
            Assert.Throws<EqualException>(() => Assert.HtmlFragment("<p>one</p>", "<div>two</div>"));
        }

        [Fact]
        public void HtmlFragment_WhenHtmlIsAcceptablyDifferent_ShouldNotThrow()
        {
            Assert.HtmlFragment("<p>text</p>", "<p data-attribute=\"value\">text</p>", ignoreAdditionalAttributes: true);
        }

        [Fact]
        public void HtmlElement_WhenHtmlElementIsNotFound_ShouldReportReason()
        {
            var ex = Assert.Throws<HtmlException>(() => Assert.HtmlElement("one", "<div><p>one</p></div>", "p.class"));

            Assert.Equal(HtmlCompareMismatchReason.ElementNotFound, ex.Reason);
        }

        [Fact]
        public void HtmlContains_WhenHtmlDoesNotContainElement_ShouldThrow()
        {
            Assert.Throws<HtmlContainsException>(() => Assert.HtmlContains("<p>one</p>", "div"));
        }
    }
}
