using System;

namespace Interpreter.Abstractions {
        internal class OptionBehaviour<TSourceType, TExeType>
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                internal int ArgumentCount { get; set; }

                internal string[] Arguments { get; set; }

                internal string Help { get; set; }

                internal Action<CommandLineExecutor<TSourceType, TExeType>, Interpreter<TSourceType, TExeType>, string[]
                > Effect { get; set; }

                internal bool Active { get; set; }

                internal bool Transient { get; set; }

                public override string ToString() =>
                        Help + Environment.NewLine + ", argument count == " + ArgumentCount;
        }
}