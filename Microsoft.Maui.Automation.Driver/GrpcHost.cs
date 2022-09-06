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
        host.StartAsync().ContinueWith(t =>
        {
            var svcs = host.Services;
        });


    }

    public void SetCurrentClient(GrpcRemoteAppClient client)
    {
        currentClient.TrySetResult(client);
    }

    TaskCompletionSource<GrpcRemoteAppClient> currentClient { get; set; } = new ();

    public Task<GrpcRemoteAppClient> CurrentClient => currentClient.Task;

    readonly IHost host;

    public static IHostBuilder CreateHostBuilder(GrpcHost grpcHost, string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .ConfigureServices(services => {
                    services.AddGrpc();
                    services.AddSingleton<GrpcHost>(grpcHost);
                })
                .Configure(app =>
                    app.UseRouting().UseEndpoints(endpoints =>
                        endpoints.MapGrpcService<GrpcRemoteAppClient>())));


    public Task Stop()
        => host?.StopAsync() ?? Task.CompletedTask;
}
