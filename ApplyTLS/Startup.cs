using System;
using System.Net.Http;
using ApplyTLS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplyTLS
{
    public class Startup
    {
        private IConfiguration _configuration { get; }


        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddHealthChecks().AddCheck<GreeterHealthChecker>("example_health_check");

            //指定 endpoint 為 localhost，並忽略憑證有效性
            const string protocol = "https";

            // for healthy check
            services.AddGrpcClient<Greeter.GreeterClient>(o =>
                {
                    var host = _configuration.GetValue<string>("GrpcHost");
                    var port = _configuration.GetValue<int>("GrpcPort");

                    o.Address = new Uri($"{protocol}://{host}:{port}");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");

                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }
}