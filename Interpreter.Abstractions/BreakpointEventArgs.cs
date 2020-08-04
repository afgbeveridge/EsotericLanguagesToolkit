using System;

namespace Interpreter.Abstractions {
        public class BreakpointEventArgs : EventArgs {
                public InterpreterState State { get; set; }
        }
}