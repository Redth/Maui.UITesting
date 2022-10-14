using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui.Automation.Driver;

public class AppDriverBuilder
{
	public static AppDriverBuilder WithConfig(string configFile)
	{
		var f = new FileInfo(configFile);
		if (!f.Exists)
			throw new FileNotFoundException(f.FullName);

		IAutomationConfiguration config;

		if (f.Extension.Equals(".yaml", StringComparison.OrdinalIgnoreCase))
			config = AutomationConfiguration.FromYaml(f.FullName);
		else if (f.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
			config = AutomationConfiguration.FromJson(f.FullName);
		else
			throw new NotSupportedException("Unsupported configuration file type. Must be .json or .yaml");

		return new AppDriverBuilder(config);
	}

	public readonly IAutomationConfiguration Configuration;

	protected readonly IHostBuilder HostBuilder;
	protected IHost? Host;

	public AppDriverBuilder()
	{
		HostBuilder = Extensions.Hosting.Host.CreateDefaultBuilder();
		Configuration = new AutomationConfiguration();
	}

	public AppDriverBuilder(IAutomationConfiguration configuration)
	{
		HostBuilder = Extensions.Hosting.Host.CreateDefaultBuilder();
		Configuration = configuration;
	}

	public AppDriverBuilder ConfigureDriver(Action<IAutomationConfiguration> configuration)
	{
		configuration(Configuration);
		return this;
	}

	public AppDriverBuilder AppId(string appId)
	{
		Configuration.AppId = appId;
		return this;
	}

	public AppDriverBuilder AppFilename(string appFilename)
	{
		Configuration.AppFilename = appFilename;
		return this;
	}

	public AppDriverBuilder Device(string device)
	{
		Configuration.Device = device;
		return this;
	}

	public AppDriverBuilder DevicePlatform(Platform devicePlatform)
	{
		Configuration.DevicePlatform = devicePlatform;
		return this;
	}

	public AppDriverBuilder AutomationPlatform(Platform automationPlatform)
	{
		Configuration.AutomationPlatform = automationPlatform;
		return this;
	}

	public AppDriverBuilder ConfigureLogging(Action<ILoggingBuilder> configure)
	{
		HostBuilder.ConfigureLogging(configure);
		return this;
	}

	public virtual IDriver Build()
	{
		Host = HostBuilder.Build();

		return new AppDriver(Configuration,
			Host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory);
	}
}
