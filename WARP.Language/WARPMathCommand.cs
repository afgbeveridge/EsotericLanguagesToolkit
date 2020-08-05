using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPMathCommand : WARPCommand {
                public WARPMathCommand() { }

                internal WARPMathCommand(Func<long, WARPObject, long> f) => Command = f;

                private Func<long, WARPObject, long> Command { get; }

                internal override Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        bool inPopMode = result.PropertyName == Constants.KeyWords.Pop;
                        var pbee = Environment(state);
                        long cur = inPopMode
                                ? stack.Pop<WARPObject>().AsNumeric()
                                : pbee[result.PropertyName].As<WARPObject>().AsNumeric();
                        var obj = new WARPObject(state.KnownRadix(), FlexibleNumeralSystem.Encode(Command(cur, result.Expression),
                                state.KnownRadix()));
                        if (result.PropertyName == Constants.KeyWords.Pop)
                                pbee.Push(obj);
                        else
                                pbee[result.PropertyName] = obj;
                        return Task.CompletedTask;
                }
        }
}