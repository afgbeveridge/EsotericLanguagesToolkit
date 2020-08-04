using System;
using System.Collections.Generic;
using System.Configuration;

namespace Interpreter.Abstractions {
        public static class ConfigurationSupport {
                private static readonly Dictionary<Type, Func<string, object>> Converters =
                        new Dictionary<Type, Func<string, object>> {
                                {typeof(bool), s => Convert.ToBoolean(s)}, {typeof(int), s => Convert.ToInt32(s)}
                        };

                public static T ConfigurationFor<T>(string configName, T def = default) {
                        var val = ConfigurationManager.AppSettings[configName];
                        return string.IsNullOrEmpty(val) ? def : (T) GetConverter<T>()(val);
                }

                private static Func<string, object> GetConverter<T>() {
                        var targetType = typeof(T);
                        return Converters.ContainsKey(targetType) ? Converters[targetType] : s => s;
                }
        }
}