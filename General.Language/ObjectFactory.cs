using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Abstractions;
using static General.Language.Constants;

namespace General.Language {
        internal class ObjectFactory {

                private readonly Dictionary<string, Func<InterpreterState, LanguageObject>> Handlers =
                        new Dictionary<string, Func<InterpreterState, LanguageObject>>();

                internal ObjectFactory(Dictionary<KnownConcept, string> bindings) {
                        Handlers[bindings[KnownConcept.Period]] = s => LanguageObject.Mutate(KnownRadix(s), "1");
                        Handlers[bindings[KnownConcept.CurrentStack]] = s =>
                                LanguageObject.Mutate(KnownRadix(s), s.Stack<PropertyBasedExecutionEnvironment>().Size.ToString());
                        Handlers[bindings[KnownConcept.Quine]] = s => LanguageObject.Mutate(KnownRadix(s), s.Source().Content.Sum(l => l.Length).ToString());
                        Handlers[bindings[KnownConcept.Pop]] = s =>
                                LanguageObject.Mutate(KnownRadix(s), s.Stack<PropertyBasedExecutionEnvironment>().Pop<LanguageObject>()
                                        .AsString());
                }

                private static int KnownRadix(InterpreterState state) => state.KnownRadix();

                internal bool KnowsAbout(string symbol) => Handlers.ContainsKey(symbol);

                internal LanguageObject Fabricate(InterpreterState state, string symbol) => Handlers[symbol](state);
        }
}