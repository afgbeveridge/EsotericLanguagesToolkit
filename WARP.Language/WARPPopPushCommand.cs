using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPPopPushCommand : WARPCommand {
                internal override void Execute(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var obj = stack.Pop();
                        stack.Pop();
                        stack.Push(obj);
                }
        }
}