using System;
using System.Globalization;
using AngleAssert;

namespace Xunit.Sdk
{
#if XUNIT_VISIBILITY_INTERNAL
    internal
#else
    public
#endif
    class HtmlException : AssertActualExpectedException
    {
        public HtmlException(HtmlCompareResult result, string selector)
            : base(result.Expected, result.Actual, $"{nameof(Assert)}.{nameof(Assert.Html)}() Failure")
        {
            Selector = selector;
            Reason = result.Reason;
        }

        public string Selector { get; }

        public HtmlCompareMismatchReason? Reason { get; }

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
