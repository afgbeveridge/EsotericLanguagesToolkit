using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Interpreter.Abstractions;
using static General.Language.Constants;

namespace General.Language {
        public abstract class LanguageCommand {

                protected Dictionary<KnownConcept, string> KeywordBindings { get; private set; }

                protected string Translate(KnownConcept concept) => KeywordBindings[concept];

                public abstract Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack);

                public LanguageCommand WithBindings(Dictionary<KnownConcept, string> bindings) {
                        KeywordBindings = bindings;
                        return this;
                }

                internal static dynamic PropertyNameAndExpression(BaseInterpreterStack stack) {
                        dynamic result = new ExpandoObject();
                        result.PropertyName = stack.Pop<LanguageObject>().AsString();
                        result.Expression = stack.Pop<LanguageObject>();
                        return result;
                }

                internal static ActionCommand<PropertyBasedExecutionEnvironment> Gather(InterpreterState state,
                        string key, Builder bld, Dictionary<KnownConcept, string> bindings) {
                        ExecutionSupport.Emit(() => string.Format("Command created: {0}, Source Position {1}", key,
                                state.Source().SourcePosition));
                        state.Source().Advance();
                        var cmd = new ActionCommand<PropertyBasedExecutionEnvironment>(bld.Action, key);
                        if (bld.Expression != null) {
                                var a = bld.Examine(state, key, bindings);
                                Action<LanguageObject> pushIfNonEmpty = o => {
                                        if (o != null && !string.IsNullOrEmpty(o.AsString()))
                                                cmd.ExecutionContext.Enqueue(o);
                                };
                                pushIfNonEmpty(a.RealizedObject);
                                pushIfNonEmpty(new LanguageObject(state.KnownRadix(), a.PropertyName));
                                ExecutionSupport.Emit(() => string.Concat("Command input parsed: ", a.Source));
                        }

                        return cmd;
                }

                protected PropertyBasedExecutionEnvironment Environment(InterpreterState state) =>
                        state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();
        }
}