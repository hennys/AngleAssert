# AngleAssert

Library to compare and assert HTML. 

HTML parsing done using [AngleSharp](https://github.com/AngleSharp/AngleSharp)

## Usage

The HtmlComparer class implements IEqualityComparer&lt;string&gt; to make it easy to use directly with many Assertion libraries.

Example of use with the default xUnit.net string assertions.

Assert.Equal("&lt;p&gt;expected&lt;/p&gt;", "&lt;p&gt;actual&lt;/p&gt;", HtmlComparer.Default);

The Equals method of the HtmlComparer returns a HtmlCompareResult object. This object can be implicitly be converted to a boolean, 
but it also exposes properties that contains additional information about the element where the compare failed so that a more detailed 
message can be provided to users.

### xUnit.net integration

Custom assertions for xUnit.net v2, for developers using the source-based assert library via the xunit.assert.source NuGet package can be found in the AngleAssert.Xunit project. 

## Compare options

The default comparer, accessible through HtmlComparer.Default, uses a common set of compare options, but you can create a customized comparer by passing in your own options.

var comparer = new HtmlComparer(new HtmlCompareOptions { IgnoreAdditionalAttributes = false, IgnoreAdditionalClassNames = true, IgnoreClassNameOrder = false });

## License

The MIT License (MIT)

Copyright (c) 2018 Henrik Nystrom

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.