using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class ComparisonCommand : LanguageCommand {
                public override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var obj = stack.Pop<LanguageObject>();
                        var env = Environment(state);
                        if (!env.HasScratchPadEntry(Translate(KnownConcept.Comparison))) {
                                stack.Push(obj);
                                Environment(state).ScratchPad[Translate(KnownConcept.Comparison)] = string.Empty;
                        }
                        else {
                                var lhs = stack.Pop<LanguageObject>();
                                Environment(state).ScratchPad.Remove(Translate(KnownConcept.Comparison));
                                var bothNumeric = lhs.IsNumeric && obj.IsNumeric;
                                Func<int> cmp = () => {
                                        var f = lhs.AsNumeric();
                                        var s = obj.AsNumeric();
                                        return f < s ? -1 : f > s ? 1 : 0;
                                };
                                stack.Push(new LanguageObject(state.KnownRadix(), bothNumeric
                                        ? cmp()
                                        : string.Compare(lhs.AsString(), obj.AsString())));
                        }
                        return Task.CompletedTask;
                }
        }
}