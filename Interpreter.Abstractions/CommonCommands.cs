using System;
using System.Threading.Tasks;
using Common;

namespace Interpreter.Abstractions {
        public static class CommonCommands {
                private static IOWrapper DefaultWrapper = new ConsoleIOWrapper();

                public static Action<InterpreterState, TSourceType, TExeType> BinaryAddition<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() + stack.Pop<CanonicalNumber>());

                public static Action<InterpreterState, TSourceType, TExeType> BinarySubtraction<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>().Invert() + stack.Pop<CanonicalNumber>());

                public static Action<InterpreterState, TSourceType, TExeType>
                        BinaryMultiplication<TSourceType, TExeType>() where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() * stack.Pop<CanonicalNumber>());

                public static Action<InterpreterState, TSourceType, TExeType> Division<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) => {
                                var divisor = stack.Pop<CanonicalNumber>();
                                stack.Push(stack.Pop<CanonicalNumber>() / divisor);
                        };

                public static Action<InterpreterState, TSourceType, TExeType> LogicalAnd<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() & stack.Pop<CanonicalNumber>());

                public static Action<InterpreterState, TSourceType, TExeType> LogicalOr<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() | stack.Pop<CanonicalNumber>());

                public static Action<InterpreterState, TSourceType, TExeType> Equality<TSourceType, TExeType>()
                        where TSourceType : SourceCode where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(stack.Pop<CanonicalNumber>() == stack.Pop<CanonicalNumber>());

                public static Func<InterpreterState, TSourceType, TExeType, Task> OutputValueFromStack<TSourceType,
                        TExeType>(Func<IOWrapper> wrapper)
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        async (state, source, stack) => await wrapper().WriteAsync(stack.Pop<CanonicalNumber>().Value.ToString()); // TODO: Check is correct

                public static Func<InterpreterState, TSourceType, TExeType, Task> OutputCharacterFromStack<TSourceType,
                        TExeType>(Func<IOWrapper> wrapper)
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        async (state, source, stack) =>
                                await wrapper().WriteAsync(new string(Convert.ToChar(stack.Pop<CanonicalNumber>().Value), 1));

                public static Func<InterpreterState, TSourceType, TExeType, Task> ReadValueAndPush<TSourceType, TExeType>(Func<IOWrapper> wrapper, bool readLine = false)
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        async (state, source, stack) =>
                                stack.Push(readLine
                                        ? new CanonicalNumber(await wrapper().ReadStringAsync())
                                        : new CanonicalNumber((int) await wrapper().ReadCharacterAsync()));
        }
}