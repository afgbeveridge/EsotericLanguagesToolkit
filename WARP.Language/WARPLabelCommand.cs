using System.Text.RegularExpressions;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPLabelCommand : WARPCommand {
                internal static Regex SimpleLabel;

                internal override void Execute(InterpreterState state, SourceCode source, BaseInterpreterStack stack) =>
                        Environment(state).ScratchPad[stack.Pop<WARPObject>().AsString()] =
                                source.SourcePosition.Copy();
        }
}