using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Automation.Remote;

namespace Microsoft.Maui.Automation.Remote;

public class GrpcHost
{
    public GrpcHost()
    {
        var builder = CreateHostBuilder(this, Array.Empty<string>());

        host = builder.Build();
        host.StartAsync();

        Client = host.Services.GetRequiredService<GrpcRemoteAppClient>();
    }

    public readonly GrpcRemoteAppClient Client;

	readonly IHost host;

    public static IHostBuilder CreateHostBuilder(GrpcHost grpcHost, string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .ConfigureServices(services => {
                    services.AddGrpc();
                    
                    services.AddSingleton<GrpcHost>(grpcHost);
                    services.AddSingleton<GrpcRemoteAppClient>();

				})
                .Configure(app =>
                    app.UseRouting()
                        .UseGrpcWeb()
                        .UseEndpoints(endpoints =>
                            endpoints.MapGrpcService<GrpcRemoteAppClient>().EnableGrpcWeb())));


    public Task Stop()
        => host?.StopAsync() ?? Task.CompletedTask;
}
