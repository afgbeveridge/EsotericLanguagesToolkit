using System.Linq;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class TreatmentCommand : LanguageCommand {
                public override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var val = ((LanguageObject) Environment(state)[stack.Pop<LanguageObject>().AsString()]).AsString();
                        state.AddExecutionEnvironment(Environment(state).Clone());
                        val.Reverse().ToList()
                                .ForEach(c => Environment(state).Push(new LanguageObject(state.KnownRadix(), new string(new[] { c }))));
                        return Task.CompletedTask;
                }
        }
}