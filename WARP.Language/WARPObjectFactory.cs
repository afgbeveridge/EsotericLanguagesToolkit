using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPObjectFactory {
                private static readonly Dictionary<string, Func<InterpreterState, WARPObject>> Handlers =
                        new Dictionary<string, Func<InterpreterState, WARPObject>>();

                static WARPObjectFactory() {
                        Handlers["."] = s => WARPObject.Mutate(KnownRadix(s), "1");
                        Handlers["_"] = s =>
                                WARPObject.Mutate(KnownRadix(s), s.Stack<PropertyBasedExecutionEnvironment>().Size.ToString());
                        Handlers["~"] = s => WARPObject.Mutate(KnownRadix(s), s.Source().Content.Sum(l => l.Length).ToString());
                        Handlers["!"] = s =>
                                WARPObject.Mutate(KnownRadix(s), s.Stack<PropertyBasedExecutionEnvironment>().Pop<WARPObject>()
                                        .AsString());
                }

                private static int KnownRadix(InterpreterState state) => state.KnownRadix();

                private WARPObjectFactory() { }

                internal static WARPObjectFactory Instance { get; } = new WARPObjectFactory();

                internal bool KnowsAbout(string symbol) => Handlers.ContainsKey(symbol);

                internal WARPObject Fabricate(InterpreterState state, string symbol) => Handlers[symbol](state);
        }
}