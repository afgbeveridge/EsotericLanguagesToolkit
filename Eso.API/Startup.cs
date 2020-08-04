using System.Net.WebSockets;
using Eso.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eso.API {
        public class Startup {
                public Startup(IConfiguration configuration) => Configuration = configuration;

                public IConfiguration Configuration { get; }

                // This method gets called by the runtime. Use this method to add services to the container.
                public void ConfigureServices(IServiceCollection services) {
                        services.AddControllers();
                        services.AddTransient<IWebSocketHandler, WebSocketHandler>();
                        services.AddTransient<IPluginService, PluginService>();
                        PluginService.Configure();
                        services.AddSwaggerGen();
                }

                // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
                public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
                        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

                        app.UseSwagger();

                        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                        // specifying the Swagger JSON endpoint.
                        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eso API v1"));


                        app.UseWebSockets();
                        app.Use(async (http, next) => {
                                if (!http.WebSockets.IsWebSocketRequest)
                                        // Nothing to do here, pass downstream.  
                                {
                                        await next();
                                }
                                else {
                                        var webSocket = await http.WebSockets.AcceptWebSocketAsync();
                                        if (webSocket != null && webSocket.State == WebSocketState.Open) {
                                                var svc = app.ApplicationServices.GetService<IWebSocketHandler>();
                                                await (await svc.Host(webSocket)).Process();
                                        }
                                }
                        });

                        app.UseRouting();

                        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                }
        }
}