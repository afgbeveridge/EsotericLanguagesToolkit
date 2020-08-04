namespace Interpreter.Abstractions {
        public interface IInterpreter<out TSourceType, out TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                InterpreterState State { get; }
                bool StepMode { get; set; }
                InterpreterResult Execute();
                BaseObject Gather();
        }
}