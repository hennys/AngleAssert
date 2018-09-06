using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace AngleAssert
{
    /// <summary>
    /// Class that will compare two HTML strings and check if the they would be considered equal.
    /// </summary>
    public class HtmlComparer : IEqualityComparer<string>
    {
        private const string IdAttributeName = "id";
        private const string ClassAttributeName = "class";
        // Keep private as it is a mutable class
        private static readonly HtmlCompareOptions DefaultOptions = new HtmlCompareOptions();
        private readonly HtmlCompareOptions _options;
        private readonly HtmlParser _parser;

        /// <summary>
        /// Gets a <see cref="HtmlComparer"/> that uses the default <see cref="HtmlCompareOptions"/>.
        /// </summary>
        public static HtmlComparer Default { get; } = new HtmlComparer(DefaultOptions);

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlComparer"/> class using the default options.
        /// </summary>
        public HtmlComparer() : this(DefaultOptions) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlComparer"/> class with a specific set of options.
        /// </summary>
        public HtmlComparer(HtmlCompareOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _parser = new HtmlParser();
        }

        /// <summary>
        /// Checks if the provided HTML string contains an element represented by a selector.
        /// </summary>
        /// <param name="html">A string representing the HTML that should be checked.</param>
        /// <param name="selector">The CSS selector that should be used to find a matching element.</param>
        /// <param name="selectionMode">The selection mode that should be used.</param>
        /// <returns><c>True</c> if an element that matched the selector was found; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Set <paramref name="selectionMode"/> to to <see cref="ElementSelectionMode.Single"/> if
        /// you want this method to return <c>false</c> in case multple matching element are found.
        /// </remarks>
        public bool Contains(string html, string selector, ElementSelectionMode selectionMode = ElementSelectionMode.First)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentException("Selector cannot be empty.", nameof(selector));

            if (string.IsNullOrWhiteSpace(html)) return false;

            var bodyElement = _parser.ParseElements(html);

            if (selectionMode == ElementSelectionMode.Single)
            {
                return bodyElement.QuerySelectorAll(selector).Length == 1;
            }

            return bodyElement.QuerySelector(selector) != null;
        }

        /// <summary>
        /// Checks if element in the provided HTML string matches an expected HTML string.
        /// </summary>
        /// <param name="expected">A string represeting the expected HTML that the <paramref name="html"/> should be compared against.</param>
        /// <param name="html">A string representing the HTML that contains the element that should be checked.</param>
        /// <param name="selector">The CSS selector that should be used to find a matching element.</param>
        /// <param name="selectionMode">The selection mode that should be used.</param>
        /// <returns><c>True</c> if the selected element matched the expected html; otherwise <c>false</c>.</returns>
        public HtmlCompareResult Equals(string expected, string html, string selector, ElementSelectionMode selectionMode = ElementSelectionMode.First)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentException("Selector cannot be empty.", nameof(selector));

            var bodyElement = _parser.ParseElements(html);

            // Default selection mode will only need the first element
            if (selectionMode == ElementSelectionMode.First)
            {
                var element = bodyElement.QuerySelector(selector);
                return element == null ? HtmlCompareResult.ElementNotFound : Equals(expected, element);
            }

            // All other selection modes needs all elements
            var elements = bodyElement.QuerySelectorAll(selector);
            if (elements.Length == 0)
            {
                return HtmlCompareResult.ElementNotFound;
            }

            if (selectionMode == ElementSelectionMode.Single)
            {
                if (elements.Length > 1)
                {
                    return HtmlCompareResult.Mismatch(reason: HtmlCompareMismatchReason.MultipleElementsFound);
                }

                return Equals(expected, elements[0]);
            }

            var results = elements.Select(el => Equals(expected, el));

            if (selectionMode == ElementSelectionMode.All)
            {
                return results.FirstOrDefault(r => !r.Matches) ?? HtmlCompareResult.Match;
            }

            if (selectionMode == ElementSelectionMode.Any)
            {
                return results.FirstOrDefault(r => r.Matches) ?? results.First();
            }

            throw new InvalidOperationException("Unknown element selection mode encountered");
        }

        /// <summary>
        /// Checks if the provided HTML string matches an expected HTML string.
        /// </summary>
        /// <param name="expected">A string represeting the expected HTML that the <paramref name="html"/> should be compared against.</param>
        /// <param name="html">A string representing the HTML that should be checked.</param>
        /// <returns><c>True</c> if the selected element matched the expected html; otherwise <c>false</c>.</returns>
        public HtmlCompareResult Equals(string expected, string html)
        {
            if (string.IsNullOrWhiteSpace(expected))
            {
                return string.IsNullOrWhiteSpace(html) ? HtmlCompareResult.Match : HtmlCompareResult.Mismatch(expected, html);
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                return HtmlCompareResult.Mismatch(expected, html);
            }

            if (StringComparer.Ordinal.Equals(expected.Trim(), html.Trim()))
            {
                return HtmlCompareResult.Match;
            }

            var parentElement = _parser.ParseElements(html);

            return Equals(expected, parentElement);
        }

        private HtmlCompareResult Equals(string expected, IElement element)
        {
            var expectedParentElement = _parser.ParseElements(expected);

            if (_options.IncludeSelectedElement)
            {
                if (expectedParentElement.ChildElementCount > 1)
                {
                    return HtmlCompareResult.Mismatch(expected, element.OuterHtml);
                }

                return ElementsAreEqual(expectedParentElement.FirstElementChild, element) ? HtmlCompareResult.Match : HtmlCompareResult.Mismatch(expected, element.OuterHtml);
            }

            return ElementsAreEqual(expectedParentElement.ChildNodes, element.ChildNodes) ? HtmlCompareResult.Match : HtmlCompareResult.Mismatch(expected, element.InnerHtml);
        }

        private bool ElementsAreEqual(INodeList listX, INodeList listY)
        {
            foreach ((var nodeX, var nodeY) in ZipAll(listX, listY, NotIsComment))
            {
                if (!ElementsAreEqual(nodeX, nodeY))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ElementsAreEqual(INode nodeX, INode nodeY)
        {
            if (nodeX?.NodeType != nodeY?.NodeType)
            {
                return false;
            }

            if (nodeX.NodeType == NodeType.Text)
            {
                // Text nodes don't have any children
                return _options.TextComparer.Equals(nodeX.TextContent, nodeY.TextContent);
            }

            if (nodeX.NodeType == NodeType.Element)
            {
                var elementX = nodeX as IElement;
                var elementY = nodeY as IElement;

                if (nodeX.NodeName != nodeY.NodeName && !StringComparer.OrdinalIgnoreCase.Equals(_options.WildcardElementName, nodeX.NodeName))
                {
                    return false;
                }

                if (!IdsAreEqual(elementX.Id, elementY.Id))
                {
                    return false;
                }

                if (!ClassListsAreEqual(elementX.ClassList, elementY.ClassList))
                {
                    return false;
                }

                if (!AttributesAreEqual(elementX.Attributes, elementY.Attributes))
                {
                    return false;
                }
            }

            if (nodeX.HasChildNodes)
            {
                if (!nodeY.HasChildNodes)
                {
                    return false;
                }

                if (!ElementsAreEqual(nodeX.ChildNodes, nodeY.ChildNodes))
                {
                    return false;
                }
            }
            else if (nodeY.HasChildNodes)
            {
                return false;
            }

            return true;
        }

        private bool AttributesAreEqual(INamedNodeMap attributeMapX, INamedNodeMap attributeMapY)
        {
            if (_options.IgnoreAdditionalAttributes)
            {
                if (attributeMapX.Length > attributeMapY.Length)
                {
                    return false;
                }
            }
            else if (attributeMapX.Length != attributeMapY.Length)
            {
                return false;
            }

            foreach (var attributeX in attributeMapX)
            {
                // Id & Class are handled separately
                if (attributeX.Name == ClassAttributeName || attributeX.Name == IdAttributeName)
                {
                    continue;
                }

                var attributeY = attributeMapY.GetNamedItem(attributeX.NamespaceUri, attributeX.Name);
                if (attributeY == null || !_options.AttributeComparer.Equals(attributeX.Value, attributeY.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ClassListsAreEqual(ITokenList classListX, ITokenList classListY)
        {
            if (_options.IgnoreAdditionalClassNames)
            {
                if (classListX.Length > classListY.Length)
                {
                    return false;
                }

                if (_options.IgnoreClassNameOrder)
                {
                    return classListX.Intersect(classListY, StringComparer.Ordinal).Count() == classListX.Length;
                }

                return classListX.SequenceEqual(classListY.Intersect(classListX), StringComparer.Ordinal);
            }

            // Check exact element match if no additional class names are allowed
            if (classListX.Length != classListY.Length)
            {
                return false;
            }

            if (_options.IgnoreClassNameOrder)
            {
                return classListX.Union(classListY, StringComparer.Ordinal).Count() == classListX.Length;
            }

            return classListX.SequenceEqual(classListY, StringComparer.Ordinal);
        }

        private bool IdsAreEqual(string idX, string idY)
        {
            if (StringComparer.Ordinal.Equals(idX, idY))
            {
                return true;
            }

            if (idX is null && _options.IgnoreAdditionalAttributes)
            {
                return true;
            }

            return false;
        }

        bool IEqualityComparer<string>.Equals(string x, string y) => Equals(x, y);

        int IEqualityComparer<string>.GetHashCode(string obj) => throw new NotSupportedException();

        private static bool NotIsComment(INode node) => node.NodeType != NodeType.Comment;

        private static IEnumerable<(T itemX, T itemY)> ZipAll<T>(IEnumerable<T> listX, IEnumerable<T> listY, Func<T, bool> predicate)
        {
            using (var listXenum = listX.Where(predicate).GetEnumerator())
            using (var listYenum = listY.Where(predicate).GetEnumerator())
            {
                while (listXenum.MoveNext())
                {
                    if (listYenum.MoveNext())
                    {
                        yield return (listXenum.Current, listYenum.Current);
                    }
                    else
                    {
                        yield return (listXenum.Current, default(T));
                    }
                }

                // Enumerate remaining 
                while (listYenum.MoveNext())
                {
                    yield return (default(T), listYenum.Current);
                }
            }
        }
    }
}
