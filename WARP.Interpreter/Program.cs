//============================================================================================================================================================================
// Copyright (c) 2013 Tony Beveridge
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

using General.Language;
using Interpreter.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;
using WARP.Language;

namespace WARP {
        public class Program {
                private const string Message =
                        "WARP interpreter v 2.0: (c) 2013 Tony Beveridge, released under the MIT license";

                private static void Main(string[] args) =>
                        new CommandLineExecutor<SimpleSourceCode, PropertyBasedExecutionEnvironment>()
                                .Execute(new[] { typeof(Language.CommandBuilder).Assembly, typeof(WhiteSpaceSkipper).Assembly }, Message, args,
                                        interp => {
                                                Bind(interp.KnownInterpreters);
                                                var env = interp.State
                                                        .GetExecutionEnvironment<PropertyBasedExecutionEnvironment>();
                                                env.ScratchPad[General.Language.Constants.RASName] =
                                                        new RandomAccessStack<LanguageObject> {
                                                                MaximumSize =
                                                                        ConfigurationSupport.ConfigurationFor<int>(
                                                                                "rasSize")
                                                        };
                                                env.ScratchPad[General.Language.Constants.CurrentBase] = new ConsoleIOWrapper();
                                                env.ScratchPad[General.Language.Constants.CurrentRadix] = FlexibleNumeralSystem.StandardRadix;
                                                env.OnUnknownKey = e => new LanguageObject(e.ScratchPadAs<int>(General.Language.Constants.CurrentRadix));
                                        }
                                );
                private static void Bind(ReadOnlyCollection<ITrivialInterpreterBase<SimpleSourceCode, PropertyBasedExecutionEnvironment>> handlers) {
                        handlers
                                .Select(h => h as General.Language.CommandBuilder)
                                .Where(h => h != null)
                                .ToList()
                                .ForEach(h => h.WithBindings(Constants.DefaultKeywordBindings).Initialize());
                }
        }
}