﻿//============================================================================================================================================================================
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

using Befunge_93.Language;
using Interpreter.Abstractions;

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

namespace Befunge_93 {
        public class Program {
                private const string Message =
                        "Befunge-93 interpreter v 0.0: (c) 2011-2013 Tony Beveridge, released under the MIT license";

                private static readonly MutableTuple<int> StandardTorusSize =
                        new MutableTuple<int>(ConfigurationSupport.ConfigurationFor("torusColumns", 80),
                                ConfigurationSupport.ConfigurationFor("torusRows", 25));

                private static void Main(string[] args) =>
                        new CommandLineExecutor<SourceCodeTorus, BaseInterpreterStack>().Execute(
                                typeof(CommandBuilder).Assembly, Message, args,
                                interp => {
                                        interp.State.GetSource<SourceCodeTorus>().Size = StandardTorusSize;
                                        var env = interp.State.GetExecutionEnvironment<BaseInterpreterStack>();
                                        env.Interferer = new StackInterferer();
                                        env.ScratchPad[CommandBuilder.IOWrapperKey] = new ConsoleIOWrapper();
                                }
                        );
        }
}