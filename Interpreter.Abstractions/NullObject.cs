using System;

namespace Interpreter.Abstractions {
        public class NullObject : BaseObject {
                private static readonly Lazy<NullObject> SoleInstance = new Lazy<NullObject>(() => new NullObject());

                private NullObject() { }

                public static NullObject Instance => SoleInstance.Value;

                protected override void Interpret(InterpreterState state) { }
        }
}