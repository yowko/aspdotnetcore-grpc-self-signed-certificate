using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApplyTLS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel((context, kestrelOptions) =>
                        {
                            var port = context.Configuration.GetValue<int>("GrpcPort");

                            kestrelOptions.ListenAnyIP(port, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                listenOptions.UseHttps(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"), 
                                        Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password"));
                            });
                        });
                });
    }
}