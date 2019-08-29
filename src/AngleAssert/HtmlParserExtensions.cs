using System;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AngleAssert
{
    internal static class HtmlParserExtensions
    {
        public static IHtmlElement ParseElements(this IHtmlParser parser, string source)
        {
            return parser.ParseDocument(source).Body;
        }
    }
}
