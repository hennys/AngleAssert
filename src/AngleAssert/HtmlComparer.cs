using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace AngleAssert
{
    /// <summary>
    /// Class that will compare two HTML strings and check if the they would be considered equal.
    /// </summary>
    public class HtmlComparer : IEqualityComparer<string>
    {
        private const string StandardContextHtml = "<!DOCTYPE html><html><head><title></title></head><body></body></html>";
        private const string IdAttributeName = "id";
        private const string ClassAttributeName = "class";
        // Keep private as it is a mutable class
        private static readonly HtmlCompareOptions DefaultOptions = new HtmlCompareOptions();
        private readonly HtmlCompareOptions _options;
        private readonly IHtmlParser _parser;

        /// <summary>
        /// Gets a <see cref="HtmlComparer"/> that uses the default <see cref="HtmlCompareOptions"/>.
        /// </summary>
        public static HtmlComparer Default { get; } = new HtmlComparer(DefaultOptions);

        /// <summary>
        /// Gets a <see cref="HtmlComparer"/> for comparing html fragments.
        /// </summary>
        public static HtmlComparer Fragment { get; } = new HtmlComparer(new HtmlCompareOptions { TreatHtmlAsFragment = true });

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlComparer"/> class using the default options.
        /// </summary>
        public HtmlComparer() : this(DefaultOptions) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlComparer"/> class with a specific set of options.
        /// </summary>
        /// <param name="options">The set of options that should be used.</param>
        public HtmlComparer(HtmlCompareOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var context = BrowsingContext.New(Configuration.Default);
            _parser = context.GetService<IHtmlParser>();
        }

        /// <summary>
        /// Checks if the provided HTML string contains an element represented by a selector.
        /// </summary>
        /// <param name="html">A string representing the HTML that should be checked.</param>
        /// <param name="selector">The CSS selector that should be used to find a matching element.</param>
        /// <returns><c>True</c> if an element that matched the selector was found; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// Initialize the <see cref="HtmlComparer"/> with an options instance where <see cref="HtmlCompareOptions.ElementSelectionMode"/>
        /// is set to <see cref="ElementSelectionMode.Single"/> if you want this method to return <c>false</c> in case
        /// multple matching element are found.
        /// </remarks>
        public bool Contains(string html, string selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (string.IsNullOrWhiteSpace(selector))
            {
                throw new ArgumentException("Selector cannot be empty.", nameof(selector));
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                return false;
            }
            var rootNode = ParseHtml(html, _options.TreatHtmlAsFragment);

            if (_options.ElementSelectionMode == ElementSelectionMode.Single)
            {
                return rootNode.QuerySelectorAll(selector).Length == 1;
            }

            return rootNode.QuerySelector(selector) != null;
        }

        /// <summary>
        /// Checks if element in the provided HTML string matches an expected HTML string.
        /// </summary>
        /// <param name="expected">A string represeting the expected HTML that the <paramref name="html"/> should be compared against.</param>
        /// <param name="html">A string representing the HTML that contains the element that should be checked.</param>
        /// <param name="selector">The CSS selector that should be used to find a matching element.</param>
        /// <returns><c>True</c> if the selected element matched the expected html; otherwise <c>false</c>.</returns>
        public HtmlCompareResult Equals(string expected, string html, string selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (string.IsNullOrWhiteSpace(selector))
            {
                throw new ArgumentException("Selector cannot be empty.", nameof(selector));
            }

            var expectedRoot = ParseHtml(expected, true);
            if (_options.ElementComparisonMode != ElementComparisonMode.InnerHtml && expectedRoot.ChildElementCount > 1)
            {
                throw new ArgumentException($"The expected html cannot contain multiple child nodes when the ElementComparisonMode option is set to '{_options.ElementComparisonMode}'.", nameof(expected));
            }
            else if (_options.ElementComparisonMode == ElementComparisonMode.Element && expectedRoot.FirstChild.HasChildNodes)
            {
                throw new ArgumentException("The expected html cannot contain child nodes when the ElementComparisonMode option is set to Element.", nameof(expected));
            }
            var expectedNodeTree = CreateNodeTree(expectedRoot);

            var root = ParseHtml(html, _options.TreatHtmlAsFragment);

            // Default selection mode will only need the first element
            if (_options.ElementSelectionMode == ElementSelectionMode.First)
            {
                var element = root.QuerySelector(selector);
                if (element is null)
                {
                    return HtmlCompareResult.ElementNotFound;
                }

                return MatchesElement(expected, expectedNodeTree, element);
            }

            // All other selection modes needs all elements
            var elements = root.QuerySelectorAll(selector);
            if (elements.Length == 0)
            {
                return HtmlCompareResult.ElementNotFound;
            }

            if (_options.ElementSelectionMode == ElementSelectionMode.Single)
            {
                if (elements.Length > 1)
                {
                    return HtmlCompareResult.Mismatch(reason: HtmlCompareMismatchReason.MultipleElementsFound);
                }

                return MatchesElement(expected, expectedNodeTree, elements[0]);
            }

            var results = elements.Select(el => MatchesElement(expected, expectedNodeTree, el));

            if (_options.ElementSelectionMode == ElementSelectionMode.All)
            {
                return results.FirstOrDefault(r => !r.Matches) ?? HtmlCompareResult.Match;
            }

            if (_options.ElementSelectionMode == ElementSelectionMode.Any)
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

            var expectedNodeTree = CreateNodeTree(ParseHtml(expected, _options.TreatHtmlAsFragment));
            var candidateNodeTree = CreateNodeTree(ParseHtml(html, _options.TreatHtmlAsFragment));

            return NodeTreesAreEqual(expectedNodeTree, candidateNodeTree) ? HtmlCompareResult.Match : HtmlCompareResult.Mismatch(expected, html);
        }

        private HtmlCompareResult MatchesElement(string expected, NodeTree expectedNodeTree, IElement element)
        {
            if (_options.ElementComparisonMode == ElementComparisonMode.Element)
            {
                var expectedElement = expectedNodeTree.First();
                if (NodesAreEqual(expectedElement, element))
                {
                    return HtmlCompareResult.Match;
                }

                return HtmlCompareResult.Mismatch(expected, HtmlMarkupFormatter.Instance.OpenTag(element, true));
            }

            var includeRoot = _options.ElementComparisonMode == ElementComparisonMode.OuterHtml;
            var elementNodeTree = CreateNodeTree(element, includeRoot);

            if (NodeTreesAreEqual(expectedNodeTree, elementNodeTree))
            {
                return HtmlCompareResult.Match;
            }

            return HtmlCompareResult.Mismatch(expected, includeRoot ? element.OuterHtml : element.InnerHtml);
        }

        private bool NodeTreesAreEqual(NodeTree expected, NodeTree candidate)
        {
            foreach (var (itemX, itemY) in ZipAll(expected, candidate))
            {
                if (!NodesAreEqual(itemX, itemY))
                {
                    return false;
                }
            }

            return true;
        }

        private bool NodesAreEqual(INode nodeX, INode nodeY)
        {
            if (nodeX?.NodeType != nodeY?.NodeType)
            {
                return false;
            }

            if (nodeX.NodeType == NodeType.Text)
            {
                if (ShouldTrimTextContent(nodeX.ParentElement))
                {
                    return _options.TextComparer.Equals(nodeX.TextContent.Trim(), nodeY.TextContent.Trim());
                }

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

        private IElement ParseHtml(string html, bool treatAsFragment)
        {
            if (treatAsFragment && !FragmentIsDocument(html))
            {
                var fragmentDoc = _parser.ParseDocument(StandardContextHtml);
                fragmentDoc.Body.InnerHtml = html;
                return fragmentDoc.Body;
            }

            var doc = _parser.ParseDocument(html);
            return doc.DocumentElement;
        }

        private NodeTree CreateNodeTree(IElement root, bool includeRoot = false) => new NodeTree(root, _options.IgnoreEmptyTextNodes, includeRoot);

        private static bool FragmentIsDocument(string html) => html.StartsWith("<!DOCTYPE html>") || html.StartsWith("<html>");

        // In reality this algoritm is significantly more complex
        private static bool ShouldTrimTextContent(IElement element) => !InlineElements.Contains(element.TagName);

        private static readonly HashSet<string> InlineElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "abbr", "acronym", "audio", "b", "bdi", "bdo", "big", "br", "button", "canvas", "cite", "code", "data", "datalist", "del", "dfn", "em", "embed", "i", "iframe", "img", "input", "ins", "kbd",
            "label", "map", "mark", "meter", "noscript", "object", "output", "picture", "progress", "q", "ruby", "s", "samp", "script", "select", "slot", "small", "span", "strong", "sub", "sup", "svg", "template",
            "textarea", "time", "u", "tt", "var", "video", "wbr"
        };

        private static IEnumerable<(T itemX, T itemY)> ZipAll<T>(IEnumerable<T> listX, IEnumerable<T> listY)
        {
            using (var listXenum = listX.GetEnumerator())
            using (var listYenum = listY.GetEnumerator())
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

        private class NodeTree : IEnumerable<INode>
        {
            private readonly bool _includeRoot;
            private readonly bool _ignoreEmptyTextNodes;

            public NodeTree(IElement root, bool ignoreEmptyTextNodes = false, bool includeRoot = false)
            {
                Root = root;
                _includeRoot = includeRoot;
                _ignoreEmptyTextNodes = ignoreEmptyTextNodes;
            }

            public IElement Root { get; }

            public IEnumerator<INode> GetEnumerator()
            {
                var root = _includeRoot ? Root.Parent : Root;
                return new NodeTreeEnumerator(Root.Owner.CreateTreeWalker(root, ~FilterSettings.Comment, NodeFilter));
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private FilterResult NodeFilter(INode node)
            {
                if (_ignoreEmptyTextNodes && node.NodeType == NodeType.Text)
                {
                    return string.IsNullOrWhiteSpace(node.NodeValue) ? FilterResult.Skip : FilterResult.Accept;
                }

                // We must filter out siblings when including the root element
                if (_includeRoot && !Root.Equals(node) && node.IsSiblingOf(Root))
                {
                    return FilterResult.Reject;
                }

                return FilterResult.Accept;
            }

            private class NodeTreeEnumerator : IEnumerator<INode>
            {
                private readonly ITreeWalker _walker;

                public NodeTreeEnumerator(ITreeWalker walker)
                {
                    _walker = walker;
                }

                public INode Current => _walker.Current;
                object IEnumerator.Current => Current;

                public void Dispose() { }
                public bool MoveNext() => _walker.ToNext() != null;
                public void Reset() => _walker.Current = _walker.Root;
            }
        }
    }
}
