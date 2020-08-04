using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Abstractions {
        public class RandomAccessStack<TCellType> : BaseInterpreterStack where TCellType : BaseObject, new() {
                private const int InitialCapacity = 512;
                private const int Resize = 1024;

                public RandomAccessStack()
                        : this(InitialCapacity) { }

                public RandomAccessStack(int initialCapacity = 0, int maxCapacity = Resize) {
                        State.AddRange(GeneratePadding(initialCapacity));
                        MaximumSize = maxCapacity <= 0 ? Resize : maxCapacity;
                }

                public int Pointer { get; set; }

                public TCellType CurrentCell {
                        get => State[Pointer] as TCellType;
                        set => State[Pointer] = value;
                }

                public int MaximumSize { get; set; }

                public void Advance() {
                        if (++Pointer >= State.Count) State.AddRange(GeneratePadding());
                }

                public void Retreat() => Pointer = --Pointer < 0 ? 0 : Pointer;

                public void Set(int index) {
                        ExecutionSupport.Assert(index >= 0,
                                string.Concat("Illegal random access stack index: ", index));
                        if (index >= State.Count) State.AddRange(GeneratePadding(index - State.Count + 1));

                        Pointer = index;
                }

                public override string ToString() =>
                        string.Concat(base.ToString(), ", Pointer == ", Pointer, ", Count == ", State.Count,
                                ", Current cell == ", CurrentCell);

                private IEnumerable<BaseObject> GeneratePadding(int paddingSize = Resize) {
                        ExecutionSupport.Assert(State.Count <= MaximumSize,
                                string.Concat("Maximum size exceeded: ", State.Count, " > ", MaximumSize));
                        return Enumerable.Repeat(new TCellType(), paddingSize);
                }
        }
}