using System.Reflection;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace Deadfish.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SimpleSourceCode, RandomAccessStack<CanonicalNumber>> { RetainEOL = false }
                                .IgnoreUnknownCommands()
                                .ExecuteAsync(typeof(CommandBuilder).GetTypeInfo().Assembly, src,
                                        interp => {
                                                var env = interp.State
                                                        .GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>();
                                                env.ScratchPad[CommandBuilder.IOWrapperKey] = wrapper;
                                                interp.State
                                                        .GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                                        .MaximumSize = 1;
                                        }
                                );

                public string Language => "Deadfish";

                public string Summary =>
                        "Deadfish is a very odd interpreted programming language created by Jonathan Todd Skinner. It was released under public domain and was originally programmed in C, but has since been ported to many other programming languages.";

                public string Url => "https://esolangs.org/wiki/Deadfish";
        }
}