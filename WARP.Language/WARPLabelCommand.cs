using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPLabelCommand : WARPCommand {
                internal static Regex SimpleLabel;

                internal override Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        Environment(state).ScratchPad[stack.Pop<WARPObject>().AsString()] =
                                source.SourcePosition.Copy();
                        return Task.CompletedTask;
                }
        }
}