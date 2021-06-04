using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Test
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory lf)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var logger = lf.CreateLogger("Startup");
                    LogContext.PushProperty("complex", new { Name = @"Property ""DOUBLE QUOTES"" on it", When = DateTime.UtcNow, Value = 42, Sub = new { Name = "Test", Iv = 32 } }, true);
                    LogContext.PushProperty("str", "Simple string property");
                    LogContext.PushProperty("int", 42);
                    LogContext.PushProperty("test", new[] {10, 100, 1000});
                    var value = @"This value also have ""double quotes"" on it";
                    logger.LogInformation(@"Message with ""double quotes"" and a str value: {value} :) ", value);

                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
