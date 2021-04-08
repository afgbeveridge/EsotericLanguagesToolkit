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
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace BrainFuck.Language {
        public class CommandBuilder : TrivialInterpreterBase<SimpleSourceCode, RandomAccessStack<CanonicalNumber>> {
                private const char StartConditional = '[';
                private const char EndConditional = ']';

                private readonly
                        Dictionary<char, Func<InterpreterState, SimpleSourceCode, RandomAccessStack<CanonicalNumber>, Task>>
                        Commands =
                                new Dictionary<char, Func<InterpreterState, SimpleSourceCode,
                                        RandomAccessStack<CanonicalNumber>, Task>>();

                public CommandBuilder() => Initialize();

                public IOWrapper Wrapper { get; set; }

                private Task WrapAsAsync(Action a) {
                        a();
                        return Task.CompletedTask;
                }

                internal void Initialize() {
                        Commands['>'] = async (state, source, stack) => await WrapAsAsync(() => stack.Advance());
                        Commands['<'] = async (state, source, stack) => await WrapAsAsync(() => stack.Retreat());
                        Commands['+'] = async (state, source, stack) =>
                                await WrapAsAsync(() => stack.CurrentCell += new CanonicalNumber(1));
                        Commands['-'] = async (state, source, stack) =>
                                await WrapAsAsync(() => stack.CurrentCell += new CanonicalNumber(-1));
                        Commands['.'] = async (state, source, stack) =>
                                await Wrapper.WriteAsync(new string(Convert.ToChar(stack.CurrentCell.Value), 1));
                        Commands[','] = async (state, source, stack) => stack.CurrentCell.Value = await Wrapper.ReadCharacterAsync();
                        Commands[StartConditional] = async (state, source, stack) =>
                                await WrapAsAsync(() => {
                                        if (stack.CurrentCell.Value > 0) {
                                                source.Advance();
                                        }
                                        else {
                                                source.Seek(EndConditional, SeekDirection.Forward, StartConditional);
                                                source.Advance();
                                        }
                                });
                        Commands[EndConditional] = async (state, source, stack) =>
                                await WrapAsAsync(() => source.Seek(StartConditional, SeekDirection.Backward, EndConditional));
                }

                public override bool Applicable(InterpreterState state) =>
                        Applicable(state.BaseSourceCode.CurrentCharacter());

                public bool Applicable(char current) => Commands.ContainsKey(current);

                public override BaseObject Gather(InterpreterState state) {
                        state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                .ScratchPad[Constants.Builder] = this;
                        Wrapper = state.GetExecutionEnvironment<RandomAccessStack<CanonicalNumber>>()
                                .ScratchPadAs<IOWrapper>(Constants.CurrentBase);
                        var key = state.BaseSourceCode.CurrentCharacter();
                        if (key != StartConditional && key != EndConditional)
                                state.BaseSourceCode.Advance();
                        ExecutionSupport.Emit(() => string.Format("Command created: {0}, source position: {1}", key,
                                state.GetSource<SourceCode>().SourcePosition));
                        return new BrainFuckCommand(Commands[key], key.ToString());
                }
        }
}