using Eso.API.Editor.Models;
using Eso.API.Editor.Repos;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eso.API.Editor.Services {
        public class ExampleGenerator : IExampleGenerator {

                private const string ConceptMarkerRegex = "%%(.+)%%";

                private IEsoLangRepository Repo { get; }

                public ExampleGenerator(IEsoLangRepository repo) => Repo = repo;

                public IEnumerable<ExampleProgram> ProcessTemplates(string language, IEnumerable<string> paths) => 
                        paths.Select(p => ProcessTemplate(language, Path.GetFileNameWithoutExtension(p), File.ReadAllLines(p)));

                public string ProcessGeneralTemplate(string language, string path) => null;

                private ExampleProgram ProcessTemplate(string language, string name, string[] content) {
                        // Get commands using repo, do subs, join with empty string
                        var cmds = Repo.Get(language).Commands.ToDictionary(c => c.Concept, c => c.Keyword);
                        var all = content.Select(s => {
                                return Regex.Replace(s, ConceptMarkerRegex, m => {
                                        var match = m.Captures[0].Value;
                                        return cmds[match.Substring(2, match.Length - 4)];
                                });
                        });
                        return new ExampleProgram { Context = string.Join(string.Empty, all), Description = name };
                }

        }
}
