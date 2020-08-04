using System;
using System.IO;

namespace Interpreter.Abstractions {
        public static class ExecutionSupport {
                private static StreamWriter Writer;

                public static bool DebugMode { get; set; }

                public static void Prepare(string name) {
                        if (DebugMode) {
                                UnPrepare();
                                Writer = new StreamWriter(File.Open(string.Concat("debug.", name, ".txt"),
                                        FileMode.Append));
                        }
                }

                public static void UnPrepare() {
                        if (Writer != null) {
                                Writer.Close();
                                Writer = null;
                        }
                }

                public static void Assert(bool condition, string msg, Action<string> preException = null) {
                        if (!condition) {
                                if (preException != null) preException(msg);

                                throw new ApplicationException(msg);
                        }
                }

                public static T AssertNotNull<T>(T obj, string msg, Action<string> preException = null)
                        where T : class =>
                        AssertNotNull(obj, val => msg, preException);

                public static T AssertNotNull<T>(T obj, Func<T, string> f, Action<string> preException = null)
                        where T : class {
                        Assert(obj != null, f(obj), preException);
                        return obj;
                }

                public static void Emit(Func<string> msg) {
                        if (DebugMode && Writer != null) Writer.WriteLine(msg());
                }

                public static void EnableStatistics() { }
        }
}