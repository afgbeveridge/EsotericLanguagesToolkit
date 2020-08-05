using System;
using System.Dynamic;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal abstract class WARPCommand {
                internal abstract Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack);

                internal static dynamic PropertyNameAndExpression(BaseInterpreterStack stack) {
                        dynamic result = new ExpandoObject();
                        result.PropertyName = stack.Pop<WARPObject>().AsString();
                        result.Expression = stack.Pop<WARPObject>();
                        return result;
                }

                internal static ActionCommand<PropertyBasedExecutionEnvironment> Gather(InterpreterState state,
                        string key, Builder bld) {
                        ExecutionSupport.Emit(() => string.Format("Command created: {0}, Source Position {1}", key,
                                state.Source().SourcePosition));
                        state.Source().Advance();
                        var cmd = new ActionCommand<PropertyBasedExecutionEnvironment>(bld.Action, key);
                        if (bld.Expression != null) {
                                var a = bld.Examine(state, key);
                                Action<WARPObject> pushIfNonEmpty = o => {
                                        if (o != null && !string.IsNullOrEmpty(o.AsString()))
                                                cmd.ExecutionContext.Enqueue(o);
                                };
                                pushIfNonEmpty(a.RealizedObject);
                                pushIfNonEmpty(new WARPObject(a.PropertyName));
                                ExecutionSupport.Emit(() => string.Concat("Command input parsed: ", a.Source));
                        }

                        return cmd;
                }

                protected PropertyBasedExecutionEnvironment Environment(InterpreterState state) =>
                        state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();
        }
}