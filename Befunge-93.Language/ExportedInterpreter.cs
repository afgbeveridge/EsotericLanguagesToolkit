using System.Reflection;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace Befunge_93.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SourceCodeTorus, BaseInterpreterStack>()
                                .ExecuteAsync(typeof(CommandBuilder).GetTypeInfo().Assembly, src,
                                        interp => {
                                                interp.State.GetSource<SourceCodeTorus>().Size =
                                                        Constants.StandardTorusSize;
                                                var env = interp.State.GetExecutionEnvironment<BaseInterpreterStack>();
                                                env.Interferer = new StackInterferer();
                                                env.ScratchPad[CommandBuilder.IOWrapperKey] = wrapper;
                                        }
                                );

                public string Language => "Befunge-93";

                public string Summary =>
                        "Befunge is a two-dimensional esoteric programming language invented in 1993 by Chris Pressey with the goal of being as difficult to compile as possible. Code is laid out on a two-dimensional grid of instructions, and execution can proceed in any direction of that grid";

                public string Url => "https://esolangs.org/wiki/Befunge";
        }
}