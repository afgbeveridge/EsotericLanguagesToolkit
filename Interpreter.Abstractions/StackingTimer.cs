using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public static class StackingTimer {
                static StackingTimer() => Times = new Stack<Tuple<string, DateTime>>();

                private static Stack<Tuple<string, DateTime>> Times { get; }

                public static void Start(string timerName = null) =>
                        Times.Push(new Tuple<string, DateTime>(timerName ?? "Unnamed task", DateTime.Now));

                public static TimeSpan Stop(bool display = true) {
                        var tos = Times.Pop();
                        var span = DateTime.Now - tos.Item2;
                        if (display) Console.WriteLine(tos.Item1 + " completes; execution time: " + span.ToString("c"));

                        return span;
                }
        }
}