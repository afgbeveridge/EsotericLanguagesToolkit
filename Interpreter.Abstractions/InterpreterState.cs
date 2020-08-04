using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public class InterpreterState {
                private object SourceObject;
                private Stack<object> Stacks;

                public BaseInterpreterStack BaseExecutionEnvironment => GetExecutionEnvironment<BaseInterpreterStack>();

                public SourceCode BaseSourceCode => GetSource<SourceCode>();

                public InterpreterState Establish<TSourceType, TExeType>()
                        where TSourceType : SourceCode, new()
                        where TExeType : BaseInterpreterStack, new() {
                        SetSource(new TSourceType());
                        Stacks = new Stack<object>();
                        AddExecutionEnvironment<TExeType>();
                        return this;
                }

                public TSourceType GetSource<TSourceType>() where TSourceType : SourceCode =>
                        (TSourceType) SourceObject;

                internal void SetSource<TSourceType>(TSourceType src) where TSourceType : SourceCode =>
                        SourceObject = src;

                public TExeType GetExecutionEnvironment<TExeType>() where TExeType : BaseInterpreterStack =>
                        (TExeType) GetStacks().Peek();

                public void AddExecutionEnvironment<TExeType>(TExeType exeObject = default)
                        where TExeType : BaseInterpreterStack, new() =>
                        GetStacks().Push(exeObject ?? new TExeType());

                public void PopExecutionEnvironment<TExeType>() where TExeType : BaseInterpreterStack =>
                        GetStacks().Pop();

                public void RotateExecutionEnvironment<TExeType>() where TExeType : BaseInterpreterStack, new() {
                        if (GetStacks().Count > 1) {
                                var tos = GetStacks().Pop();
                                var next = GetStacks().Pop();
                                AddExecutionEnvironment((TExeType) tos);
                                AddExecutionEnvironment((TExeType) next);
                        }
                }

                private Stack<object> GetStacks() => Stacks;
        }
}