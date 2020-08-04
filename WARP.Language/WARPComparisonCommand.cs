using System;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPComparisonCommand : WARPCommand {
                internal override void Execute(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var obj = stack.Pop<WARPObject>();
                        var env = Environment(state);
                        if (!env.HasScratchPadEntry(Constants.KeyWords.Comparison)) {
                                stack.Push(obj);
                                Environment(state).ScratchPad[Constants.KeyWords.Comparison] = string.Empty;
                        }
                        else {
                                var lhs = stack.Pop<WARPObject>();
                                Environment(state).ScratchPad.Remove(Constants.KeyWords.Comparison);
                                var bothNumeric = lhs.IsNumeric && obj.IsNumeric;
                                Func<int> cmp = () => {
                                        var f = lhs.AsNumeric();
                                        var s = obj.AsNumeric();
                                        return f < s ? -1 : f > s ? 1 : 0;
                                };
                                stack.Push(new WARPObject(bothNumeric
                                        ? cmp()
                                        : string.Compare(lhs.AsString(), obj.AsString())));
                        }
                }
        }
}