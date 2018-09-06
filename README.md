# AngleAssert

Library to compare and assert HTML. Supports validating that two HTML string are equivalent, by comparing the parsed DOM rather than the exact string which allows for insignificant differences such as which quote marks that are used the or the order of attributes or class names.  

HTML parsing done using [AngleSharp](https://github.com/AngleSharp/AngleSharp)

## Usage

The primary class of this library is the ``HtmlComparer``. This class can be used to compare two HTML strings.
```csharp
var result = HtmlComparer.Default.Equals("<p id='my' class='one two'>expected</p>", 
                                         "<p class=\"two one\" id=\"my\">actual</p>");
```

The ``Equals`` method of the ``HtmlComparer`` class returns a ``HtmlCompareResult`` object. This object can be implicitly be converted to a boolean, but it also exposes properties that contains additional information about the element where the compare failed so that a more detailed message can be provided to users.

It is also possible to compare a specific element of the HTML string by providing a selector to the ``Equals`` method.
```csharp
var result = HtmlComparer.Default.Equals("<em>expected</em>", "<div><p>em>actual</em></p></div><p>other</p>", "div > p");
```

### Assertions

The ``HtmlComparer`` class implements ``IEqualityComparer<string>`` to make it easy to use directly with many assertion libraries.

Example of use together with the default xUnit.net string assertions.

```csharp
Assert.Equal("<p>expected</p>", "<p>actual</p>", HtmlComparer.Default);
```

### xUnit.net integration

Custom assertions for xUnit.net v2, for developers using the source-based assert library via the xunit.assert.source NuGet package can be found in the AngleAssert.Xunit project.

Example of use with the xUnit.net Assert partial extensions

```csharp
Assert.Html("<p>expected</p>", "<p>actual</p>");
```

Example of selector usage:

```csharp
Assert.Html("<p>expected</p>", "<div class='one'><p>actual</p><div>", "div.one");
```

## Compare options

The default comparer, accessible through ``HtmlComparer.Default``, uses a common set of compare options, but you can create a customized comparer by passing in your own options.

```csharp
var comparer = new HtmlComparer(
                   new HtmlCompareOptions 
                   { 
                       IgnoreAdditionalAttributes = false, 
                       IgnoreAdditionalClassNames = true, 
                       IgnoreClassNameOrder = false 
                   });
```

## NuGet packages

AngleAssert is available through nuget.org as two separate packages:
* https://www.nuget.org/packages/AngleAssert
* https://www.nuget.org/packages/AngleAssert.Xunit

## Build status

[![Build status](https://ci.appveyor.com/api/projects/status/xbbq0vecqd8glf50?svg=true)](https://ci.appveyor.com/project/hennys/angleassert)

## License

The MIT License (MIT)

Copyright (c) 2018 Henrik Nystrom

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
