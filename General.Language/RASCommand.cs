using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class RASCommand : LanguageCommand {
                public RASCommand() { }

                public RASCommand(Action<RandomAccessStack<LanguageObject>, BaseInterpreterStack> action) =>
                        Action = action;

                private Action<RandomAccessStack<LanguageObject>, BaseInterpreterStack> Action { get; }

                public override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var st = Environment(state).ScratchPad[Constants.RASName] as RandomAccessStack<LanguageObject>;
                        st.Set((int) stack.Pop<LanguageObject>().AsNumeric());
                        Action(st, stack);
                        return Task.CompletedTask;
                }
        }
}