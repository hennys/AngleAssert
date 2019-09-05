using AngleAssert;
using Xunit.Sdk;

namespace Xunit
{
    public partial class Assert
    {
        /// <summary>
        /// Verifies that two HTML fragments are equivalent using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment</param>
        /// <param name="actual">The HTML fragment to be compared against</param>
        /// <exception cref="EqualException">Thrown when the HTML fragments are not equivalent</exception>
        public static void HtmlFragment(string expected, string actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            Equal(expected, actual, HtmlComparer.Fragment);
        }

        /// <summary>
        /// Verifies that two HTML documents are equivalent using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML document</param>
        /// <param name="actual">The HTML document to be compared against</param>
        /// <exception cref="EqualException">Thrown when the HTML documents are not equivalent</exception>

        public static void Html(string expected, string actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            Equal(expected, actual, HtmlComparer.Default);
        }

        /// <summary>
        /// Verifies that two HTML fragments are equivalent using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment</param>
        /// <param name="actual">The HTML fragment to be compared against</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="EqualException">Thrown when the HTML fragments are not equivalent</exception>
        public static void HtmlFragment(string expected, string actual, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            var comparerOptions = new HtmlCompareOptions
            {
                IgnoreAdditionalAttributes = ignoreAdditionalAttributes,
                IgnoreAdditionalClassNames = ignoreAdditionalClassNames,
                IgnoreClassNameOrder = ignoreClassNameOrder,
                TreatHtmlAsFragment = true
            };

            Equal(expected, actual, new HtmlComparer(comparerOptions));
        }

        /// <summary>
        /// Verifies that two HTML documents are equivalent using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML document</param>
        /// <param name="actual">The HTML document to be compared against</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="EqualException">Thrown when the HTML strings are not equivalent</exception>
        public static void Html(string expected, string actual, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            var comparerOptions = new HtmlCompareOptions
            {
                IgnoreAdditionalAttributes = ignoreAdditionalAttributes,
                IgnoreAdditionalClassNames = ignoreAdditionalClassNames,
                IgnoreClassNameOrder = ignoreClassNameOrder
            };

            Equal(expected, actual, new HtmlComparer(comparerOptions));
        }

        /// <summary>
        /// Verifies that a specific element in a HTML document or fragment are equivalent to a given fragment using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment element</param>
        /// <param name="html">The HTML document or fragment from which the element to compare should be selected from.</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <exception cref="HtmlException">Thrown when the HTML elements are not equivalent</exception>
        public static void HtmlElement(string expected, string html, string selector)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(selector), selector);

            var result = HtmlComparer.Fragment.Equals(expected, html, selector);
            if (!result.Matches)
            {
                throw new HtmlException(result, selector);
            }
        }

        /// <summary>
        /// Verifies that a specific element in a HTML document or fragment are equivalent to a given fragment using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML fragment element</param>
        /// <param name="html">The HTML document or fragment from which the element to compare should be selected from.</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <param name="elementComparisonMode">Indicated if the selected element itself should be included in the comparison or if only it's content should be compared.</param>
        /// <param name="elementSelectionMode">Indicates how selected elements should be compared.</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="HtmlException">Thrown when the HTML elements are not equivalent</exception>
        public static void HtmlElement(string expected, string html, string selector, ElementComparisonMode elementComparisonMode = ElementComparisonMode.InnerHtml, ElementSelectionMode elementSelectionMode = ElementSelectionMode.First, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(selector), selector);

            var comparerOptions = new HtmlCompareOptions
            {
                ElementComparisonMode = elementComparisonMode,
                ElementSelectionMode = elementSelectionMode,
                IgnoreAdditionalAttributes = ignoreAdditionalAttributes,
                IgnoreAdditionalClassNames = ignoreAdditionalClassNames,
                IgnoreClassNameOrder = ignoreClassNameOrder,
                TreatHtmlAsFragment = true
            };

            var result = new HtmlComparer(comparerOptions).Equals(expected, html, selector);
            if (!result.Matches)
            {
                throw new HtmlException(result, selector);
            }
        }

        /// <summary>
        /// Checks if the HTML document contains any element matched by the selector.
        /// </summary>
        /// <param name="html">The HTML document that should be asserted.</param>
        /// <param name="selector">The selector used to find an element.</param>
        /// <exception cref="HtmlContainsException">Thrown when the HTML document doesn't contain any element that matches the selector.</exception>
        public static void HtmlContains(string html, string selector)
        {
            GuardArgumentNotNull(nameof(selector), selector);

            if (!HtmlComparer.Default.Contains(html, selector))
            {
                throw new HtmlContainsException(html, selector);
            }
        }

        /// <summary>
        /// Checks if the HTML fragment contains any element matched by the selector.
        /// </summary>
        /// <param name="html">The HTML fragment that should be asserted.</param>
        /// <param name="selector">The selector used to find an element.</param>
        /// <exception cref="HtmlContainsException">Thrown when the HTML fragment doesn't contain any element that matches the selector.</exception>
        public static void HtmlFragmentContains(string html, string selector)
        {
            GuardArgumentNotNull(nameof(selector), selector);

            if (!HtmlComparer.Fragment.Contains(html, selector))
            {
                throw new HtmlContainsException(html, selector);
            }
        }
    }
}
