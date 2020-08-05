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
using Common;
using Interpreter.Abstractions;

namespace Deadfish.Language {
        public class CommandBuilder : TrivialInterpreterBase<SimpleSourceCode, RandomAccessStack<CanonicalNumber>> {
                internal const string IOWrapperKey = "__";

                private readonly
                        Dictionary<char, Action<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>>>
                        Commands =
                                new Dictionary<char, Action<InterpreterState, SimpleSourceCode,
                                        RandomAccessStack<CanonicalNumber>>>();

                public CommandBuilder() => Initialize();

                public IOWrapper Wrapper { get; set; }

                private static void ResetIfNecessary(RandomAccessStack<CanonicalNumber> stack, Action a) {
                        var cur = stack.CurrentCell.Value;
                        if (cur == 256 || cur < 0) stack.CurrentCell.Value = 0;
                        a();
                }

                internal void Initialize() {
                        Commands['i'] = (state, source, stack) =>
                                ResetIfNecessary(stack, () => stack.CurrentCell.Value += 1);
                        Commands['d'] = (state, source, stack) =>
                                ResetIfNecessary(stack, () => stack.CurrentCell.Value -= 1);
                        Commands['s'] = (state, source, stack) =>
                                ResetIfNecessary(stack, () => stack.CurrentCell.Value *= stack.CurrentCell.Value);
                        Commands['o'] = (state, source, stack) => ResetIfNecessary(stack,
                                () => Wrapper.WriteAsync(stack.CurrentCell.Value.ToString()));
                }

                public override bool Applicable(InterpreterState state) =>
                        Applicable(state.BaseSourceCode.CurrentCharacter());

                public bool Applicable(char current) => Commands.ContainsKey(current);

                public override BaseObject Gather(InterpreterState state) {
                        Wrapper = state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                .ScratchPadAs<IOWrapper>(IOWrapperKey);
                        var key = state.BaseSourceCode.CurrentCharacter();
                        state.BaseSourceCode.Advance();
                        ExecutionSupport.Emit(() =>
                                $"Command created: {key}, source position: {state.GetSource<SourceCode>().SourcePosition}");
                        return new DeadfishCommand(Commands[key], key.ToString());
                }
        }
}

