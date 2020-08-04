using System;

namespace Interpreter.Abstractions {
        public abstract class BaseObject {
                public void Apply(InterpreterState state) {
                        Interpret(state);
                        state.BaseExecutionEnvironment.Dump("Post-execution: " + ToString() + Environment.NewLine);
                }

                public virtual BaseObject Accept(string src) => this;

                public TObject As<TObject>() where TObject : BaseObject => (TObject) this;

                protected virtual void Interpret(InterpreterState state) => state.BaseExecutionEnvironment.Push(this);

                public virtual object Clone() => Activator.CreateInstance(GetType()) as BaseObject;
        }
}