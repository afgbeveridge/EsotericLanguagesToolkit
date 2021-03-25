using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Common;

namespace Eso.API.Services {
        public class PluginService : IPluginService {
                private static CompositionHost Container { get; set; }

                public IEsotericInterpreter InterpreterFor(string language)
                        => Container.GetExports<IEsotericInterpreter>().FirstOrDefault(interp =>
                                string.Equals(interp.Language, language, StringComparison.CurrentCultureIgnoreCase));

                public IEnumerable<IEsotericInterpreter> RegisteredInterpreters =>
                        Container.GetExports<IEsotericInterpreter>();

                internal static void Configure() {
                        try {
                                var conventions = new ConventionBuilder();
                                conventions
                                        .ForTypesDerivedFrom<IEsotericInterpreter>()
                                        .Export<IEsotericInterpreter>()
                                        .Shared();
                                var searchDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                                var configuration =
                                        new ContainerConfiguration().WithAssembliesInPath(searchDirectory, conventions);
                                Container = configuration.CreateContainer();
                        }
                        catch (Exception ex) {
                                if (ex is ReflectionTypeLoadException) {
                                        var typeLoadException = ex as ReflectionTypeLoadException;
                                        var loaderExceptions = typeLoadException.LoaderExceptions;
                                }
                        }
                }
        }

        public static class ContainerConfigurationExtensions {
                public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration,
                        string path, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
                        WithAssembliesInPath(configuration, path, null, searchOption);

                public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration,
                        string path, AttributedModelProvider conventions,
                        SearchOption searchOption = SearchOption.TopDirectoryOnly) {
                        var files = Directory.GetFiles(path, "*.dll", searchOption);
                        var names = files.Select(AssemblyLoadContext.GetAssemblyName);
                        var assemblies = names.Select(Assembly.Load).ToArray();
                        configuration = configuration.WithAssemblies(assemblies, conventions);
                        return configuration;
                }
        }
}