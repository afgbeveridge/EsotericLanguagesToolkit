using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace Deadfish.Language {

        internal class DeadfishCommand : BaseCommand<
                        Func<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>, Task>> {
                internal DeadfishCommand(
                        Func<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>, Task> cmd,
                        string keyWord)
                        : base(cmd, keyWord) { }

                protected override Task InterpretAsync(InterpreterState state) => Command(state,
                        state.GetSource<SimpleSourceCode>(),
                        state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>());

                public override object Clone() => new DeadfishCommand(Command, KeyWord);
        }
}