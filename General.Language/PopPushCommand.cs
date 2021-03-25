using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class PopPushCommand : LanguageCommand {
                public override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var obj = stack.Pop();
                        stack.Pop();
                        stack.Push(obj);
                        return Task.CompletedTask;
                }
        }
}