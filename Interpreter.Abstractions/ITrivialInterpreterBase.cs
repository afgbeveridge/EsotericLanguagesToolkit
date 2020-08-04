namespace Interpreter.Abstractions {
        public interface ITrivialInterpreterBase<TSourceType, TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                IInterpreter<TSourceType, TExeType> Interpreter { get; set; }

                bool Applicable(InterpreterState state);

                BaseObject Gather(InterpreterState state);
        }
}