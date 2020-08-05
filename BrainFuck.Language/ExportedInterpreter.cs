using System.Reflection;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace BrainFuck.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                private const int StandardMaxCellCount = 30000;

                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SimpleSourceCode, RandomAccessStack<CanonicalNumber>>()
                                .ExecuteAsync(typeof(CommandBuilder).GetTypeInfo().Assembly, src,
                                        interp => {
                                                var env = interp.State
                                                        .GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>();
                                                env.ScratchPad[Constants.CurrentBase] = wrapper;
                                                interp.State
                                                        .GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                                        .MaximumSize = StandardMaxCellCount;
                                        }
                                );

                public string Language => "Brainfuck";

                public string Summary =>
                        "Brainfuck is the most famous esoteric programming language, and has inspired the creation of a host of other languages. Due to the fact that the last half of its name is often considered one of the most offensive words in the English language, it is sometimes referred to as brainf***, brainf*ck, brainfsck, b****fuck, brainf**k or BF. This can make it a bit difficult to search for information regarding brainfuck on the web, as the proper name might not be used at all in some articles.";

                public string Url => "https://esolangs.org/wiki/Brainfuck";
        }
}