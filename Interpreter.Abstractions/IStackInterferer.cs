namespace Interpreter.Abstractions {
        public interface IStackInterferer {
                void PreStackObjectAccess(BaseInterpreterStack stack, int objectsRequested);
        }
}