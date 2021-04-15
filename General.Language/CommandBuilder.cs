using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using Common;
using Interpreter.Abstractions;
using static General.Language.Constants;

namespace General.Language {
        public abstract class CommandBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                
                private readonly Dictionary<string, Builder> Commands = new Dictionary<string, Builder>();
                private ObjectFactory Factory { get; set; }

                protected Regex BoundVariableExpression;
                protected Regex BoundVariableOrStackExpression;

                protected Regex Expression;
                protected Regex ObjectReference;

                protected Dictionary<KnownConcept, string> KeywordBindings { get; set; }

                public CommandBuilder() { }

                public IOWrapper Wrapper { get; set; }

                public CommandBuilder Reset() => this.Fluently(() => KeywordBindings = new Dictionary<KnownConcept, string>());

                public CommandBuilder WithBindings(Dictionary<KnownConcept, string> bindings) => this.Fluently(() => KeywordBindings = bindings);

                public virtual CommandBuilder Initialize() =>
                        this.Fluently(() => {
                                CreateExpressions();
                                Factory = new ObjectFactory(KeywordBindings);
                                BuildCommands();
                        });

                private void BuildCommands() {
                        AddIfDefined(KnownConcept.Assignment,
                               () => Builder.Create(
                                       (state, source, stack) =>
                                               Get<AssignmentCommand>().ExecuteAsync(state, source, stack),
                                               BoundVariableExpression));
                        AddIfDefined(KnownConcept.Addition, 
                                () => Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur + expr.AsNumeric(),
                                                KeywordBindings[KnownConcept.Addition]).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression));
                        AddIfDefined(KnownConcept.Subtraction, 
                                () => Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur - expr.AsNumeric(),
                                                KeywordBindings[KnownConcept.Subtraction]).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression));
                        AddIfDefined(KnownConcept.Multiplication, 
                                () => Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur * expr.AsNumeric(1L),
                                                KeywordBindings[KnownConcept.Multiplication]).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression));
                        AddIfDefined(KnownConcept.Division, 
                                () => Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur / expr.AsNumeric(1L),
                                                KeywordBindings[KnownConcept.Division]).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression));
                        AddIfDefined(KnownConcept.Modulo, 
                                () => Builder.Create(
                                (state, source, stack) =>
                                        CreateMathProcessor((cur, expr) => cur % expr.AsNumeric(1L),
                                                KeywordBindings[KnownConcept.Modulo]).ExecuteAsync(state, source, stack),
                                BoundVariableOrStackExpression));
                        AddIfDefined(KnownConcept.Pop, () => Builder.Create((state, source, stack) => stack.Pop()));
                        AddIfDefined(KnownConcept.Push, () => Builder.Create((state, source, stack) => stack.Push(stack.Pop<LanguageObject>()),
                                Expression));
                        AddIfDefined(KnownConcept.DuplicateTOS, () => Builder.Create((state, source, stack) => stack.Duplicate()));
                        AddIfDefined(KnownConcept.RadixSwitch, () => Builder.Create(
                                (state, source, stack) => state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>().ScratchPad[Constants.CurrentRadix] =
                                        Convert.ToInt32(stack.Pop<LanguageObject>().AsNumeric()),
                                RegexBuilder.New().StartCaptureGroup("expr").AddCharacterClass("0-9A-Z").OneOrMore()
                                        .EndCaptureGroup().EndMatching().ToRegex()));
                        AddIfDefined(KnownConcept.PopAndPush,
                                () => Builder.Create(
                                        (state, source, stack) =>
                                                Get<PopPushCommand>().ExecuteAsync(state, source, stack), Expression));
                        AddIfDefined(KnownConcept.OutputNativeForm,
                                () => Builder.Create(
                                        (state, source, stack) => Wrapper.WriteAsync(stack.Pop<LanguageObject>().AsString()
                                                .Replace("\\n", System.Environment.NewLine)), Expression));
                        AddIfDefined(KnownConcept.OutputCharacter,
                                () => Builder.Create(
                                        (state, source, stack) =>
                                                Wrapper.WriteAsync(stack.Pop<LanguageObject>().AsCharacter()), Expression));
                        AddIfDefined(KnownConcept.Input,
                                () => Builder.Create(
                                (state, source, stack) => Get<InputCommand>().WithWrapper(Wrapper)
                                        .ExecuteAsync(state, source, stack),
                                RegexBuilder.New().StartsWith().StartCaptureGroup("var")
                                        .OneFrom(InputCommand.Options).EndCaptureGroup().EndMatching().ToRegex()));
                        AddIfDefined(KnownConcept.Untreat,
                                () => Builder.Create((state, source, stack) =>
                                state.PopExecutionEnvironment<PropertyBasedExecutionEnvironment>()));
                        AddIfDefined(KnownConcept.RotateStacks,
                                () => Builder.Create((state, source, stack) =>
                                state.RotateExecutionEnvironment<PropertyBasedExecutionEnvironment>()));
                        AddIfDefined(KnownConcept.Label,
                                () => Builder.Create(
                                (state, source, stack) => Get<LabelCommand>().ExecuteAsync(state, source, stack),
                                LabelCommand.SimpleLabel));
                        AddIfDefined(KnownConcept.Jump,
                                () => Builder.Create(
                                (state, source, stack) => Get<JumpCommand>().WithObjectFactory(Factory).ExecuteAsync(state, source, stack),
                                JumpCommand.LabelExpression));
                        AddIfDefined(KnownConcept.Treat,
                                () => Builder.Create(
                                        (state, source, stack) =>
                                                Get<TreatmentCommand>().ExecuteAsync(state, source, stack),
                                        ObjectReference));
                        AddIfDefined(KnownConcept.ConditionalExecution,
                                () => Builder.Create(
                                        (state, source, stack) =>
                                                Get<DecisionCommand>().ExecuteAsync(state, source, stack), Expression));
                        // :exp1:exp2 exp1 == exp2, push 0, exp1 < exp2 push -1, else push 1
                        AddIfDefined(KnownConcept.Comparison,
                                () => Builder.Create(
                                (state, source, stack) => Get<ComparisonCommand>().ExecuteAsync(state, source, stack),
                                Expression));
                        // Treat an object as an array, take an element and push
                        AddIfDefined(KnownConcept.RetrieveFromRAS,
                                () => Builder.Create((state, source, stack) =>
                                        CommandFactory
                                                .Get(() => new RASCommand((st, stk) => stk.Push(st.CurrentCell)),
                                                        KeywordBindings[KnownConcept.CurrentStack]).ExecuteAsync(state, source, stack) // TODO: Literal
                                , RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex()));
                        // Treat an object as an array, update the object noted at the index given with a value popped from the stack
                        AddIfDefined(KnownConcept.PlaceInRAS,
                                () => Builder.Create((state, source, stack) =>
                                        CommandFactory
                                                .Get(
                                                        () => new RASCommand((st, stk) =>
                                                                st.CurrentCell = stk.Pop<LanguageObject>()), KeywordBindings[KnownConcept.Comparison]) // TODO: Literal
                                                .ExecuteAsync(state, source, stack),
                                RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex()));
                }

                public CommandBuilder AddIfDefined(KnownConcept concept, Func<Builder> bldr)
                        => this.Fluently(() => {
                                if (KeywordBindings.ContainsKey(concept))
                                        Commands[KeywordBindings[concept]] = bldr();
                        });
                        
                protected static PropertyBasedExecutionEnvironment Environment(InterpreterState state) =>
                        state.GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();

                protected T Get<T>() where T : LanguageCommand, new() => CommandFactory.Get<T>().WithBindings(KeywordBindings) as T;

                protected LanguageCommand CreateMathProcessor(Func<long, LanguageObject, long> f, string qualifier) =>
                        CommandFactory.Get(() => new MathCommand(f), qualifier).WithBindings(KeywordBindings);

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
                                .Escape
                                .Literal(KeywordBindings[KnownConcept.Pop])
                                .Or
                                .Escape
                                .Literal(KeywordBindings[KnownConcept.Quine])
                                .Or
                                .Escape
                                .Literal(KeywordBindings[KnownConcept.CurrentStack])
                                .Or
                                .Literal("\"[^\"]*\"") // TODO
                                .EndCaptureGroup()
                                .RememberAs("expression");
                        ObjectReference = RegexBuilder.New().StartsWith().Include("objectCapture").EndMatching()
                                .ToRegex();
                        RegexBuilder.New().StartCaptureGroup("expr").AddCharacterClass("a-z").OneOrMore()
                                .EndCaptureGroup().RememberAs("label");
                        LabelCommand.SimpleLabel =
                                RegexBuilder.New().StartsWith().Include("label").EndMatching().ToRegex();
                        Expression = RegexBuilder.New().StartsWith().Include("expression").EndMatching().ToRegex();
                        BoundVariableExpression = RegexBuilder.New().StartsWith().Include("objectCapture")
                                .Include("expression").EndMatching().ToRegex();
                        BoundVariableOrStackExpression = RegexBuilder.New().StartsWith().StartCaptureGroup("var")
                                .AddCharacterClass("a-z")
                                .BoundedRepetition(2).Or.Escape.Literal(KeywordBindings[KnownConcept.Pop]).EndCaptureGroup().Include("expression")
                                .EndMatching().ToRegex();
                        JumpCommand.LabelExpression = RegexBuilder.New()
                                .StartsWith().StartCaptureGroup("var").Include("objectReference").Or.Escape.Literal(KeywordBindings[KnownConcept.Pop]).Or
                                .Escape.Literal(KeywordBindings[KnownConcept.CurrentStack]).Or.Escape.Literal($"{KeywordBindings[KnownConcept.Period]}")
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
                        return LanguageCommand.Gather(state, res.Key, res.Builder, KeywordBindings);
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