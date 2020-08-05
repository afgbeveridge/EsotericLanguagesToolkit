using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace WARP.Language {
        internal class WARPInputCommand : WARPCommand {
                [ThreadStatic] internal static IOWrapper InteractionWrapper;

                internal static readonly IEnumerable<string> Options = new[] { "l", "c" };

                internal WARPInputCommand WithWrapper(IOWrapper w) {
                        InteractionWrapper = w;
                        return this;
                }

                internal override async Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var style = stack.Pop<WARPObject>().AsString();
                        ExecutionSupport.Assert(Options.Contains(style),
                                string.Concat("Invalid argument for ',' - ", style));
                        stack.Push(style == "l"
                                ? new WARPObject(await InteractionWrapper.ReadStringAsync("0"))
                                : new WARPObject(Convert.ToChar(await InteractionWrapper.ReadCharacterAsync())));
                }
        }
}