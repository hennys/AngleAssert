using System;
using System.Collections.Generic;

namespace AngleAssert
{
    /// <summary>
    /// The result of a comparison done by an <see cref="HtmlComparer"/> instance. Can be implicitly converted to a <see cref="bool"/>.
    /// </summary>
    public sealed class HtmlCompareResult : IEquatable<HtmlCompareResult>
    {
        private HtmlCompareResult(bool matches = false, string expected = null, string actual = null, HtmlCompareMismatchReason reason = HtmlCompareMismatchReason.Default)
        {
            Matches = matches;
            Expected = expected;
            Actual = actual;
            Reason = reason;
        }

        /// <summary>
        /// Gets a value indicating if the compare operation resulted in a match.
        /// </summary>
        public bool Matches { get; }

        /// <summary>
        /// Gets the reason for why a comparison was considered a mismatch. 
        /// </summary>
        /// <remarks>This property is only set when the comparison is a mismatch.</remarks>
        public HtmlCompareMismatchReason? Reason { get; }

        /// <summary>
        /// Gets the HTML that was expected.
        /// </summary>
        /// <remarks>This property is only set when the comparison is a mismatch.</remarks>
        public string Expected { get; }

        /// <summary>
        /// Gets the HTML that was found.
        /// </summary>
        /// <remarks>This property is only set when the comparison is a mismatch.</remarks>
        public string Actual { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is HtmlCompareResult r && Equals(r);

        /// <inheritdoc />
        public bool Equals(HtmlCompareResult other)
        {
            return Matches == other.Matches
                && Expected == other.Expected
                && Actual == other.Actual
                && Reason == other.Reason;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = -905952502;
            hashCode = hashCode * -1521134295 + Matches.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Expected);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Actual);
            hashCode = hashCode * -1521134295 + Reason.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Gets a <see cref="HtmlCompareResult"/> representing a match.
        /// </summary>
        public readonly static HtmlCompareResult Match = new HtmlCompareResult(true);

        /// <summary>
        /// Gets a <see cref="HtmlCompareResult"/> representing a match.
        /// </summary>
        public readonly static HtmlCompareResult ElementNotFound = new HtmlCompareResult(false, reason: HtmlCompareMismatchReason.ElementNotFound);

        /// <summary>
        /// Gets a <see cref="HtmlCompareResult"/> representing a match.
        /// </summary>
        public readonly static HtmlCompareResult MultipleElementsFound = new HtmlCompareResult(false, reason: HtmlCompareMismatchReason.MultipleElementsFound);

        /// <summary>
        /// Gets a <see cref="HtmlCompareResult"/> representing a mismatch.
        /// </summary>
        /// <param name="expected">The HTML that was expected in the comparison.</param>
        /// <param name="actual">The actual HTML that was found.</param>
        /// <param name="reason">The reason for the mismatch.</param>
        public static HtmlCompareResult Mismatch(string expected = null, string actual = null, HtmlCompareMismatchReason reason = HtmlCompareMismatchReason.Default)
            => new HtmlCompareResult(false, expected, actual, reason);

        /// <inheritdoc />
        public static implicit operator bool(HtmlCompareResult compareResults) => compareResults?.Matches ?? false;
    }
}
