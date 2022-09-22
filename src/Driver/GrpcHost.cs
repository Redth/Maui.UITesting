using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Remote;

public class GrpcHost
{
	public GrpcHost(IAutomationConfiguration configuration)
	{
		var builder = CreateHostBuilder(this);

		host = builder.Build();
		host.Start();

		Services = host.Services;
		Client = host.Services.GetRequiredService<GrpcRemoteAppClient>();
	}

	public readonly GrpcRemoteAppClient Client;

	readonly IWebHost host;

	public readonly IServiceProvider Services;

	public ILogger<GrpcHost> Logger => Services.GetRequiredService<ILogger<GrpcHost>>();

	public static IWebHostBuilder CreateHostBuilder(GrpcHost grpcHost, IAutomationConfiguration configuration)
	{
		var builder = new WebHostBuilder()
			.ConfigureLogging(log =>
			{
				if (configuration.Get(ConfigurationKeys.GrpcHostLoggingEnabled, false))
					log.AddConsole();
			})
			.UseKestrel(kestrel =>
			{
				kestrel.ListenAnyIP(5000, listen =>
				{
					listen.Protocols = HttpProtocols.Http2;
				});
			})
			.ConfigureServices(services =>
			{
				services.AddGrpc();
				services.AddSingleton<GrpcHost>(grpcHost);
				services.AddSingleton<GrpcRemoteAppClient>();
			})
			.Configure(app =>
			{
				app
					.UseRouting()
					.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true })
					.UseEndpoints(endpoints =>
					{
						endpoints.MapGrpcService<GrpcRemoteAppClient>();
					});
			});

		return builder;
	}

	public Task Stop()
		=> host?.StopAsync() ?? Task.CompletedTask;
}
