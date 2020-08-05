using System.Reflection;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace WARP.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SimpleSourceCode, PropertyBasedExecutionEnvironment>()
                                .ExecuteAsync(Assembly.GetExecutingAssembly(), src,
                                        interp => {
                                                var env = interp.State
                                                        .GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();
                                                env.ScratchPad[Constants.RASName] =
                                                        new RandomAccessStack<WARPObject> {
                                                                MaximumSize =
                                                                        ConfigurationSupport.ConfigurationFor<int>(
                                                                                "rasSize")
                                                        };
                                                env.ScratchPad[Constants.CurrentBase] = wrapper;
                                                env.OnUnknownKey = e => new WARPObject();
                                        }
                                );

                public string Language => "WARP";

                public string Summary =>
                        "WARP is an object and stack based language, created by User:Aldous zodiac (talk) in May 2013. All numerics are signed, integral and expressed in hexatridecimal (base 36) notation, unless the radix system is changed within an executing program.";

                public string Url => "https://esolangs.org/wiki/WARP";
        }
}