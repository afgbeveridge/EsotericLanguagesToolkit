
using Interpreter.Abstractions;

namespace General.Language {
        public static class InterpreterStateExtensions {

                public static int KnownRadix(this InterpreterState state) => 
                        state
                                .GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()
                                .ScratchPadAs<int>(Constants.CurrentRadix);

        }
}
