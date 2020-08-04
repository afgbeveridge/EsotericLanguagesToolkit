using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Abstractions {
        public class BaseInterpreterStack {
                public BaseInterpreterStack() {
                        State = new List<BaseObject>();
                        Interferer = new NullInterferer();
                        ScratchPad = new Dictionary<string, object>();
                }

                public IStackInterferer Interferer { get; set; }

                public int Size => State.Count;

                public Dictionary<string, object> ScratchPad { get; set; }

                protected List<BaseObject> State { get; set; }

                public BaseInterpreterStack Duplicate() {
                        Interferer.PreStackObjectAccess(this, 1);
                        ExecutionSupport.Assert(State.Any(), "Attempt to duplicate with an empty state");
                        State.Insert(0, State.First().Clone() as BaseObject);
                        return this;
                }

                public BaseInterpreterStack Push(BaseObject obj) {
                        Statistics.Increment("Push");
                        ExecutionSupport.AssertNotNull(obj, "Attempt to push null object");
                        State.Insert(0, obj);
                        ExecutionSupport.Emit(() =>
                                string.Format("Object pushed onto stack({0}): {1}", State.Count(), obj));
                        return this;
                }

                public BaseObject Pop() => PopMultiple().First();

                public BaseInterpreterStack Pick(int idx) {
                        ExecutionSupport.Assert(idx < State.Count,
                                string.Format("Pick {0} invalid when only {1} elements", idx, State.Count));
                        Push(State[idx].Clone() as BaseObject);
                        return this;
                }

                private List<BaseObject> PopMultiple(int cnt = 1) {
                        Interferer.PreStackObjectAccess(this, cnt);
                        ExecutionSupport.Assert(State.Any(), "Attempt to pop an empty stack");
                        var objs = State.GetRange(0, cnt);
                        State.RemoveRange(0, cnt);
                        ExecutionSupport.Emit(() =>
                                string.Format("{0} object(s) popped from stack, size now == {1}", cnt, State.Count()));
                        Statistics.Increment("Pop", cnt);
                        return objs;
                }

                public T Pop<T>() where T : BaseObject =>
                        ExecutionSupport.AssertNotNull(Pop() as T,
                                val => string.Format("Object is not a {0}", typeof(T).Name));

                public void Swap() {
                        var objs = PopMultiple(2);
                        Push(objs.First()).Push(objs[1]);
                }

                public void Rotate() {
                        var objs = PopMultiple(3);
                        Push(objs[1]).Push(objs.First()).Push(objs[2]);
                }

                public virtual void Dump(string wrapper) =>
                        ExecutionSupport.Emit(() =>
                                wrapper + Environment.NewLine + ToString() + Environment.NewLine + "................." +
                                Environment.NewLine);

                public override string ToString() => ToString(StackDumpDirective.Top);

                public bool HasScratchPadEntry(string key) => ScratchPad.ContainsKey(key);

                public TObj ScratchPadAs<TObj>(string key) => (TObj) ScratchPad[key];

                public string ToString(StackDumpDirective depth) =>
                        "Stack size == " + State.Count + ", tos => " + Environment.NewLine +
                        (!State.Any() ? "<Empty>" :
                                depth == StackDumpDirective.Top ? State.First().ToString() :
                                string.Join("===>", State.Select(s => s + Environment.NewLine).ToArray()));
        }
}