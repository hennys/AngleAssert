using System;

namespace AngleAssert
{
    /// <summary>
    /// <see cref="StringComparer"/> that can be used to compare two strings where multple recurring whitespaces are ignored.
    /// </summary>
    public class HtmlTextComparer : StringComparer
    {
        /// <summary>
        /// A <see cref="HtmlTextComparer"/> that uses the <see cref="StringComparer.Ordinal"/> internally.
        /// </summary>
        public static new readonly StringComparer Ordinal = new HtmlTextComparer(StringComparer.Ordinal);

        private readonly StringComparer _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTextComparer"/> class.
        /// </summary>
        /// <param name="stringComparer">Underlying <see cref="StringComparer"/> used to compare the strings.</param>
        public HtmlTextComparer(StringComparer stringComparer)
        {
            _comparer = stringComparer ?? throw new ArgumentNullException(nameof(stringComparer));
        }

        /// <inheritdoc />
        public override bool Equals(string x, string y) => _comparer.Equals(RemoveInsignificantWhitespace(ref x), RemoveInsignificantWhitespace(ref y));

        /// <inheritdoc />
        public override int Compare(string x, string y) => _comparer.Compare(RemoveInsignificantWhitespace(ref x), RemoveInsignificantWhitespace(ref y));

        /// <inheritdoc />
        public override int GetHashCode(string obj) => _comparer.GetHashCode(RemoveInsignificantWhitespace(ref obj));

        private static string RemoveInsignificantWhitespace(ref string s)
        {
            if (s is null)
            {
                return null;
            }

            var result = new char[s.Length];
            var j = 0;
            var whitespace = false;
            for (var i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s, i))
                {
                    whitespace = true;
                    continue;
                }
                else if (whitespace)
                {
                    result[j++] = ' ';
                    whitespace = false;
                }
                result[j++] = s[i];
            }

            if (whitespace && j > 0)
            {
                result[j++] = ' ';
            }

            return new string(result, 0, j);
        }
    }
}
