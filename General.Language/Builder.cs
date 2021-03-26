using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class Builder {
                internal Regex Expression { get; set; }

                internal Func<InterpreterState, SourceCode, BaseInterpreterStack, Task> Action { get; set; }

                internal static Builder Null { get; } = new Builder { Action = (s, c, e) => Task.CompletedTask };

                public static Builder Create(Action<InterpreterState, SourceCode, BaseInterpreterStack> action,
                                               Regex expr = null) => 
                        new Builder { 
                                Expression = expr, 
                                Action = (state, source, stack) => {
                                        action(state, source, stack);
                                        return Task.CompletedTask;
                                }
                };

                public static Builder Create(Func<InterpreterState, SourceCode, BaseInterpreterStack, Task> asyncAction,
                        Regex expr = null) => new Builder { Expression = expr, Action = asyncAction };

                internal static Builder Inactive(Regex expr) => new Builder { Expression = expr };

                internal MatchAnalyzer Examine(InterpreterState state, string key, Dictionary<KnownConcept, string> bindings) {
                        var input = state.Source().Current();
                        var hadEnough = false;
                        while (state.Source().More() && !hadEnough) {
                                var currentIsMatch = Expression.Match(input).Success;
                                var followingIsMatch = Expression.Match(string.Concat(input, state.Source().Peek()))
                                        .Success;
                                if (!currentIsMatch || followingIsMatch)
                                        if (state.Source().Advance())
                                                input = string.Concat(input, state.Source().Current());
                                hadEnough = currentIsMatch && !followingIsMatch;
                        }

                        if (state.Source().More()) state.Source().Advance();
                        return new MatchAnalyzer(input, bindings).Absorb(state, key, Expression);
                }
        }
}