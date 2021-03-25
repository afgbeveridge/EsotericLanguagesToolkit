using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace General.Language {
        public class JumpCommand : LanguageCommand {

                private ObjectFactory Factory { get; set; }

                public static Regex LabelExpression;

                internal JumpCommand WithObjectFactory(ObjectFactory fac) {
                        Factory = fac;
                        return this;
                }

                public override async Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        var env = Environment(state);
                        var val = (Factory.KnowsAbout(result.PropertyName)
                                ? Factory.Fabricate(state, result.PropertyName)
                                : (LanguageObject) env[result.PropertyName]).AsNumeric(0L);
                        if (val > 0) {
                                if (!env.HasScratchPadEntry(result.Expression.AsString()))
                                        await FindAllLabels(state);
                                ExecutionSupport.Assert(env.HasScratchPadEntry(result.Expression.AsString()),
                                        string.Concat("Unknown label: ", result.Expression.AsString()));
                                source.SourcePosition =
                                        ((MutableTuple<int>) Environment(state)
                                                .ScratchPad[result.Expression.AsString()]).Copy();
                        }
                }

                private async Task FindAllLabels(InterpreterState state) {
                        var more = true;
                        var code = state.GetSource<SimpleSourceCode>();
                        var pos = code.SourcePosition.Copy();
                        var labelDesignator = Translate(KnownConcept.Label);
                        var targetChar = labelDesignator.First();
                        do {
                                more = code.More();
                                if (more) {
                                        code.Seek(targetChar);
                                        more = code.More() && code.Current() == labelDesignator;
                                        if (more)
                                                await Gather(state, labelDesignator,
                                                        Builder.Create(
                                                                (stat, src, st) =>
                                                                        new LabelCommand().ExecuteAsync(stat, src, st),
                                                                LabelCommand.SimpleLabel)).ApplyAsync(state);
                                }
                        } while (more);

                        code.SourcePosition = pos;
                }
        }
}