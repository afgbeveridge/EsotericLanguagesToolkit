using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public class ActionCommand<TStack> : BaseCommand<Func<InterpreterState, SourceCode, TStack, Task>>
                where TStack : BaseInterpreterStack {
                public ActionCommand(Func<InterpreterState, SourceCode, TStack, Task> cmd, string keyWord)
                        : base(cmd, keyWord) =>
                        ExecutionContext = new Queue<BaseObject>();

                public Queue<BaseObject> ExecutionContext { get; }

                protected override async Task InterpretAsync(InterpreterState state) {
                        foreach (var obj in ExecutionContext) state.Stack<TStack>().Push(obj);

                        await Command(state, state.GetSource<SourceCode>(), state.GetExecutionEnvironment<TStack>());
                        Record();
                }

                public override object Clone() => new ActionCommand<TStack>(Command, KeyWord);
        }
}