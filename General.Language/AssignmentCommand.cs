using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class AssignmentCommand : LanguageCommand {
                public override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        if (result.PropertyName == Translate(KnownConcept.Pop))
                                Environment(state).Push(result.Expression);
                        else
                                Environment(state)[result.PropertyName] = result.Expression;
                        return Task.CompletedTask;
                }
        }
}