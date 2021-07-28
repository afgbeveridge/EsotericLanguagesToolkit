using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Eso.Blazor.SPA.Shared {
        public static class MarkdownConverter {

                private static Dictionary<string, Func<string, StringReader, string>> Converters { get; set; }

                private static string RegexGenerator(string anchor) => $"{anchor}(^{anchor.First()})+{anchor}";
                static MarkdownConverter() {
                        Converters = new Dictionary<string, Func<string, StringReader, string>> {
                                [RegexGenerator("''")] = (match, _) => $"*{match}*",
                                [RegexGenerator("'''")] = (match, _) => $"# {match}",
                                [RegexGenerator("==")] = (match, _) => $"## {match}",
                                ["\\{\\|\\sclass=\"wikitable\""] = TableReader
                        };
                }

                public static string ToMarkdown(string src)
                        => string.IsNullOrWhiteSpace(src) ? src : Convert(src);

                private static string Convert(string src) {
                        var buffer = new StringBuilder();
                        var keys = Converters.Keys.ToList();
                        using (var sr = new StringReader(src)) {
                                string line;
                                while ((line = sr.ReadLine()) != null) {
                                        keys.ForEach(k =>
                                             line = Regex.Replace(line, k, m => {
                                                     var match = m.Groups[1].Value;
                                                     return Converters[k](match, sr);
                                             }));
                                }
                        }
                        return src;
                }

                private static string TableReader(string match, StringReader reader) {
                        var line = default(string);
                        var cur = default(string);
                        const string sep = "|";
                        string Header() {
                                line = sep;
                                while ((cur = reader.ReadLine()) != null && cur.First() == '!') {
                                        line += cur[1..] + "|";
                                }
                                return line + Environment.NewLine;
                        }
                        string Rows() {
                                line = sep;
                                var inRow = true;
                                while ((cur = reader.ReadLine()) != null && !cur.StartsWith("|}")) {
                                        if (cur == "|-") {
                                                if (inRow) line += Environment.NewLine;
                                                inRow = !inRow;
                                        }
                                        line += cur + sep;
                                }
                                return line;
                        }
                        return Header() + Rows();
                }

        }
}
