namespace Interpreter.Abstractions {
        public abstract class
                TrivialInterpreterBase<TSourceType, TExeType> : ITrivialInterpreterBase<TSourceType, TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                public abstract bool Applicable(InterpreterState state);

                public abstract BaseObject Gather(InterpreterState state);

                public IInterpreter<TSourceType, TExeType> Interpreter { get; set; }
        }
}