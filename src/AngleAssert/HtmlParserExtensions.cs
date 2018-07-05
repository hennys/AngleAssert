using System;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace AngleAssert
{
    internal static class HtmlParserExtensions
    {
        public static IHtmlElement ParseElements(this HtmlParser parser, string source)
        {
            var doc = parser.Parse(source);

            if (doc.Head.HasChildNodes && parser.Options.IsStrictMode)
            {
                throw new ArgumentException("Source cannot contain <HEAD> elements.", nameof(source));
            }

            return doc.Body;
        }
    }
}
