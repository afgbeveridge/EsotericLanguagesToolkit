using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPRASCommand : WARPCommand {
                public WARPRASCommand() { }

                internal WARPRASCommand(Action<RandomAccessStack<WARPObject>, BaseInterpreterStack> action) =>
                        Action = action;

                private Action<RandomAccessStack<WARPObject>, BaseInterpreterStack> Action { get; }

                internal override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var st = Environment(state).ScratchPad[Constants.RASName] as RandomAccessStack<WARPObject>;
                        st.Set((int) stack.Pop<WARPObject>().AsNumeric());
                        Action(st, stack);
                        return Task.CompletedTask;
                }
        }
}