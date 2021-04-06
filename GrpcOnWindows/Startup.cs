namespace GrpcOnWindows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using CompressionLevel = System.IO.Compression.CompressionLevel;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = false;
                options.AutomaticAuthentication = false;
                options.MaxRequestBodySize = 262144L;  // 256 KB
            });

            services.AddGrpc(options =>
            {
                options.CompressionProviders?.Clear();
                options.EnableDetailedErrors = true;
                options.IgnoreUnknownServices = false;
                options.MaxReceiveMessageSize = 131072;  // 128 KB
                options.MaxSendMessageSize = 131072;  // 128 KB
                options.ResponseCompressionLevel = CompressionLevel.NoCompression;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>()
                    .WithDisplayName("Greeter")
                    .AllowAnonymous();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(
                        "Communication with gRPC endpoints must be made through a gRPC client. " +
                        "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
