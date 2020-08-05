using System;
using System.Text.RegularExpressions;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class Builder {
                internal Regex Expression { get; set; }

                internal Action<InterpreterState, SourceCode, BaseInterpreterStack> Action { get; set; }

                internal static Builder Null { get; } = new Builder { Action = (s, c, e) => { } };

                internal static Builder Create(Action<InterpreterState, SourceCode, BaseInterpreterStack> action,
                        Regex expr = null) => new Builder { Expression = expr, Action = action };

                internal static Builder Inactive(Regex expr) => new Builder { Expression = expr };

                internal MatchAnalyzer Examine(InterpreterState state, string key) {
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
                        return new MatchAnalyzer(input).Absorb(state, key, Expression);
                }
        }
}