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

		public abstract Task InputText(Element element, string text);

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

		public abstract Task LongPress(Element element);

		public abstract Task OpenUri(string uri);

		protected async Task<IEnumerable<Element>> SetDriver(Task<IEnumerable<Element>> elements)
		{
			var allElements = await elements;
			foreach (var element in allElements)
			{
				element.Driver = this;
				if (element.Children.Any())
					SetDriver(element.Children);
			}
			return allElements;
		}

		void SetDriver(IEnumerable<Element> elements)
		{
			foreach (var element in elements)
			{
				element.Driver = this;
				if (element.Children.Any())
					SetDriver(element.Children);
			}
		}

		public abstract Task<IEnumerable<Element>> GetElements();

		public abstract Task<string?> GetProperty(string elementId, string propertyName);

		public abstract Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		public abstract Task PullFile(string remoteFile, string localDirectory);

		public abstract Task PushFile(string localFile, string destinationDirectory);

		public abstract Task RemoveApp();

		public abstract Task StopApp();

		public abstract Task Swipe((int x, int y) start, (int x, int y) end);

		public abstract Task Tap(int x, int y);

		public abstract Task Tap(Element element);

		public abstract Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args);

	}
}