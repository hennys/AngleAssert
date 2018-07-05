namespace AngleAssert
{
    /// <summary>
    /// Values describing the reasons for a mismatch returned by a <see cref="HtmlComparer"/> instance.
    /// </summary>
    public enum HtmlCompareMismatchReason
    {
        /// <summary>
        /// The HTML does not match and no specific reason for the mismatch has been given.
        /// </summary>
        Default,

        /// <summary>
        /// The HTML was not considered a match as the selector did not match any elements.
        /// </summary>
        ElementNotFound,

        /// <summary>
        /// The HTML was not considered a match as multiple elements matches the selector but only a single match is allowed.
        /// </summary>
        MultipleElementsFound,
    }
}
