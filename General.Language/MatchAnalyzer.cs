using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Interpreter.Abstractions;

namespace General.Language {
        internal class MatchAnalyzer {
                private List<Tuple<string, Func<InterpreterState, string, string>>> AnalysisHelpers { get; } = 
                        new List<Tuple<string, Func<InterpreterState, string, string>>>();


                internal MatchAnalyzer(string source, Dictionary<KnownConcept, string> bindings) { 
                        Source = source;
                        AnalysisHelpers.Add(Tuple.Create<string, Func<InterpreterState, string, string>>(
                                string.Concat("-", FlexibleNumeralSystem.CharList), (state, src) => src));
                        AnalysisHelpers.Add(
                                Tuple.Create<string, Func<InterpreterState, string, string>>(bindings[KnownConcept.Label], (state, src) => src));
                        AnalysisHelpers.Add(
                                Tuple.Create<string, Func<InterpreterState, string, string>>(bindings[KnownConcept.Jump], (state, src) => src));
                        AnalysisHelpers.Add(Tuple.Create<string, Func<InterpreterState, string, string>>("\"",
                                (state, src) => src.Substring(1, src.Length - 2)));
                        AnalysisHelpers.Add(Tuple.Create<string, Func<InterpreterState, string, string>>(bindings[KnownConcept.Pop],
                                (state, src) => state.Stack<BaseInterpreterStack>().Pop<LanguageObject>().AsString()));
                        AnalysisHelpers.Add(Tuple.Create<string, Func<InterpreterState, string, string>>(
                                "abcdefghijklmnopqrstuvwxyz",
                                (state, src) => {
                                        var b = state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()[src];
                                        return b == null ? string.Empty : ((LanguageObject)b).AsString();
                                }));
                        AnalysisHelpers.Add(Tuple.Create<string, Func<InterpreterState, string, string>>(bindings[KnownConcept.Quine],
                                (state, src) => string.Join(Environment.NewLine, state.Source().Content)));
                }

                internal string Source { get; set; }

                internal string PropertyName { get; private set; }

                internal LanguageObject RealizedObject { get; private set; }

                internal MatchAnalyzer Absorb(InterpreterState state, string cmd, Regex r) {
                        var m = r.Match(Source);
                        PropertyName = m.Groups["var"].Value;
                        if (m.Groups["expr"].Success) {
                                var expr = m.Groups["expr"].Value;
                                var helper = AnalysisHelpers.FirstOrDefault(t => t.Item1 == cmd) ??
                                             AnalysisHelpers.First(t => t.Item1.Contains(expr.Substring(0, 1)));
                                RealizedObject = new LanguageObject(state.KnownRadix(), helper.Item2(state, expr));
                        }

                        return this;
                }
        }
}