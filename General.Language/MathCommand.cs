using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class MathCommand : LanguageCommand {
                public MathCommand() { }

                internal MathCommand(Func<long, LanguageObject, long> f) => Command = f;

                private Func<long, LanguageObject, long> Command { get; }

                public override Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        bool inPopMode = result.PropertyName == Translate(KnownConcept.Pop);
                        var pbee = Environment(state);
                        long cur = inPopMode
                                ? stack.Pop<LanguageObject>().AsNumeric()
                                : pbee[result.PropertyName].As<LanguageObject>().AsNumeric();
                        var obj = new LanguageObject(state.KnownRadix(), FlexibleNumeralSystem.Encode(Command(cur, result.Expression),
                                state.KnownRadix()));
                        if (result.PropertyName == Translate(KnownConcept.Pop))
                                pbee.Push(obj);
                        else
                                pbee[result.PropertyName] = obj;
                        return Task.CompletedTask;
                }
        }
}