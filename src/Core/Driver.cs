using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Maui.Automation.Driver
{
	public abstract class Driver : IDriver
	{
		public Driver(IAutomationConfiguration configuration, ILoggerFactory? loggerFactory = null)
		{
			Configuration = configuration;
			LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
		}

		public readonly ILoggerFactory LoggerFactory;

		public abstract string Name { get; }
		
		public IAutomationConfiguration Configuration { get; }

		public abstract Task Back();

		public abstract Task ClearAppState();

		public abstract void Dispose();

		public abstract Task<IDeviceInfo> GetDeviceInfo();

		public abstract Task InputText(IElement element, string text);

		public abstract Task ClearText(IElement element);

		public virtual async Task Start(bool forceReInstall = false, bool clearAppState = false)
		{
			await StopApp();
			if (forceReInstall)
				await RemoveApp();
			if (clearAppState)
				await ClearAppState();
			await InstallApp();
			await LaunchApp();
		}
		public abstract Task InstallApp();

		public abstract Task KeyPress(char keyCode);

		public abstract Task LaunchApp();

		public abstract Task LongPress(int x, int y);

		public abstract Task LongPress(IElement element);

		public abstract Task OpenUri(string uri);

		public abstract Task<IEnumerable<IElement>> GetElements(Platform automationPlatform);
		public Task<IEnumerable<IElement>> GetElements()
			=> GetElements(Configuration.AutomationPlatform);

		public abstract Task<string?> GetProperty(Platform automationPlatform, string elementId, string propertyName);
		public Task<string?> GetProperty(string elementId, string propertyName)
			=> GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

		public abstract Task<PerformActionResult> PerformAction(Platform automationPlatform, string action, string elementId, params string[] arguments);
		public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

		public abstract Task<string[]> Backdoor(Platform automationPlatform, string fullyQualifiedTypeName, string staticMethodName, string[] args);
		public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
			=> Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

		public abstract Task PullFile(string remoteFile, string localDirectory);

		public abstract Task PushFile(string localFile, string destinationDirectory);

		public abstract Task RemoveApp();

		public abstract Task StopApp();

		public abstract Task Swipe((int x, int y) start, (int x, int y) end);

		public abstract Task Tap(int x, int y);

		public abstract Task Tap(IElement element);

		int screenshotSequence = 0;

		protected string GetScreenshotFilename(string? filename = null)
		{
			screenshotSequence++;

			if (string.IsNullOrEmpty(filename))
				filename = $"{screenshotSequence}.png";

			string dir;
			if (!string.IsNullOrEmpty(filename) && Path.IsPathFullyQualified(filename))
				dir = Path.GetDirectoryName(filename)!;
			else
				dir = Configuration.Get(ConfigurationKeys.DriverScreenshotDirectory,
					Path.Combine(AppContext.BaseDirectory, "screenshots"))!;

			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			var fullFilename = Path.Combine(dir, filename);

			return fullFilename;
		}

		public abstract Task Screenshot(string? filename);
	}
}