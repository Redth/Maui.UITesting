using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Remote;

public class GrpcHost : IAsyncDisposable
{
	public GrpcHost(IAutomationConfiguration configuration, ILoggerFactory? loggerFactory)
	{
		LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

		var builder = CreateHostBuilder(this, configuration);

		host = builder.Build();
		host.Start();

		Services = host.Services;
		Client = host.Services.GetRequiredService<GrpcRemoteAppClient>();
	}

	public readonly GrpcRemoteAppClient Client;

	public readonly ILoggerFactory LoggerFactory;
	readonly IWebHost host;

	public readonly IServiceProvider Services;

	public static IWebHostBuilder CreateHostBuilder(GrpcHost grpcHost, IAutomationConfiguration configuration)
	{
		var builder = new WebHostBuilder()
			.ConfigureLogging(log =>
			{
				log.ClearProviders();

				if (configuration.Get(ConfigurationKeys.GrpcHostLoggingEnabled, false))
					log.AddProvider(new LoggerFactoryInstanceLoggerProvider(grpcHost.LoggerFactory));
			})
			.UseKestrel(kestrel =>
			{
				var listenPort = configuration.Get(ConfigurationKeys.GrpcHostListenPort, 5000);
				kestrel.ListenAnyIP(listenPort, listen =>
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

	public async ValueTask DisposeAsync()
	{
		if (host is not null)
		{
			try { await host.StopAsync(); }
			catch { }

			host.Dispose();
		}
		
	}
}

internal class LoggerFactoryInstanceLoggerProvider : ILoggerProvider
{
	public LoggerFactoryInstanceLoggerProvider(ILoggerFactory loggerFactory)
	{
		LoggerFactory = loggerFactory;
	}

	public readonly ILoggerFactory LoggerFactory;

	public ILogger CreateLogger(string categoryName)
		=> LoggerFactory.CreateLogger(categoryName);

	public void Dispose()
	{
	}
}