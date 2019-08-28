using System;
using System.IO;
using Xunit;

namespace AngleAssert
{
    public class HtmlComparerTest
    {
        #region Equals

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", "   ")]
        [InlineData(null, "")]
        [InlineData(" ", "\t")]
        [InlineData("different internal whitespace", "different   internal   whitespace")]
        public void Equals_WithSimpleMatchingTextStrings_ShouldReturnMatch(string expected, string candidate)
        {
            Assert.True(HtmlComparer.Default.Equals(expected, candidate));
        }

        [Theory]
        [InlineData(null, "some string")]
        [InlineData("some string", null)]
        [InlineData("different casing", "Different Casing")]
        public void Equals_WithSimpleNonMatchingTextStrings_ShouldReturnMismatch(string expected, string candidate)
        {
            AssertMismatch(HtmlComparer.Default.Equals(expected, candidate), expected, candidate);
        }

        [Theory]
        [InlineData("<p>text</p>", "<p>text</p>")]
        [InlineData("<p>text</p>", " <p>text</p> ")]
        public void Equals_WithMatchingHtmlFragments_ShouldReturnMatch(string expected, string candidate)
        {
            Assert.True(HtmlComparer.Default.Equals(expected, candidate));
        }

        [Theory]
        [InlineData("<p>text</p>", "<strong>text</strong>")]
        [InlineData("<p>text</p>", "<p> text </p>")]
        [InlineData("<p>text</p>", "<p><strong>text</strong></p>")]
        [InlineData("<p>text</p>", "<p>text<strong>content</strong></p>")]
        [InlineData("<p>text<strong>content</strong></p>", "<p>text</p>")]
        [InlineData("<p>text<span>content</span></p>", "<p>text<strong>content</strong></p>")]
        [InlineData("<p><span>text</span><span>content</span></p>", "<p><span>text</span></p>")]
        public void Equals_WhenHtmlFragmentsDiffers_ShouldReturnMismatch(string expected, string candidate)
        {
            AssertMismatch(HtmlComparer.Default.Equals(expected, candidate), expected, candidate);
        }

        [Fact]
        public void Equals_WhenHtmlIsMissingElmenent_ShouldReturnMismatch()
        {
            const string expected = "<p><span>text</span><span>content</span></p>";
            const string candidate = "<p><span>text</span></p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenReferenceElementIsWildcard_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<any>text</any>", "<p>text</p>"));
        }

        [Fact]
        public void Equals_WhenDifferentAttributeQuotesAreUsed_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<p id='one'>text</p>", "<p id=\"one\">text</p>"));
        }

        [Fact]
        public void Equals_WhenHtmlContainsComment_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<p id='one'>text</p>", "<p id='one'><!--comment-->text</p>"));
        }

        [Fact]
        public void Equals_WhenEmptyTextNodesAreDifferent_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<div><div>text</div></div>", "<div> <div>text</div> </div>"));
        }

        [Fact]
        public void Equals_WhenElementIdsAreDifferent_ShouldReturnMismatch()
        {
            const string expected = "<p id='one'>text</p>";
            const string candidate = "<p id='two'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenClassAttributesAreDifferent_ShouldReturnMismatch()
        {
            const string expected = "<p class='one'>text</p>";
            const string candidate = "<p class='two'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenClassAttributesArePartiallyDifferent_ShouldReturnMismatch()
        {
            const string expected = "<p class='one two'>text</p>";
            const string candidate = "<p class='one'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenClassAttributeValuesAreInDifferentOrder_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<p class='one two'>text</p>", "<p class='two one'>text</p>"));
        }

        [Fact]
        public void Equals_WhenCandidateHasAdditionalClassNames_ShouldReturnMismatch()
        {
            const string expected = "<p class='one'>text</p>";
            const string candidate = "<p class='one two'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenCandidateHasAdditionalClassNamesAndIgnoreAdditionalClassNamesIsTrue_ShouldReturnMatch()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { IgnoreAdditionalClassNames = true });
            Assert.True(comparer.Equals("<p class='one'>text</p>", "<p class='two one'>text</p>"));
        }

        [Fact]
        public void Equals_WhenClassAttributeValuesAreInDifferentOrderAndIgnoreOrderIsFalse_ShouldReturnMismatch()
        {
            const string expected = "<p class='one two'>text</p>";
            const string candidate = "<p class='two one'>text</p>";

            var comparer = new HtmlComparer(new HtmlCompareOptions { IgnoreClassNameOrder = false });

            var result = comparer.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenAttributeIsMissing_ShouldReturnMismatch()
        {
            const string expected = "<p data-one='value'>text</p>";
            const string candidate = "<p data-two='value'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenCandidateHasAdditionalAttributes_ShouldReturnMismatch()
        {
            const string expected = "<p class='one'>text</p>";
            const string candidate = "<p id='wow' class='one' data-value='something'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenCandidateHasAdditionalAttributesAndIgnoreAdditionalAttributesIsFalse_ShouldReturnMatch()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { IgnoreAdditionalAttributes = true });
            Assert.True(comparer.Equals("<p class='one'>text</p>", "<p id='wow' class='one' data-value='something'>text</p>"));
        }

        [Fact]
        public void Equals_WhenAttributesValuesAreDifferent_ShouldReturnMismatch()
        {
            const string expected = "<p longdesc='one'>text</p>";
            const string candidate = "<p longdesc='two'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WithDefaultAttributeComparison_WhenAttributesValuesAreDifferentCase_ShouldReturnMismatch()
        {
            const string expected = "<p longdesc='one'>text</p>";
            const string candidate = "<p longdesc='ONE'>text</p>";

            var result = HtmlComparer.Default.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WithCaseInsensitiveAttributeComparison_WhenAttributesValuesAreDifferentCase_ShouldReturnMatch()
        {
            const string expected = "<p longdesc='one'>text</p>";
            const string candidate = "<p longdesc='ONE'>text</p>";

            var comparer = new HtmlComparer(new HtmlCompareOptions { AttributeComparer = StringComparer.OrdinalIgnoreCase });

            var result = comparer.Equals(expected, candidate);

            Assert.True(result);
        }

        [Fact]
        public void Equals_WithCaseInsensitiveAttributeComparison_WhenIdsAreDifferentCase_ShouldReturnMismatch()
        {
            const string expected = "<p id='one'>text</p>";
            const string candidate = "<p id='ONE'>text</p>";

            var comparer = new HtmlComparer(new HtmlCompareOptions { AttributeComparer = StringComparer.OrdinalIgnoreCase });

            var result = comparer.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WithCaseInsensitiveAttributeComparison_WhenClassAttributesValuesAreDifferentCase_ShouldReturnMismatch()
        {
            const string expected = "<p class='one'>text</p>";
            const string candidate = "<p class='ONE'>text</p>";

            var comparer = new HtmlComparer(new HtmlCompareOptions { AttributeComparer = StringComparer.OrdinalIgnoreCase });

            var result = comparer.Equals(expected, candidate);

            AssertMismatch(result, expected, candidate);
        }

        [Fact]
        public void Equals_WhenAttributesAreInDifferentOrder_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<p id='one' class='two' longdesc='three'>text</p>", "<p class='two' longdesc='three' id='one'>text</p>"));
        }

        #endregion

        #region Equals with Selector

        [Fact]
        public void Equals_WhenSelectorIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => HtmlComparer.Default.Equals("", "", null));
        }

        [Fact]
        public void Equals_WhenSelectorIsEmpty_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => HtmlComparer.Default.Equals("", "", " "));
        }

        [Fact]
        public void Equals_WhenSelectorDoesNotMatchElement_ShouldReturnMismatch()
        {
            var result = HtmlComparer.Default.Equals("<p>text</p>", "<section><p id='one'>text</p></section>", ".someClass");

            AssertMismatch(result, reason: HtmlCompareMismatchReason.ElementNotFound);
        }

        [Fact]
        public void Equals_WhenSelectedElementMatchesExpected_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<p id='one'>text</p>", "<section class='someClass'><p id='one'>text</p></section>", ".someClass"));
        }

        [Fact]
        public void Equals_WithDefaultSelectionMode_WhenMultipleElementsExistAndFirstMatches_ShouldReturnMatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.First });

            Assert.True(comparer.Equals(expected, candidate, ".someClass"));
        }

        [Fact]
        public void Equals_WithDefaultSelectionMode_WhenMultipleElementsExistAndFirstDoesNotMatch_ShouldReturnMismatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.First });

            var result = comparer.Equals(expected, candidate, ".someClass");

            AssertMismatch(result, expected, "<p>nope</p>");
        }

        [Fact]
        public void Equals_WithSelectionModeSingle_WhenOnlyOneElementsExist_ShouldReturnMatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='one'><p>nope</p></section>
                <section class='two'><p>text</p></section>
                <section class='three'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Single });

            Assert.True(comparer.Equals(expected, candidate, ".two"));
        }

        [Fact]
        public void Equals_WithSelectionModeSingle_WhenMultipleElementsExist_ShouldReturnMultipleElementsFoundMismatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Single });

            var result = comparer.Equals(expected, candidate, ".someClass");

            AssertMismatch(result, reason: HtmlCompareMismatchReason.MultipleElementsFound);
        }

        [Fact]
        public void Equals_WithSelectionModeAll_WhenMultipleElementsAreSelectedAndAllMatch_ShouldReturnMatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>text</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.All });

            Assert.True(comparer.Equals(expected, candidate, ".someClass"));
        }

        [Fact]
        public void Equals_WithSelectionModeAll_WhenMultipleElementsAreSelectedAndNotAllMatch_ShouldReturnMismatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>text</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.All });

            var result = comparer.Equals(expected, candidate, ".someClass");

            AssertMismatch(result, expected, "<p>nope</p>");
        }

        [Fact]
        public void Equals_WithSelectionModeAny_WhenMultipleElementsExistAndOneMatches_ShouldReturnMatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>text</p></section>
                <section class='someClass'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Any });
            Assert.True(comparer.Equals(expected, candidate, ".someClass"));
        }

        [Fact]
        public void Equals_WithSelectionModeAny_WhenMultipleElementsExistAndNonMatches_ShouldReturnMismatch()
        {
            const string expected = "<p>text</p>";
            const string candidate =
            @"
                <section class='someClass'><p>hardly</p></section>
                <section class='someClass'><p>nope</p></section>
                <section class='someClass'><p>fake</p></section>
            ";

            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Any });

            var result = comparer.Equals(expected, candidate, ".someClass");

            AssertMismatch(result, expected, "<p>hardly</p>");
        }

        [Fact]
        public void Equals_WithSelectedElementComparisonOff_WhenSelectedElementMatchesExpected_ShouldReturnMismatch()
        {
            const string expected = "<p id='one'>text</p>";
            const string candidate = "<div><p id='one'>text</p><div>";

            var result = HtmlComparer.Default.Equals(expected, candidate, "div > p");

            AssertMismatch(result, expected, "text");
        }

        [Fact]
        public void Equals_WithSelectedElementComparisonOn_WhenSelectedElementMatchesExpected_ShouldReturnMatch()
        {
            const string expected = "<p id='one'>text</p>";
            const string candidate = "<div><p id='one'>text</p><div>";

            var comparer = new HtmlComparer(new HtmlCompareOptions { IncludeSelectedElement = true });

            Assert.True(comparer.Equals(expected, candidate, "div > p"));
        }

        [Fact]
        public void Equals_WithSimpleHtmlDocument_WhenSelectedElementMatchesExpected_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("Another <em>paragraph</em> of text.", SimpleHtmlDocument, "h2 + p"));
        }

        [Fact]
        public void Equals_WithSimpleHtmlDocument_WhenOnlyIndentationDiffer_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("<span>Indented html</span>", SimpleHtmlDocument, "#indented"));
        }

        [Fact]
        public void Equals_WithComplexHtmlDocument_WhenSelectedElementMatchesExpected_ShouldReturnMatch()
        {
            Assert.True(HtmlComparer.Default.Equals("1", ComplexHtmlDocument, "a.social-count"));
        }

        #endregion

        #region Contains

        [Fact]
        public void Contains_WhenHtmlIsEmpty_ShouldReturnElementNotFound()
        {
            Assert.Equal(HtmlCompareResult.ElementNotFound, HtmlComparer.Default.Contains("  ", "div"));
        }

        [Fact]
        public void Contains_WhenSelectorIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>("selector", () => HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", null));
        }

        [Fact]
        public void Contains_WhenSelectorIsEmpty_ShouldThrow()
        {
            Assert.Throws<ArgumentException>("selector", () => HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", " "));
        }

        [Fact]
        public void Contains_WhenSelectorMatchesElement_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p#one"));
        }

        [Fact]
        public void Contains_WhenSelectorUsesDifferentIdCasing_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p#One"));
        }

        [Fact]
        public void Contains_WhenSelectorUsesDifferentClassCasing_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p.Two"));
        }

        [Fact]
        public void Contains_WhenSelectorDoesNotMatchAnyElement_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p.one"));
        }

        [Fact]
        public void Contains_WithDefaultSelectionMode_WhenSelectorMatchesMultipleElements_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p><p>more</p></div>", "p"));
        }

        [Fact]
        public void Contains_WithSelectionModeSingle_WhenSelectorMatchesMultipleElements_ShouldReturnFalse()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Single });

            Assert.False(comparer.Contains("<div><p id='one' class='two'>text</p><p>more</p></div>", "p"));
        }

        [Fact]
        public void Contains_WithSelectionModeFirst_WhenSelectorMatchesMultipleElements_ShouldReturnTrue()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.First });

            Assert.True(comparer.Contains("<div><p id='one' class='two'>text</p><p>more</p></div>", "p"));
        }

        [Fact]
        public void Contains_WithSelectionModeAll_WhenSelectorMatchesMultipleElements_ShouldReturnTrue()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.All });

            Assert.True(comparer.Contains("<div><p id='one' class='two'>text</p><p>more</p></div>", "p"));
        }

        [Fact]
        public void Contains_WithSelectionModeAny_WhenSelectorMatchesMultipleElements_ShouldReturnTrue()
        {
            var comparer = new HtmlComparer(new HtmlCompareOptions { ElementSelectionMode = ElementSelectionMode.Any });

            Assert.True(comparer.Contains("<div><p id='one' class='two'>text</p><p>more</p></div>", "p"));
        }

        [Fact]
        public void Contains_WhenSelectorWithAttributeMatchesElement_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", ".two[id]"));
        }

        [Fact]
        public void Contains_WhenSelectorWithAttributeDoesNotMatchAnyElement_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p[desc]"));
        }

        [Fact]
        public void Contains_WhenSelectorWithAttributeValueMatchesElement_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p[id=one]"));
        }

        [Fact]
        public void Contains_WhenSelectorWithAttributeValueDoesNotMatchAnyElement_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p[id=two]"));
        }

        [Fact]
        public void Contains_WhenSelectorWithClassNameMatchesElementWithMultipleClassNames_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains("<div><p id='one' class='two three'>text</p></div>", "p.three"));
        }

        [Fact]
        public void Contains_WhenSelectorWithClassNameHasDifferentCasingThanElement_ShouldReturnFalse()
        {
            Assert.False(HtmlComparer.Default.Contains("<div><p id='one' class='two'>text</p></div>", "p.TWO"));
        }

        [Fact]
        public void Contains_WhenSelectorMatchesElementInSimpleHtmlDocument_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains(SimpleHtmlDocument, "main > h1"));
        }

        [Fact]
        public void Contains_WhenSelectorMatchesElementInComplexHtmlDocument_ShouldReturnTrue()
        {
            Assert.True(HtmlComparer.Default.Contains(ComplexHtmlDocument, "svg.octicon-mark-github"));
        }

        #endregion

        private static string _simpleHtmlDoc;
        private static string _complexHtmlDoc;

        private static string SimpleHtmlDocument => _simpleHtmlDoc ?? (_simpleHtmlDoc = GetResourceString("simple.html"));

        private static string ComplexHtmlDocument => _complexHtmlDoc ?? (_complexHtmlDoc = GetResourceString("complex.html"));

        private static string GetResourceString(string fileName)
        {
            using (var stream = typeof(HtmlComparerTest).Assembly.GetManifestResourceStream($"AngleAssert.Resources.{fileName}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static void AssertElementNotFound(HtmlCompareResult result)
            => AssertMismatch(result, reason: HtmlCompareMismatchReason.ElementNotFound);

        private static void AssertMismatch(HtmlCompareResult result, string expected = null, string actual = null, HtmlCompareMismatchReason reason = HtmlCompareMismatchReason.Default)
        {
            var expectedResult = HtmlCompareResult.Mismatch(expected, actual?.Replace('\'', '"'), reason);

            Assert.Equal(expectedResult, result);
        }
    }
}
