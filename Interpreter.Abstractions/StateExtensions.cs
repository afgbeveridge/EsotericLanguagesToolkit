namespace Interpreter.Abstractions {
        public static class StateExtensions {
                public static SimpleSourceCode Source(this InterpreterState state) =>
                        state.GetSource<SimpleSourceCode>();

                public static T Stack<T>(this InterpreterState state) where T : BaseInterpreterStack =>
                        state.GetExecutionEnvironment<T>();
        }
}