using System;
using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public class NullObject : BaseObject {
                private static readonly Lazy<NullObject> SoleInstance = new Lazy<NullObject>(() => new NullObject());

                private NullObject() { }

                public static NullObject Instance => SoleInstance.Value;

                protected override Task InterpretAsync(InterpreterState state) => Task.CompletedTask;
        }
}