using System;
using System.Globalization;
using AngleAssert;

namespace Xunit.Sdk
{
    /// <summary>
    /// Thrown when the provided HTML didn't match the expected HTML.
    /// </summary>
    public class HtmlException : AssertActualExpectedException
    {
        /// <summary>
        /// Creates a new <see cref="HtmlException"/> instance
        /// </summary>
        public HtmlException(HtmlCompareResult result, string selector)
            : base(result.Expected, result.Actual, $"{nameof(Assert)}.{nameof(Assert.Html)}() Failure")
        {
            Selector = selector;
            Reason = result.Reason;
        }

        /// <summary>
        /// The selector that was used.
        /// </summary>
        public string Selector { get; }

        /// <summary>
        /// The reason for the mismatch.
        /// </summary>
        public HtmlCompareMismatchReason? Reason { get; }

        /// <inheritdoc />
        public override string Message
        {
            get
            {
                if (Reason == HtmlCompareMismatchReason.ElementNotFound)
                {
                    return string.Format(CultureInfo.CurrentCulture, $"{UserMessage}{Environment.NewLine}Not found any element matching selector '{Selector}'");
                }
                if (Reason == HtmlCompareMismatchReason.MultipleElementsFound)
                {
                    return string.Format(CultureInfo.CurrentCulture, $"{UserMessage}{Environment.NewLine}Found more than one element matching selector '{Selector}'.");
                }

                return base.Message;
            }
        }
    }
}
