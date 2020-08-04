//============================================================================================================================================================================
// Copyright (c) 2011-2013 Tony Beveridge
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//============================================================================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interpreter.Abstractions;
using FEE = Interpreter.Abstractions.PropertyBasedExecutionEnvironment;

namespace FalseInterpreter {
        public class NumberBuilder : BasicNumberBuilder<SimpleSourceCode, PropertyBasedExecutionEnvironment> { }

        public class VariableBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private string Value { get; set; }

                public override bool Applicable(InterpreterState state) =>
                        char.IsLower(state.Source().Current().First());

                public override BaseObject Gather(InterpreterState state) {
                        Value = state.Source().Current();
                        ExecutionSupport.Emit(() => string.Format("Variable pushed: {0}", Value));
                        state.Source().Advance();
                        return new FalseVariable(Value);
                }
        }

        public class WhiteSpaceSkipper : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                public override bool Applicable(InterpreterState state) =>
                        char.IsWhiteSpace(state.Source().Current().First());

                public override BaseObject Gather(InterpreterState state) {
                        while (state.Source().More() && Applicable(state))
                                state.Source().Advance();
                        return NullObject.Instance;
                }
        }

        public class SequenceBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private static readonly Dictionary<string, Tuple<string, bool>> SequenceDictionary =
                        new Dictionary<string, Tuple<string, bool>> {
                                {"{", new Tuple<string, bool>("}", true)},
                                {"\"", new Tuple<string, bool>("\"", false)}
                        };

                public override bool Applicable(InterpreterState state) =>
                        SequenceDictionary.ContainsKey(state.Source().Current());

                public override BaseObject Gather(InterpreterState state) {
                        var cur = SequenceDictionary[state.Source().Current()];
                        var bldr = new StringBuilder();
                        state.Source().Advance();
                        while (state.Source().More() && state.Source().Current() != cur.Item1) {
                                bldr.Append(state.Source().Current());
                                state.Source().Advance();
                        }

                        state.Source().Advance();
                        return new FalseString(bldr.ToString(), cur.Item2);
                }
        }

        public class CommandBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private static readonly
                        Dictionary<string, Action<InterpreterState, SourceCode, PropertyBasedExecutionEnvironment>>
                        Commands =
                                new Dictionary<string, Action<InterpreterState, SourceCode,
                                        PropertyBasedExecutionEnvironment>>();

                static CommandBuilder() {
                        Commands["$"] = (state, source, stack) => stack.Duplicate();
                        Commands["("] = (state, source, stack) => stack.Pick(stack.Pop<CanonicalNumber>().Value);
                        Commands["+"] = CommonCommands.BinaryAddition<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["-"] =
                                CommonCommands.BinarySubtraction<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["*"] = CommonCommands
                                .BinaryMultiplication<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["/"] = CommonCommands.Division<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["<"] = (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() > stack.Pop<CanonicalNumber>());
                        Commands[">"] = (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() < stack.Pop<CanonicalNumber>());
                        Commands["="] = CommonCommands.Equality<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["&"] = CommonCommands.LogicalAnd<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["|"] = CommonCommands.LogicalOr<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["_"] = (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() * CanonicalBoolean.True);
                        Commands["~"] = (state, source, stack) => stack.Push(stack.Pop<CanonicalNumber>().Negate());
                        Commands[":"] = (state, source, stack) =>
                                stack[stack.Pop<FalseVariable>().Key] = stack.Pop<BaseObject>();
                        Commands[";"] = (state, source, stack) => stack.Push(stack[stack.Pop<FalseVariable>().Key]);
                        Commands["!"] = (state, source, stack) => stack.Pop<FalseLambda>().Execute(state);
                        Commands["`"] = (state, source, stack) => stack.Pop<BaseObject>();
                        Commands["%"] = (state, source, stack) => stack.Pop<BaseObject>();
                        Commands["\\"] = (state, source, stack) => stack.Swap();
                        Commands["@"] = (state, source, stack) => stack.Rotate();
                        Commands["."] = CommonCommands
                                .OutputValueFromStack<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["^"] =
                                CommonCommands.ReadValueAndPush<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands[","] = CommonCommands
                                .OutputCharacterFromStack<SourceCode, PropertyBasedExecutionEnvironment>();
                        Commands["?"] = (state, source, stack) => ExecuteConditional(state);
                        Commands["#"] = (state, source, stack) => ExecuteLoop(state);
                }

                private static void ExecuteConditional(InterpreterState state) {
                        var fun = state.Stack<FEE>().Pop<FalseLambda>();
                        if (state.Stack<FEE>().Pop<CanonicalNumber>())
                                fun.Execute(state);
                }

                private static void ExecuteLoop(InterpreterState state) {
                        var fun = state.Stack<FEE>().Pop<FalseLambda>();
                        var test = state.Stack<FEE>().Pop<FalseLambda>();
                        test.Execute(state);
                        while (state.Stack<FEE>().Pop<CanonicalNumber>()) {
                                fun.Execute(state);
                                test.Execute(state);
                        }
                }

                public override bool Applicable(InterpreterState state) =>
                        Commands.ContainsKey(state.Source().Current());

                public override BaseObject Gather(InterpreterState state) {
                        var key = state.Source().Current();
                        state.Source().Advance();
                        ExecutionSupport.Emit(() => string.Format("Command created: {0}, Source Position {1}", key,
                                state.Source().SourcePosition));
                        return new ActionCommand<PropertyBasedExecutionEnvironment>(Commands[key], key);
                }
        }

        public class LambdaBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private const string LambdaStart = "[";
                private const string LambdaEnd = "]";

                public override bool Applicable(InterpreterState state) => state.Source().Current() == LambdaStart;

                public override BaseObject Gather(InterpreterState state) {
                        var func = new FalseLambda();
                        state.Source().Advance();
                        while (state.Source().Current() != LambdaEnd && state.Source().More())
                                func.AddCommand(Interpreter.Gather());
                        state.Source().Advance();
                        return func;
                }
        }

        public class CharBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                public override bool Applicable(InterpreterState state) => state.Source().Current() == "'";

                public override BaseObject Gather(InterpreterState state) {
                        state.Source().Advance();
                        var num = new CanonicalNumber(Convert.ToInt32(state.Source().Current().First()));
                        state.Source().Advance();
                        return num;
                }
        }

        // TODO: Combine with CommandBuilder
        public class
                ExtendedCommandBuilder : TrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment> {
                private const string CommandStartAndEnd = ")";

                private static readonly
                        Dictionary<string, Action<InterpreterState, SourceCode, PropertyBasedExecutionEnvironment,
                                string>> Commands =
                                new Dictionary<string, Action<InterpreterState, SourceCode,
                                        PropertyBasedExecutionEnvironment, string>>();

                static ExtendedCommandBuilder() {
                        Commands["st"] = (state, source, stack, ctx) => StackingTimer.Start(ctx);
                        Commands["ct"] = (state, source, stack, ctx) => StackingTimer.Stop();
                        Commands["rs"] = (state, source, stack, ctx) => Statistics.Reset();
                        Commands["ds"] = (state, source, stack, ctx) => Statistics.Dump();
                }

                public override bool Applicable(InterpreterState state) =>
                        state.Source().Current() == CommandStartAndEnd;

                public override BaseObject Gather(InterpreterState state) {
                        state.Source().Advance();
                        var key = string.Concat(state.Source().Current(), state.Source().AdvanceAndReturn());
                        state.Source().Advance();
                        ExecutionSupport.Assert(Commands.ContainsKey(key),
                                string.Concat("Extended command \"", key, "\" unknown"));
                        var ctx = string.Empty;
                        while (state.Source().More() && !Applicable(state)) {
                                ctx = ctx + state.Source().Current();
                                state.Source().Advance();
                        }

                        ExecutionSupport.Emit(() => string.Format("Extended command created: {0}", key));
                        state.Source().Advance();
                        return new ExtendedFalseCommand(Commands[key], key, ctx);
                }
        }
}