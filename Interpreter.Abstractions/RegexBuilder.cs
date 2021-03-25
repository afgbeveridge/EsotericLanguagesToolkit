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

namespace Interpreter.Abstractions {
        public class RegexBuilder {
                static RegexBuilder() => RegisteredPatterns = new Dictionary<string, string>();

                public RegexBuilder() => Builder = new StringBuilder();

                public RegexBuilder Or => this.Fluently(() => Builder.Append("|"));

                private bool InCaptureGroup { get; set; }

                private StringBuilder Builder { get; }

                private static Dictionary<string, string> RegisteredPatterns { get; }

                public static RegexBuilder New() => new RegexBuilder();

                public RegexBuilder StartsWith() => this.Fluently(() => Builder.Append("^"));

                public RegexBuilder StartCaptureGroup(string name) {
                        if (InCaptureGroup) EndCaptureGroup();
                        InCaptureGroup = true;
                        return this.Fluently(() => Builder.Append("(?<").Append(name).Append(">"));
                }

                public RegexBuilder AddCharacterClass(string expr) =>
                        this.Fluently(() => Builder.Append("[").Append(expr).Append("]"));

                public RegexBuilder Optional(string expr, int max = 1) =>
                        this.Fluently(() => Builder.Append(expr).Append("{0,").Append(max).Append("}"));

                public RegexBuilder BoundedRepetition(int bound) =>
                        this.Fluently(() => Builder.Append("{").Append(bound).Append("}"));

                public RegexBuilder OneOrMore() => this.Fluently(() => Builder.Append("+"));

                public RegexBuilder Literal(string content) => this.Fluently(() => Builder.Append(content));

                public RegexBuilder OneFrom(IEnumerable<string> options) =>
                        this.Fluently(() => Builder.Append(string.Join("|", options)));

                public RegexBuilder EndCaptureGroup() {
                        InCaptureGroup = false;
                        return this.Fluently(() => Builder.Append(")"));
                }

                public RegexBuilder EndMatching() => this.Fluently(() => Builder.Append("$"));

                public RegexBuilder Escape => this.Fluently(() => Builder.Append("\\"));

                public RegexBuilder RememberAs(string key) =>
                        this.Fluently(() => RegisteredPatterns[key] = Builder.ToString());

                public RegexBuilder Include(string key) =>
                        this.Fluently(() => Builder.Append(RegisteredPatterns[key]));

                public RegexBuilder Reset() => this.Fluently(() => Builder.Length = 0);

                public Regex ToRegex() {
                        if (InCaptureGroup) EndCaptureGroup();
                        return new Regex(Builder.ToString());
                }

                public static Regex ToRegex(string key) => new Regex(RegisteredPatterns[key]);
        }
}