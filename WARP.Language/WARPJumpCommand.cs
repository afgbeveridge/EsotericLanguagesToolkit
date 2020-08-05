using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPJumpCommand : WARPCommand {
                internal static Regex LabelExpression;

                internal override async Task ExecuteAsync(InterpreterState state, SourceCode source,
                        BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        var env = Environment(state);
                        var val = (WARPObjectFactory.Instance.KnowsAbout(result.PropertyName)
                                ? WARPObjectFactory.Instance.Fabricate(state, result.PropertyName)
                                : (WARPObject) env[result.PropertyName]).AsNumeric(0L);
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
                        var targetChar = Constants.KeyWords.Label.First();
                        do {
                                more = code.More();
                                if (more) {
                                        code.Seek(targetChar);
                                        more = code.More() && code.Current() == Constants.KeyWords.Label;
                                        if (more)
                                                await Gather(state, Constants.KeyWords.Label,
                                                        Builder.Create(
                                                                (stat, src, st) =>
                                                                        new WARPLabelCommand().ExecuteAsync(stat, src, st),
                                                                WARPLabelCommand.SimpleLabel)).ApplyAsync(state);
                                }
                        } while (more);

                        code.SourcePosition = pos;
                }
        }
}