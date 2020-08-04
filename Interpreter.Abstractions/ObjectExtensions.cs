using System;

namespace Interpreter.Abstractions {
        public static class ObjectExtensions {
                public static T Fluently<T>(this T obj, Action action) {
                        action();
                        return obj;
                }

                public static bool IfTrue(this bool val, Action action) {
                        if (val) action();

                        return val;
                }

                public static bool IfFalse(this bool val, Action action) {
                        if (!val) action();

                        return val;
                }
        }
}