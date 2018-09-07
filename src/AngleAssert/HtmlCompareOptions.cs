using System;

namespace AngleAssert
{
    /// <summary>
    /// Describes options for how the <see cref="HtmlComparer"/> should compare HTML.
    /// </summary>
    public class HtmlCompareOptions
    {
        private const string DefaultWildcardElementName = "any";

        /// <summary>
        /// Gets or sets what element name should match any element name in the candidate HTML.
        /// The default value is 'any'.
        /// </summary>
        public string WildcardElementName { get; set; } = DefaultWildcardElementName;

        /// <summary>
        /// When comparing using a selector, this property will define how multiple selector matches should be treated.
        /// The default mode is to consider multiple matches a mismatch. See <see cref="ElementSelectionMode"/> for additional options.
        /// </summary>
        public ElementSelectionMode ElementSelectionMode { get; set; } = ElementSelectionMode.First;

        /// <summary>
        /// When comparing using a selector, this property indicates if the selected element should itself be included in the comparison.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IncludeSelectedElement { get; set; }

        /// <summary>
        /// Indicates if the order of class names in the candidate HTML should be ignored.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IgnoreClassNameOrder { get; set; } = true;

        /// <summary>
        /// Indicates if additional class names on any element in the candidate HTML should be ignored.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreAdditionalClassNames { get; set; }

        /// <summary>
        /// Indicates if additional attribute on any element in the candidate HTML should be ignored. 
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreAdditionalAttributes { get; set; }

        /// <summary>
        /// Indicates if empty text nodes should be ignored when comparing HTML. 
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IgnoreEmptyTextNodes { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="StringComparer"/> that should be used to compare text elements.
        /// The default comparer is <see cref="StringComparer.Ordinal"/>.
        /// </summary>
        public StringComparer TextComparer { get; set; } = HtmlTextComparer.Ordinal;

        /// <summary>
        /// Gets or sets the <see cref="StringComparer"/> that should be used to compare attribute values.
        /// The default comparer is <see cref="StringComparer.Ordinal"/>.
        /// </summary>
        /// <remarks>
        /// Id and class value comparisons are not affected by this setting and is always compared using the <see cref="StringComparer.Ordinal"/> comparer.
        /// </remarks>
        public StringComparer AttributeComparer { get; set; } = StringComparer.Ordinal;
    }
}
