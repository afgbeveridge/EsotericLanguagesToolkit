using System.Linq;
using Interpreter.Abstractions;

namespace Befunge_93.Language {
        public class StackInterferer : IStackInterferer {
                public void PreStackObjectAccess(BaseInterpreterStack stack, int objectsRequested) {
                        if (stack.Size < objectsRequested)
                                Enumerable.Repeat(new CanonicalNumber(), objectsRequested - stack.Size).ToList()
                                        .ForEach(obj => stack.Push(obj));
                }
        }
}