using AngleAssert;
using Xunit.Sdk;

namespace Xunit
{
#if XUNIT_VISIBILITY_INTERNAL
    internal
#else
    public
#endif

    partial class Assert
    {
        /// <summary>
        /// Verifies that two HTML string are equivalent using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML</param>
        /// <param name="actual">The HTML to be compared against</param>
        /// <exception cref="EqualException">Thrown when the HTML strings are not equivalent</exception>

        public static void Html(string expected, string actual)
        {
            GuardArgumentNotNull(nameof(expected), expected);

            Equal(expected, actual, HtmlComparer.Fragment);
        }

        /// <summary>
        /// Verifies that two HTML string are equivalent using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML</param>
        /// <param name="actual">The HTML to be compared against</param>
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
        /// Verifies that two HTML string are equivalent using the default options.
        /// </summary>
        /// <param name="expected">The expected HTML</param>
        /// <param name="html">The HTML from which the element to compare should be selected from.</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <exception cref="HtmlException">Thrown when the HTML strings are not equivalent</exception>
        public static void Html(string expected, string html, string selector)
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
        /// Verifies that two HTML string are equivalent using the provided options.
        /// </summary>
        /// <param name="expected">The expected HTML</param>
        /// <param name="html">The HTML from which the element to compare should be selected from.</param>
        /// <param name="selector">A selector used to find the element to compare.</param>
        /// <param name="elementSelectionMode">Indicates how selected elements should be compared.</param>
        /// <param name="ignoreAdditionalAttributes">Indicates if additional attribute on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreAdditionalClassNames">Indicates if additional class names on any element in the candidate HTML should be ignored.</param>
        /// <param name="ignoreClassNameOrder">Indicates if the order of class names in the candidate HTML should be ignored</param>
        /// <exception cref="HtmlException">Thrown when the HTML strings are not equivalent</exception>
        public static void Html(string expected, string html, string selector, ElementSelectionMode elementSelectionMode = ElementSelectionMode.First, bool ignoreAdditionalAttributes = false, bool ignoreAdditionalClassNames = false, bool ignoreClassNameOrder = true)
        {
            GuardArgumentNotNull(nameof(expected), expected);
            GuardArgumentNotNull(nameof(selector), selector);

            var comparerOptions = new HtmlCompareOptions
            {
                ElementSelectionMode = elementSelectionMode,
                IgnoreAdditionalAttributes = ignoreAdditionalAttributes,
                IgnoreAdditionalClassNames = ignoreAdditionalClassNames,
                IgnoreClassNameOrder = ignoreClassNameOrder
            };

            var result = new HtmlComparer(comparerOptions).Equals(expected, html, selector);
            if (!result.Matches)
            {
                throw new HtmlException(result, selector);
            }
        }

        /// <summary>
        /// Checks if the HTML contains any element matched by the selector.
        /// </summary>
        /// <param name="html">The html that should be asserted.</param>
        /// <param name="selector">The selector used to find an element.</param>
        public static void HtmlElement(string html, string selector)
        {
            GuardArgumentNotNull(nameof(selector), selector);

            if (!HtmlComparer.Fragment.Contains(html, selector))
            {
                throw new HtmlElementException(html, selector);
            }
        }
    }
}
