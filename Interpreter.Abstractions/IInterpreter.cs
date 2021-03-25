using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public interface IInterpreter<TSourceType, TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                InterpreterState State { get; }
                bool StepMode { get; set; }
                Task<InterpreterResult> ExecuteAsync();
                BaseObject Gather();

                ReadOnlyCollection<ITrivialInterpreterBase<TSourceType, TExeType>> KnownInterpreters { get; }
        }
}