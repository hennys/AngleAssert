namespace AngleAssert
{
    /// <summary>
    /// Describes how elements should be selected and compared by the <see cref="HtmlComparer"/>.
    /// </summary>
    public enum ElementSelectionMode
    {
        /// <summary>
        /// Only the first element matched by the selector will be compared.
        /// </summary>
        First,

        /// <summary>
        /// Only one element can be matched by the selector to consider it as a match.
        /// </summary>
        Single,

        /// <summary>
        /// All elements matched by the selector will be compared.
        /// </summary>
        All,

        /// <summary>
        /// All elements matched by the selector will be compared, but only one need to match.
        /// </summary>
        Any,
    }
}
