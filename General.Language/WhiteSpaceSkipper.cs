using System.Linq;
using Interpreter.Abstractions;

namespace General.Language {
        public class WhiteSpaceSkipper : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                public override bool Applicable(InterpreterState state) =>
                        char.IsWhiteSpace(state.Source().Current().First());

                public override BaseObject Gather(InterpreterState state) {
                        while (state.Source().More() && Applicable(state))
                                state.Source().Advance();
                        return NullObject.Instance;
                }
        }
}