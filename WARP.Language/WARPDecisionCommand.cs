using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPDecisionCommand : WARPCommand {
                internal override async Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var lhs = stack.Pop<WARPObject>().AsNumeric();
                        var rhs = stack.Pop<WARPObject>().AsNumeric();
                        var res = Environment(state).ScratchPadAs<CommandBuilder>(Constants.Builder)
                                .KeyAndBuilder(state);
                        ActionCommand<PropertyBasedExecutionEnvironment> cmd = Gather(state, res.Key, res.Builder);
                        ExecutionSupport.Emit(() => string.Concat("Comparison: ", lhs, " == ", rhs, "?"));
                        if (lhs == rhs)
                                await cmd.ApplyAsync(state);
                }
        }
}