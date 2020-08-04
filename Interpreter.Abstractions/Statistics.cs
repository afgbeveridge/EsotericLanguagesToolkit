using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Abstractions {
        public static class Statistics {
                private static Dictionary<string, double> CapturedStatistics = new Dictionary<string, double>();

                public static void Increment(string key, int addend = 1) {
                        if (!CapturedStatistics.ContainsKey(key)) CapturedStatistics[key] = 0;

                        CapturedStatistics[key] += addend;
                }

                public static void Reset() => CapturedStatistics = new Dictionary<string, double>();

                public static void Dump() {
                        Console.WriteLine(string.Concat("Statistics", Environment.NewLine));
                        CapturedStatistics.ToList()
                                .ForEach(kvp => Console.WriteLine(string.Concat(kvp.Key, " == ", kvp.Value)));
                }
        }
}