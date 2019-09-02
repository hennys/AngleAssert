namespace AngleAssert
{
    /// <summary>
    /// Describes how HTML elements should be compared by the <see cref="HtmlComparer"/>.
    /// </summary>
    public enum ElementComparisonMode
    {
        /// <summary>
        /// Only the inner content of the element will be compared.
        /// </summary>
        InnerHtml,

        /// <summary>
        /// Both the element and the content will be compared.
        /// </summary>
        OuterHtml,

        /// <summary>
        /// Only the element itself will be compared.
        /// </summary>
        Element,
    }
}
