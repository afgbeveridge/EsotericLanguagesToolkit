using Eso.API.Editor.Repos;
using Eso.API.Editor.Services;
using Eso.API.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eso.API.Editor {
        public class Startup {
                public Startup(IConfiguration configuration) {
                        Configuration = configuration;
                }

                public IConfiguration Configuration { get; }

                // This method gets called by the runtime. Use this method to add services to the container.
                public void ConfigureServices(IServiceCollection services) {
                        services.AddCors(options => {
                                options.AddDefaultPolicy(
                                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                        });
                        var conn = Configuration.GetConnectionString("DefaultConnection");
                        services.AddDbContextPool<EditorDBContext>(options => options.UseMySql(conn, ServerVersion.AutoDetect(conn)))
                                .AddControllers();
                        services.AddScoped<IEsoLangRepository, EsoLangRepository>();
                        services.AddScoped<IExampleGenerator, ExampleGenerator>();
                        services.AddSingleton<IQueueSink, RabbitMQSink>();
                }

                // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
                public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
                        if (env.IsDevelopment()) {
                                app.UseDeveloperExceptionPage();
                        }

                        app.UseRouting();
                        app.UseCors();

                        app.UseAuthorization();

                        app.UseEndpoints(endpoints => {
                                endpoints.MapControllers();
                        });
                }
        }
}
