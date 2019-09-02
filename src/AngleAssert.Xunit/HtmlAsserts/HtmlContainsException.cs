using System;

namespace Xunit.Sdk
{
    /// <summary>
    /// Thrown when a HTML Element was expected, but couldn't be found.
    /// </summary>
    public sealed class HtmlContainsException : XunitException
    {
        /// <summary>
        /// Creates a new <see cref="HtmlContainsException"/> instance
        /// </summary>
        public HtmlContainsException(string html, string selector)
            : base($"{nameof(Assert)}.{nameof(Assert.HtmlContains)}() Failure")
        {
            Html = html;
            Selector = selector;
        }

        /// <summary>
        /// Gets the actual HTML value.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Gets the expected element selector.
        /// </summary>
        public string Selector { get; }

        /// <inheritdoc />
        public override string Message => $"{UserMessage}{Environment.NewLine}Expected element with selector '{Selector}'.{Environment.NewLine}Actual HTML: {Html}";
    }
}
