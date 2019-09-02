using System.Net.Http;
using System.Threading.Tasks;
using AngleAssert;
using Xunit.Sdk;

namespace Xunit
{
    public partial class Assert
    {
        private static async Task<string> GetHtml(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Verifies that the HTML returned in a <see cref="HttpResponseMessage"/> is equivalent to the expected HTML using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML document</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the HTML to be compared against</param>
        /// <exception cref="EqualException">Thrown when the HTML documents are not equivalent</exception>

        public static async Task Html(string expected, HttpResponseMessage response)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(response), response);

            Html(expected, await GetHtml(response));
        }

        /// <summary>
        /// Verifies that the HTML returned in a <see cref="HttpResponseMessage"/> is equivalent to the expected HTML using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML document</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the HTML to be compared against</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="EqualException">Thrown when the HTML strings are not equivalent</exception>
        public static async Task Html(string expected, HttpResponseMessage response, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(response), response);

            Html(expected, await GetHtml(response), ignoreAdditionalAttributes, ignoreAdditionalClassNames, ignoreClassNameOrder);
        }

        /// <summary>
        /// Verifies that a specific element in a HTML document returned in a <see cref="HttpResponseMessage"/>  is equivalent to a given fragment using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment element</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the HTML to be compared against</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <exception cref="HtmlException">Thrown when the HTML elements are not equivalent</exception>
        public static async Task HtmlElement(string expected, HttpResponseMessage response, string selector)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(response), response);
            GuardArgumentNotNull(nameof(selector), selector);

            HtmlElement(expected, await GetHtml(response), selector);
        }

        /// <summary>
        /// Verifies that a specific element in a HTML document returned in a <see cref="HttpResponseMessage"/>  is equivalent to a given fragment using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment element</param>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the HTML to be compared against</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <param name="includeSelectedElement">Indicated if the selected element itself should be included in the comparison or if only it's content should be compared.</param>
        /// <param name="elementSelectionMode">Indicates how selected elements should be compared.</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="HtmlException">Thrown when the HTML elements are not equivalent</exception>
        public static async Task HtmlElement(string expected, HttpResponseMessage response, string selector, bool includeSelectedElement = false, ElementSelectionMode elementSelectionMode = ElementSelectionMode.First, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(response), response);
            GuardArgumentNotNull(nameof(selector), selector);

            HtmlElement(expected, await GetHtml(response), selector, includeSelectedElement, elementSelectionMode, ignoreAdditionalAttributes, ignoreAdditionalClassNames, ignoreClassNameOrder);
        }

        /// <summary>
        /// Checks if the HTML document returned in the <see cref="HttpResponseMessage"/> contains any element matched by the selector.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the HTML that should be asserted.</param>
        /// <param name="selector">The selector used to find an element.</param>
        public static async Task HtmlContains(HttpResponseMessage response, string selector)
        {
            GuardArgumentNotNull(nameof(response), response);
            GuardArgumentNotNull(nameof(selector), selector);

            HtmlContains(await GetHtml(response), selector);
        }
    }
}
