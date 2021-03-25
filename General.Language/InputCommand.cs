using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Interpreter.Abstractions;

namespace General.Language {
        public class InputCommand : LanguageCommand {

                [ThreadStatic] internal static IOWrapper InteractionWrapper;

                public static readonly IEnumerable<string> Options = new[] { "l", "c" };

                public InputCommand WithWrapper(IOWrapper w) {
                        InteractionWrapper = w;
                        return this;
                }

                public override async Task ExecuteAsync(InterpreterState state, SourceCode code, BaseInterpreterStack stack) {
                        var style = stack.Pop<LanguageObject>().AsString();
                        ExecutionSupport.Assert(Options.Contains(style),
                                string.Concat("Invalid argument for ',' - ", style));
                        stack.Push(style == "l"
                                ? new LanguageObject(state.KnownRadix(), await InteractionWrapper.ReadStringAsync("0"))
                                : new LanguageObject(state.KnownRadix(), Convert.ToChar(await InteractionWrapper.ReadCharacterAsync())));
                }
        }
}