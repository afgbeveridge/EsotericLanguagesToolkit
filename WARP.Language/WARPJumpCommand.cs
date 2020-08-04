using System.Linq;
using System.Text.RegularExpressions;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPJumpCommand : WARPCommand {
                internal static Regex LabelExpression;

                internal override void Execute(InterpreterState state, SourceCode source, BaseInterpreterStack stack) {
                        var result = PropertyNameAndExpression(stack);
                        var env = Environment(state);
                        var val = (WARPObjectFactory.Instance.KnowsAbout(result.PropertyName)
                                ? WARPObjectFactory.Instance.Fabricate(state, result.PropertyName)
                                : (WARPObject) env[result.PropertyName]).AsNumeric(0L);
                        if (val > 0) {
                                if (!env.HasScratchPadEntry(result.Expression.AsString()))
                                        FindAllLabels(state);
                                ExecutionSupport.Assert(env.HasScratchPadEntry(result.Expression.AsString()),
                                        string.Concat("Unknown label: ", result.Expression.AsString()));
                                source.SourcePosition =
                                        ((MutableTuple<int>) Environment(state)
                                                .ScratchPad[result.Expression.AsString()]).Copy();
                        }
                }

                private void FindAllLabels(InterpreterState state) {
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
                                                Gather(state, Constants.KeyWords.Label,
                                                        Builder.Create(
                                                                (stat, src, st) =>
                                                                        new WARPLabelCommand().Execute(stat, src, st),
                                                                WARPLabelCommand.SimpleLabel)).Apply(state);
                                }
                        } while (more);

                        code.SourcePosition = pos;
                }
        }
}