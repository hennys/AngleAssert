using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace AngleAssert
{
    public class HtmlResponseAssertTest
    {
        [Fact]
        public async Task Html_WhenHtmlIsIdentical_ShouldNotThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<p>text</p>") };

            await Assert.Html("<p>text</p>", response);
        }

        [Fact]
        public async Task Html_WhenHtmlIsDifferent_ShouldThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<div>two</div>") };

            await Assert.ThrowsAsync<EqualException>(async () => await Assert.Html("<p>one</p>", response));
        }

        [Fact]
        public async Task Html_WhenHtmlIsAcceptablyDifferent_ShouldNotThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<p class=\"one two\">text</p>") };

            await Assert.Html("<p class=\"one\">text</p>", response, ignoreAdditionalClassNames: true);
        }

        [Fact]
        public async Task Html_WhenResponseStatusIsNotOk_ShouldThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("<div>two</div>") };

            await Assert.ThrowsAsync<HttpRequestException>(async () => await Assert.Html("<p>one</p>", response));
        }

        [Fact]
        public async Task HtmlElement_WhenHtmlElementIsNotFound_ShouldReportReason()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<div><p>one</p></div>") };

            var ex = await Assert.ThrowsAsync<HtmlException>(async () => await Assert.HtmlElement("one", response, "p.class"));

            Assert.Equal(HtmlCompareMismatchReason.ElementNotFound, ex.Reason);
        }

        [Fact]
        public async Task HtmlContains_WhenHtmlDoesNotContainElement_ShouldThrow()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<p>one</p>") };

            await Assert.ThrowsAsync<HtmlContainsException>(async () => await Assert.HtmlContains(response, "div"));
        }
    }
}
