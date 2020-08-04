using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public class PropertyBasedExecutionEnvironment : BaseInterpreterStack {
                private Dictionary<string, BaseObject> Variables = new Dictionary<string, BaseObject>();

                public Func<PropertyBasedExecutionEnvironment, BaseObject> OnUnknownKey { get; set; }

                public BaseObject this[string key] {
                        get {
                                ExecutionSupport.Emit(() => string.Format("Getting variable: {0}", key));
                                return Variables.ContainsKey(key) ? Variables[key] :
                                        OnUnknownKey != null ? Variables[key] = OnUnknownKey(this) : null;
                        }
                        set {
                                ExecutionSupport.Emit(() => string.Format("Set variable: {0} == {1}", key, value));
                                Variables[key] = value;
                        }
                }

                public void Reset() {
                        ScratchPad = new Dictionary<string, object>();
                        Variables = new Dictionary<string, BaseObject>();
                }

                public PropertyBasedExecutionEnvironment Clone() =>
                        new PropertyBasedExecutionEnvironment {
                                OnUnknownKey = OnUnknownKey, ScratchPad = ScratchPad, Variables = Variables
                        };
        }
}