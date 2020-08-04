using System;

namespace Interpreter.Abstractions {
        public class InterpreterEventArgs<TSourceType, TExeType> : EventArgs
                where TSourceType : SourceCode, new()
                where TExeType : BaseInterpreterStack, new() {
                public Interpreter<TSourceType, TExeType> ActiveInterpreter { get; set; }
                public bool ErrorState { get; set; }
        }
}