using System;
using System.Threading.Tasks;

namespace Interpreter.Abstractions {
        public abstract class BaseObject {
                public async Task ApplyAsync(InterpreterState state) {
                        await InterpretAsync(state);
                        state.BaseExecutionEnvironment.Dump("Post-execution: " + ToString() + Environment.NewLine);
                }

                public virtual BaseObject Accept(string src) => this;

                public TObject As<TObject>() where TObject : BaseObject => (TObject) this;

                protected virtual Task InterpretAsync(InterpreterState state) {
                        state.BaseExecutionEnvironment.Push(this);
                        return Task.CompletedTask;
                }

                public virtual object Clone() => Activator.CreateInstance(GetType()) as BaseObject;
        }
}