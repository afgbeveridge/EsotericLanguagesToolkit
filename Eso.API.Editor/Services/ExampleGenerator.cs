﻿using Eso.API.Editor.Models;
using Eso.API.Editor.Repos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eso.API.Editor.Services {
        public class ExampleGenerator : IExampleGenerator {

                private const string ConceptMarkerRegex = "%%[^%]+%%";

                private IEsoLangRepository Repo { get; }

                public ExampleGenerator(IEsoLangRepository repo) => Repo = repo;

                public IEnumerable<ExampleProgram> ProcessTemplates(string language, IEnumerable<string> paths, bool retainEOL = false) =>
                        paths.Select(p => ProcessTemplate(language, Path.GetFileNameWithoutExtension(p), File.ReadAllLines(p), retainEOL));

                public DocumentSet ProcessGeneralTemplate(string language, string path, bool retainEOL = false) =>
                        new DocumentSet { Raw = File.ReadAllText(path), Processed = ProcessTemplate(language, null, File.ReadAllLines(path), retainEOL).Context };

                public DocumentSet ProcessGeneralTemplate(Language lang, bool retainEOL = false) =>
                        new DocumentSet { Raw = lang.Documentation, Processed = ProcessTemplate(lang, retainEOL) };

                private ExampleProgram ProcessTemplate(string language, string name, string[] content, bool retainEOL = false) {
                        // Get commands using repo, do subs, join with empty string
                        var cmds = Repo.Get(language).Commands.ToDictionary(c => c.Concept, c => c.Keyword);
                        var all = content.Select(s => {
                                return Regex.Replace(s, ConceptMarkerRegex, m => {
                                        var match = m.Captures[0].Value;
                                        return cmds[match[2..^2]];
                                });
                        });
                        return new ExampleProgram { Context = string.Join(!retainEOL ? string.Empty : Environment.NewLine, all), Description = name };
                }

                private string ProcessTemplate(Language language, bool retainEOL = false) {
                        var cmds = language.Commands.ToDictionary(c => c.Concept, c => c.Keyword);
                        return Regex.Replace(language.Documentation, ConceptMarkerRegex, m => {
                                var match = m.Captures[0].Value;
                                return cmds[match[2..^2]];
                        });
                }

        }
}
