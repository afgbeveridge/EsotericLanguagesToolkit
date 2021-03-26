using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class DecisionCommand : LanguageCommand {
                public override async Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var lhs = stack.Pop<LanguageObject>().AsNumeric();
                        var rhs = stack.Pop<LanguageObject>().AsNumeric();
                        var res = Environment(state).ScratchPadAs<CommandBuilder>(Constants.Builder)
                                .KeyAndBuilder(state);
                        ActionCommand<PropertyBasedExecutionEnvironment> cmd = Gather(state, res.Key, res.Builder, KeywordBindings);
                        ExecutionSupport.Emit(() => string.Concat("Comparison: ", lhs, " == ", rhs, "?"));
                        if (lhs == rhs)
                                await cmd.ApplyAsync(state);
                }
        }
}