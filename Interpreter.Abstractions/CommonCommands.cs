using System;
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

                public static Action<InterpreterState, TSourceType, TExeType> OutputValueFromStack<TSourceType,
                        TExeType>()
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        (state, source, stack) => Console.Write(stack.Pop<CanonicalNumber>().Value);

                public static Action<InterpreterState, TSourceType, TExeType> OutputCharacterFromStack<TSourceType,
                        TExeType>(Func<IOWrapper> wrapper = null)
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                Console.Write(new string(Convert.ToChar(stack.Pop<CanonicalNumber>().Value), 1));

                public static Action<InterpreterState, TSourceType, TExeType> ReadValueAndPush<TSourceType, TExeType>(
                        bool readLine = false)
                        where TSourceType : SourceCode
                        where TExeType : BaseInterpreterStack =>
                        (state, source, stack) =>
                                stack.Push(readLine
                                        ? new CanonicalNumber(Console.ReadLine())
                                        : new CanonicalNumber(Console.Read()));
        }
}