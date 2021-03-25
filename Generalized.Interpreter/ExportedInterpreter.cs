using Common;
using General.Language;
using Interpreter.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GeneralConstants = General.Language.Constants;

namespace Generalized.Interpreter {
        public class ExportedInterpreter : IEsotericInterpreter, IGeneralizedInterpreter {
                public async Task InterpretAsync(IOWrapper wrapper, string[] src) =>
                        await new BasicInterpreter<SimpleSourceCode, PropertyBasedExecutionEnvironment>()
                                .ExecuteAsync(new[] { typeof(CommandBuilder).Assembly, typeof(WhiteSpaceSkipper).Assembly },
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

                public string Language => "*";

                public string Summary => null;

                public string Url => null;

                private void Bind(ReadOnlyCollection<ITrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment>> handlers) {
                        handlers
                                .Select(h => h as General.Language.CommandBuilder)
                                .Where(h => h != null)
                                .ToList()
                                .ForEach(h => h.Reset().WithBindings(Bindings).Initialize()); 
                }

                private Dictionary<KnownConcept, string> Bindings { get; set; }

                public void UseBindings(Dictionary<KnownConcept, string> bindings) => Bindings = bindings; 
        }

}