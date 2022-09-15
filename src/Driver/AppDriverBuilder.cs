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

	public AppDriverBuilder()
	{
		Configuration = new AutomationConfiguration();
	}

	public AppDriverBuilder(IAutomationConfiguration configuration)
	{
		Configuration = configuration;
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

	public IDriver Build()
		=> new AppDriver(Configuration);
}
