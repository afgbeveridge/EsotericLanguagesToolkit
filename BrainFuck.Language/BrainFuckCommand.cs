using System;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace BrainFuck.Language {
        internal class
                BrainFuckCommand : BaseCommand<
                        Func<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>, Task>> {
                internal BrainFuckCommand(
                        Func<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>, Task> cmd,
                        string keyWord)
                        : base(cmd, keyWord) { }

                protected override Task InterpretAsync(InterpreterState state) => Command(state,
                        state.GetSource<SimpleSourceCode>(),
                        state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>());

                public override object Clone() => new BrainFuckCommand(Command, KeyWord);
        }
}