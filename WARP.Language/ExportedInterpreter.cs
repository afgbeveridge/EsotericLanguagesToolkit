using System.Reflection;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;
using General.Language;
using GeneralConstants = General.Language.Constants;
using System.Collections.ObjectModel;
using System.Linq;

namespace WARP.Language {
        public class ExportedInterpreter : IEsotericInterpreter {
                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SimpleSourceCode, PropertyBasedExecutionEnvironment>()
                                .ExecuteAsync(new[] { typeof(Language.CommandBuilder).Assembly, typeof(WhiteSpaceSkipper).Assembly }, 
                                        src,
                                        interp => {
                                                Bind(interp.KnownInterpreters);
                                                var env = interp.State
                                                        .GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();
                                                env.ScratchPad[GeneralConstants.RASName] =
                                                        new RandomAccessStack<LanguageObject> {
                                                                MaximumSize =
                                                                        ConfigurationSupport.ConfigurationFor<int>(
                                                                                "rasSize")
                                                        };
                                                env.ScratchPad[GeneralConstants.CurrentBase] = wrapper;
                                                env.ScratchPad[GeneralConstants.CurrentRadix] = FlexibleNumeralSystem.StandardRadix;
                                                env.OnUnknownKey = e => new LanguageObject(e.ScratchPadAs<int>(GeneralConstants.CurrentRadix));
                                        }
                                );

                public string Language => "WARP";

                public string Summary =>
                        "WARP is an object and stack based language, created by User:Aldous zodiac (talk) in May 2013. All numerics are signed, integral and expressed in hexatridecimal (base 36) notation, unless the radix system is changed within an executing program.";

                public string Url => "https://esolangs.org/wiki/WARP";

                private void Bind(ReadOnlyCollection<ITrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment>> handlers) {
                        handlers
                                .Select(h => h as General.Language.CommandBuilder)
                                .Where(h => h != null)
                                .ToList()
                                .ForEach(h => h.WithBindings(Constants.DefaultKeywordBindings).Initialize());
                } 
        }

}