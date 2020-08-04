namespace Interpreter.Abstractions {
        internal class NullInterferer : IStackInterferer {
                public void PreStackObjectAccess(BaseInterpreterStack stack, int objectsRequested) { }
        }
}