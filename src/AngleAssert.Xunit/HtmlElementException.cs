using System;

namespace Xunit.Sdk
{
#if XUNIT_VISIBILITY_INTERNAL
    internal
#else
    public
#endif
    class HtmlElementException : XunitException
    {
        public HtmlElementException(string html, string selector)
            : base($"{nameof(Assert)}.{nameof(Assert.HtmlElement)}() Failure")
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
