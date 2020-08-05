using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace Befunge_93 {
        internal class Befunge93Command : BaseCommand<Func<InterpreterState, SourceCodeTorus, BaseInterpreterStack, Task>> {
                internal Befunge93Command(Func<InterpreterState, SourceCodeTorus, BaseInterpreterStack, Task> cmd,
                        string keyWord)
                        : base(cmd, keyWord) { }

                protected override async Task InterpretAsync(InterpreterState state) => await Command(state,
                        state.GetSource<SourceCodeTorus>(), state.GetExecutionEnvironment<BaseInterpreterStack>());

                public override object Clone() => new Befunge93Command(Command, KeyWord);
        }
}