namespace Microsoft.Maui.Automation.Driver
{
	public abstract class Driver : IDriver
	{
		public Driver(IAutomationConfiguration configuration)
		{
			Configuration = configuration;
		}

		public abstract string Name { get; }
		
		public IAutomationConfiguration Configuration { get; }

		public abstract Task Back();

		public abstract Task ClearAppState(string appId);
		public Task ClearAppState()
			=> ClearAppState(Configuration.AppId);


		public abstract void Dispose();

		public abstract Task<IDeviceInfo> GetDeviceInfo();

		public abstract Task InputText(string text);

		public abstract Task InstallApp(string file, string appId);

		public Task InstallApp()
			=> InstallApp(Configuration.AppFilename, Configuration.AppId);

		public abstract Task KeyPress(char keyCode);

		public abstract Task LaunchApp(string appId);

		public Task LaunchApp()
			=> LaunchApp(Configuration.AppId);

		public abstract Task LongPress(int x, int y);

		public abstract Task OpenUri(string uri);

		public abstract Task<IEnumerable<Element>> FindElements(string propertyName, string pattern, bool isExpression = false, string ancestorId = "");

		public abstract Task<IEnumerable<Element>> GetElements();

		public abstract Task<string> GetProperty(string elementId, string propertyName);

		public abstract Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		public abstract Task PullFile(string appId, string remoteFile, string localDirectory);

		public Task PullFile(string remoteFile, string localDirectory)
			=> PullFile(Configuration.AppId, remoteFile, localDirectory);

		public abstract Task PushFile(string appId, string localFile, string destinationDirectory);
		
		public Task PushFile(string localFile, string destinationDirectory)
			=> PushFile(Configuration.AppId, localFile, destinationDirectory);

		public abstract Task RemoveApp(string appId);

		public Task RemoveApp()
			=> RemoveApp(Configuration.AppId);

		public abstract Task StopApp(string appId);

		public Task StopApp()
			=> StopApp(Configuration.AppId);

		public abstract Task Swipe((int x, int y) start, (int x, int y) end);

		public abstract Task Tap(int x, int y);

		public abstract Task Tap(Element element);
	}
}