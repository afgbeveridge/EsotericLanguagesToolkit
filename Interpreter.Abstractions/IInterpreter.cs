using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public interface IInterpreter<out TSourceType, out TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                InterpreterState State { get; }
                bool StepMode { get; set; }
                Task<InterpreterResult> ExecuteAsync();
                BaseObject Gather();
        }
}