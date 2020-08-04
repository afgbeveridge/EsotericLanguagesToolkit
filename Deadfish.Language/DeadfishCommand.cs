using System;
using Interpreter.Abstractions;

namespace Deadfish.Language {
        internal class
                DeadfishCommand : BaseCommand<
                        Action<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>>> {
                internal DeadfishCommand(
                        Action<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>> cmd,
                        string keyWord)
                        : base(cmd, keyWord) { }

                protected override void Interpret(InterpreterState state) => Command(state,
                        state.GetSource<SimpleSourceCode>(),
                        state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>());

                public override object Clone() => new DeadfishCommand(Command, KeyWord);
        }
}