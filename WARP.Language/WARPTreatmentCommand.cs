using System.Linq;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPTreatmentCommand : WARPCommand {
                internal override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var val = ((WARPObject) Environment(state)[stack.Pop<WARPObject>().AsString()]).AsString();
                        state.AddExecutionEnvironment(Environment(state).Clone());
                        val.Reverse().ToList()
                                .ForEach(c => Environment(state).Push(new WARPObject(new string(new[] { c }))));
                        return Task.CompletedTask;
                }
        }
}