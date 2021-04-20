using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Eso.Blazor.SPA.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client {
        public class Program {
                public static async Task Main(string[] args) {
                        var builder = WebAssemblyHostBuilder.CreateDefault(args);

                        builder.Services
                                .AddBlazorise(options => {
                                        options.ChangeTextOnKeyPress = true;
                                })
                                .AddBootstrapProviders()
                                .AddFontAwesomeIcons();

                        builder.RootComponents.Add<App>("app");

                        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:61477") });
                        builder.Services.AddHttpClient<DiscoveryServiceClient>(client => client.BaseAddress = new Uri(builder.Configuration["DiscoveryUrl"]));
                        builder.Services.AddHttpClient<StatisticsServiceClient>(client => client.BaseAddress = new Uri(builder.Configuration["StatisticsUrl"]));
                        builder.Services.AddScoped<ILanguageService, NewLanguageService>();
                        builder.Services.AddScoped<ILanguageService, ExistingLanguageService>();

                        // await builder.Build().RunAsync();

                        var host = builder.Build();

                        host.Services
                          .UseBootstrapProviders()
                          .UseFontAwesomeIcons();

                        await host.RunAsync();
                }
        }
}
