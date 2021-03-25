using System;
using System.Collections.Generic;

namespace General.Language {
        public static class CommandFactory {
                private static readonly Dictionary<Tuple<Type, string>, object> Cache =
                        new Dictionary<Tuple<Type, string>, object>();

                public static T Get<T>(Func<T> creator = null, string qualifier = null) where T : LanguageCommand, new() {
                        var key = Tuple.Create(typeof(T), qualifier ?? string.Empty);
                        if (!Cache.ContainsKey(key))
                                Cache[key] = (creator ?? (Func<T>) (() => new T()))();
                        return Cache[key] as T;
                }
        }
}