using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPAssignmentCommand : WARPCommand {
                internal override Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        if (result.PropertyName == Constants.KeyWords.Pop)
                                Environment(state).Push(result.Expression);
                        else
                                Environment(state)[result.PropertyName] = result.Expression;
                        return Task.CompletedTask;
                }
        }
}