using System.Reflection;
using Common;
using Interpreter.Abstractions;

namespace WARP.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                public void Interpret(IOWrapper wrapper, string[] src) =>
                        new BasicInterpreter<SimpleSourceCode, PropertyBasedExecutionEnvironment>()
                                .Execute(Assembly.GetExecutingAssembly(), src,
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