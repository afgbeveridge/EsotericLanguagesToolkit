using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using Common;
using Interpreter.Abstractions;

namespace WARP.Language {
        public class CommandBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private readonly Dictionary<string, Builder> Commands = new Dictionary<string, Builder>();
                private Regex BoundVariableExpression;
                private Regex BoundVariableOrStackExpression;

                private Regex Expression;
                private Regex ObjectReference;

                public CommandBuilder() => Initialize();

                public IOWrapper Wrapper { get; set; }

                public void Initialize() {
                        CreateExpressions();
                        Commands["="] =
                                Builder.Create(
                                        (state, source, stack) =>
                                                Get<WARPAssignmentCommand>().ExecuteAsync(state, source, stack),
                                        BoundVariableExpression);
                        Commands[Constants.KeyWords.Addition] = Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur + expr.AsNumeric(),
                                                Constants.KeyWords.Addition).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression);
                        Commands[Constants.KeyWords.Subtraction] = Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur - expr.AsNumeric(),
                                                Constants.KeyWords.Subtraction).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression);
                        Commands[Constants.KeyWords.Multiplication] = Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur * expr.AsNumeric(1L),
                                                Constants.KeyWords.Multiplication).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression);
                        Commands[Constants.KeyWords.Division] = Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur / expr.AsNumeric(1L),
                                                Constants.KeyWords.Division).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression);
                        Commands[Constants.KeyWords.Modulo] = Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur % expr.AsNumeric(1L),
                                                Constants.KeyWords.Modulo).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression);
                        Commands[Constants.KeyWords.Pop] = Builder.Create((state, source, stack) => stack.Pop());
                        Commands["*"] = Builder.Create((state, source, stack) => stack.Push(stack.Pop<WARPObject>()),
                                Expression);
                        Commands[";"] = Builder.Create((state, source, stack) => stack.Duplicate());
                        Commands["+"] = Builder.Create(
                                (state, source, stack) => state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>().ScratchPad[Constants.CurrentRadix] =
                                        Convert.ToInt32(stack.Pop<WARPObject>().AsNumeric()),
                                RegexBuilder.New().StartCaptureGroup("expr").AddCharacterClass("0-9A-Z").OneOrMore()
                                        .EndCaptureGroup().EndMatching().ToRegex());
                        Commands["]"] =
                                Builder.Create(
                                        (state, source, stack) =>
                                                Get<WARPPopPushCommand>().ExecuteAsync(state, source, stack), Expression);
                        Commands[")"] =
                                Builder.Create(
                                        (state, source, stack) => Wrapper.WriteAsync(stack.Pop<WARPObject>().AsString()
                                                .Replace("\\n", System.Environment.NewLine)), Expression);
                        Commands["("] =
                                Builder.Create(
                                        (state, source, stack) =>
                                                Wrapper.WriteAsync(stack.Pop<WARPObject>().AsCharacter()), Expression);
                        Commands[","] = Builder.Create(
                                (state, source, stack) => Get<WARPInputCommand>().WithWrapper(Wrapper)
                                        .ExecuteAsync(state, source, stack),
                                RegexBuilder.New().StartsWith().StartCaptureGroup("var")
                                        .OneFrom(WARPInputCommand.Options).EndCaptureGroup().EndMatching().ToRegex());
                        Commands["|"] = Builder.Create((state, source, stack) =>
                                state.PopExecutionEnvironment<PropertyBasedExecutionEnvironment>());
                        Commands["'"] = Builder.Create((state, source, stack) =>
                                state.RotateExecutionEnvironment<PropertyBasedExecutionEnvironment>());
                        Commands[Constants.KeyWords.Label] = Builder.Create(
                                (state, source, stack) => Get<WARPLabelCommand>().ExecuteAsync(state, source, stack),
                                WARPLabelCommand.SimpleLabel);
                        Commands[Constants.KeyWords.Jump] = Builder.Create(
                                (state, source, stack) => Get<WARPJumpCommand>().ExecuteAsync(state, source, stack),
                                WARPJumpCommand.LabelExpression);
                        Commands["%"] =
                                Builder.Create(
                                        (state, source, stack) =>
                                                Get<WARPTreatmentCommand>().ExecuteAsync(state, source, stack),
                                        ObjectReference);
                        Commands["?"] =
                                Builder.Create(
                                        (state, source, stack) =>
                                                Get<WARPDecisionCommand>().ExecuteAsync(state, source, stack), Expression);
                        // :exp1:exp2 exp1 == exp2, push 0, exp1 < exp2 push -1, else push 1
                        Commands[Constants.KeyWords.Comparison] = Builder.Create(
                                (state, source, stack) => Get<WARPComparisonCommand>().ExecuteAsync(state, source, stack),
                                Expression);
                        // Treat an object as an array, take an element and push
                        Commands["{"] = Builder.Create((state, source, stack) =>
                                        CommandFactory
                                                .Get(() => new WARPRASCommand((st, stk) => stk.Push(st.CurrentCell)),
                                                        "_").ExecuteAsync(state, source, stack)
                                , RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex());
                        // Treat an object as an array, update the object noted at the index given with a value popped from the stack
                        Commands["}"] = Builder.Create((state, source, stack) =>
                                        CommandFactory
                                                .Get(
                                                        () => new WARPRASCommand((st, stk) =>
                                                                st.CurrentCell = stk.Pop<WARPObject>()), ":")
                                                .ExecuteAsync(state, source, stack),
                                RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex());
                }

                private static PropertyBasedExecutionEnvironment Environment(InterpreterState state) =>
                        state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();

                private static T Get<T>() where T : WARPCommand, new() => CommandFactory.Get<T>();

                private static WARPMathCommand CreateMathProcessor(Func<long, WARPObject, long> f, string qualifier) =>
                        CommandFactory.Get(() => new WARPMathCommand(f), qualifier);

                private void CreateExpressions() {
                        RegexBuilder.New()
                                .AddCharacterClass("a-z")
                                .BoundedRepetition(2)
                                .RememberAs("objectReference");
                        RegexBuilder.New()
                                .Optional("-")
                                .AddCharacterClass("0-9A-Z").OneOrMore()
                                .RememberAs("numeric");
                        RegexBuilder.New()
                                .StartCaptureGroup("var")
                                .Include("objectReference")
                                .EndCaptureGroup()
                                .RememberAs("objectCapture");
                        RegexBuilder.New()
                                .StartCaptureGroup("expr")
                                .Include("numeric")
                                .Or
                                .Include("objectReference")
                                .Or
                                .Literal("!")
                                .Or
                                .Literal("~")
                                .Or
                                .Literal("_")
                                .Or
                                .Literal("\"[^\"]*\"")
                                .EndCaptureGroup()
                                .RememberAs("expression");
                        ObjectReference = RegexBuilder.New().StartsWith().Include("objectCapture").EndMatching()
                                .ToRegex();
                        RegexBuilder.New().StartCaptureGroup("expr").AddCharacterClass("a-z").OneOrMore()
                                .EndCaptureGroup().RememberAs("label");
                        WARPLabelCommand.SimpleLabel =
                                RegexBuilder.New().StartsWith().Include("label").EndMatching().ToRegex();
                        Expression = RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex();
                        BoundVariableExpression = RegexBuilder.New().StartsWith().Include("objectCapture")
                                .Include("expression").EndMatching().ToRegex();
                        BoundVariableOrStackExpression = RegexBuilder.New().StartsWith().StartCaptureGroup("var")
                                .AddCharacterClass("a-z")
                                .BoundedRepetition(2).Or.Literal("!").EndCaptureGroup().Include("expression")
                                .EndMatching().ToRegex();
                        WARPJumpCommand.LabelExpression = RegexBuilder.New()
                                .StartsWith().StartCaptureGroup("var").Include("objectReference").Or.Literal("!").Or
                                .Literal("_").Or.Literal("\\.")
                                .EndCaptureGroup().Include("label").EndMatching().ToRegex();
                }

                public override bool Applicable(InterpreterState state) =>
                        Commands.ContainsKey(state.Source().Current());

                public override BaseObject Gather(InterpreterState state) {
                        Wrapper = state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()
                                .ScratchPadAs<IOWrapper>(Constants.CurrentBase);
                        state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>()
                                .ScratchPad[Constants.Builder] = this;
                        var res = KeyAndBuilder(state, false);
                        return WARPCommand.Gather(state, res.Key, res.Builder);
                }

                internal dynamic KeyAndBuilder(InterpreterState state, bool advance = true) {
                        dynamic result = new ExpandoObject();
                        if (advance) state.Source().Advance();
                        result.Key = state.Source().Current();
                        result.Builder = Commands.ContainsKey(result.Key) ? Commands[result.Key] : Builder.Null;
                        return result;
                }
        }
}