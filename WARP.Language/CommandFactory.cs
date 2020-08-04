using System;
using System.Collections.Generic;

namespace WARP.Language {
        internal static class CommandFactory {
                private static readonly Dictionary<Tuple<Type, string>, object> Cache =
                        new Dictionary<Tuple<Type, string>, object>();

                internal static T Get<T>(Func<T> creator = null, string qualifier = null) where T : WARPCommand, new() {
                        var key = Tuple.Create(typeof(T), qualifier ?? string.Empty);
                        if (!Cache.ContainsKey(key))
                                Cache[key] = (creator ?? (Func<T>) (() => new T()))();
                        return Cache[key] as T;
                }
        }
}