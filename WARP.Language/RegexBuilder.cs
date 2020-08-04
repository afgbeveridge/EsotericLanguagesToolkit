//============================================================================================================================================================================
// Copyright (c) 2011-2013 Tony Beveridge
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//============================================================================================================================================================================

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class RegexBuilder {
                static RegexBuilder() => RegisteredPatterns = new Dictionary<string, string>();

                internal RegexBuilder() => Builder = new StringBuilder();

                internal RegexBuilder Or => this.Fluently(() => Builder.Append("|"));

                private bool InCaptureGroup { get; set; }

                private StringBuilder Builder { get; }

                private static Dictionary<string, string> RegisteredPatterns { get; }

                internal static RegexBuilder New() => new RegexBuilder();

                internal RegexBuilder StartsWith() => this.Fluently(() => Builder.Append("^"));

                internal RegexBuilder StartCaptureGroup(string name) {
                        if (InCaptureGroup) EndCaptureGroup();
                        InCaptureGroup = true;
                        return this.Fluently(() => Builder.Append("(?<").Append(name).Append(">"));
                }

                internal RegexBuilder AddCharacterClass(string expr) =>
                        this.Fluently(() => Builder.Append("[").Append(expr).Append("]"));

                internal RegexBuilder Optional(string expr, int max = 1) =>
                        this.Fluently(() => Builder.Append(expr).Append("{0,").Append(max).Append("}"));

                internal RegexBuilder BoundedRepetition(int bound) =>
                        this.Fluently(() => Builder.Append("{").Append(bound).Append("}"));

                internal RegexBuilder OneOrMore() => this.Fluently(() => Builder.Append("+"));

                internal RegexBuilder Literal(string content) => this.Fluently(() => Builder.Append(content));

                internal RegexBuilder OneFrom(IEnumerable<string> options) =>
                        this.Fluently(() => Builder.Append(string.Join("|", options)));

                internal RegexBuilder EndCaptureGroup() {
                        InCaptureGroup = false;
                        return this.Fluently(() => Builder.Append(")"));
                }

                internal RegexBuilder EndMatching() => this.Fluently(() => Builder.Append("$"));

                internal RegexBuilder RememberAs(string key) =>
                        this.Fluently(() => RegisteredPatterns[key] = Builder.ToString());

                internal RegexBuilder Include(string key) =>
                        this.Fluently(() => Builder.Append(RegisteredPatterns[key]));

                internal RegexBuilder Reset() => this.Fluently(() => Builder.Length = 0);

                internal Regex ToRegex() {
                        if (InCaptureGroup) EndCaptureGroup();
                        return new Regex(Builder.ToString());
                }

                internal static Regex ToRegex(string key) => new Regex(RegisteredPatterns[key]);
        }
}