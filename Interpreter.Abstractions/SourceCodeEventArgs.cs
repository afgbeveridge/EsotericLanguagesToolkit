using System;

namespace Interpreter.Abstractions {
        public class SourceCodeEventArgs<TSourceType> : EventArgs where TSourceType : SourceCode {
                //, new() {
                public TSourceType Source { get; set; }
        }
}