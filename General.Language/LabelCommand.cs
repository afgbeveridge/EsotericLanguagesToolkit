using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class LabelCommand : LanguageCommand {

                public static Regex SimpleLabel;

                public override Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        Environment(state).ScratchPad[stack.Pop<LanguageObject>().AsString()] =
                                source.SourcePosition.Copy();
                        return Task.CompletedTask;
                }
        }
}