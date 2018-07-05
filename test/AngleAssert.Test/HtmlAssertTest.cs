using Xunit;
using Xunit.Sdk;

namespace AngleAssert
{
    public class HtmlAssertTest
    {
        [Fact]
        public void Html_WhenHtmlIsIdentical_ShouldNotThrow()
        {
            Assert.Html("<p>text</p>", "<p>text</p>");
        }

        [Fact]
        public void Html_WhenHtmlIsDifferent_ShouldThrow()
        {
            Assert.Throws<EqualException>(() => Assert.Html("<p>one</p>", "<div>two</div>"));
        }

        [Fact]
        public void Html_WhenHtmlIsAcceptablyDifferent_ShouldNotThrow()
        {
            Assert.Html("<p>text</p>", "<p data-attribute=\"value\">text</p>", ignoreAdditionalAttributes: true);
        }

        [Fact]
        public void Html_WhenHtmlElementIsNotFound_ShouldReportReason()
        {
            var ex = Assert.Throws<HtmlException>(() => Assert.Html("one", "<div><p>one</p></div>", "p.class"));

            Assert.Equal(HtmlCompareMismatchReason.ElementNotFound, ex.Reason);
        }

        [Fact]
        public void HtmlElement_WhenHtmlDoesNotContainElement_ShouldThrow()
        {
            Assert.Throws<HtmlElementException>(() => Assert.HtmlElement("<p>one</p>", "div"));
        }
    }
}
