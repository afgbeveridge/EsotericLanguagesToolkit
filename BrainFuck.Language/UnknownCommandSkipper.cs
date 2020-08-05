using Interpreter.Abstractions;

namespace BrainFuck.Language {
        public class
                UnknownCommandSkipper : TrivialInterpreterBase<SimpleSourceCode, RandomAccessStack<CanonicalNumber>> {
                public override bool Applicable(InterpreterState state) {
                        var bldr = state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                .ScratchPadAs<CommandBuilder>(Constants.Builder);
                        return !bldr.Applicable(state.BaseSourceCode.CurrentCharacter());
                }

                public override BaseObject Gather(InterpreterState state) {
                        while (state.BaseSourceCode.More() && Applicable(state))
                                state.BaseSourceCode.Advance();
                        return NullObject.Instance;
                }
        }
}