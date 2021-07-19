using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Eso.Blazor.SPA.Shared {
        public static class MarkdownConverter {

                private static Dictionary<string, Func<string, string>> Converters { get; private set; }

                private static string RegexGenerator(string anchor) => $"{anchor}(^{anchor.First()})+{anchor}";
                static MarkdownConverter() {
                        Converters = new Dictionary<string, Func<string, string>> {
                                [RegexGenerator("''")] = (match) => $"*{match}*",
                                [RegexGenerator("'''")] = (match) => $"# {match}",
                                [RegexGenerator("==")] = (match) => $"## {match}"
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
                                           return Converters[k](match);
                                   }));    
                                }
                        }
                        return src;
                }

        }
}
