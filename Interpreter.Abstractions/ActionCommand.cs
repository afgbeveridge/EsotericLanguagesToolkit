using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public class ActionCommand<TStack> : BaseCommand<Action<InterpreterState, SourceCode, TStack>>
                where TStack : BaseInterpreterStack {
                public ActionCommand(Action<InterpreterState, SourceCode, TStack> cmd, string keyWord)
                        : base(cmd, keyWord) =>
                        ExecutionContext = new Queue<BaseObject>();

                public Queue<BaseObject> ExecutionContext { get; }

                protected override void Interpret(InterpreterState state) {
                        foreach (var obj in ExecutionContext) state.Stack<TStack>().Push(obj);

                        Command(state, state.GetSource<SourceCode>(), state.GetExecutionEnvironment<TStack>());
                        Record();
                }

                public override object Clone() => new ActionCommand<TStack>(Command, KeyWord);
        }
}