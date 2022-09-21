using Microsoft.Maui.Automation.Remote;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace Microsoft.Maui.Automation.Driver
{
	public class WindowsDriver : IDriver
	{
		public static class ConfigurationKeys
		{
			public const string WinAppDriverExePath = "windows-winappdriver-exe-path";
		}

		public WindowsDriver(IAutomationConfiguration configuration)
		{
			Configuration = configuration;

			var appFile = configuration.AppFilename;

			if (string.IsNullOrEmpty(configuration.AppId))
			{
				if (!string.IsNullOrEmpty(appFile) && Path.GetExtension(appFile).Equals(".msix"))
				{
					// Infer id from appx manifest
					var appId = AppUtil.GetAppxId(appFile);
					if (!string.IsNullOrEmpty(appId))
						Configuration.AppId = appId;
				}
			}

			var appDriverPath = configuration.Get(ConfigurationKeys.WinAppDriverExePath, null);

			if (string.IsNullOrEmpty(appDriverPath) || !File.Exists(appDriverPath))
				appDriverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Windows Application Driver", "WinAppDriver.exe");

			if (string.IsNullOrEmpty(appDriverPath) || !File.Exists(appDriverPath))
				appDriverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Application Driver", "WinAppDriver.exe");
			if (string.IsNullOrEmpty(appDriverPath) || !File.Exists(appDriverPath))
				throw new FileNotFoundException("Unable to locate WinAppDriver.exe, please install from: https://github.com/Microsoft/WinAppDriver");

			appDriverProcess = null;
			//appDriverProcess = new ProcessRunner(appDriverPath);
			var appCapabilities = new DesiredCapabilities();
			appCapabilities.SetCapability("app", configuration.AppId);
			appCapabilities.SetCapability("platformName", "Windows");
			appCapabilities.SetCapability("platformVersion", "1.0");

			grpc = new GrpcHost();

			Session = new Lazy<WindowsDriver<WindowsElement>>(() =>
				new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities));
		}

		private void AppDriverProcess_OutputLine(object? sender, string e)
		{
			Console.WriteLine(e);
		}

		readonly ProcessRunner? appDriverProcess;

		readonly Lazy<WindowsDriver<WindowsElement>> Session;

		readonly GrpcHost grpc;


		public string Name => "Windows";

		public IAutomationConfiguration Configuration { get; }

		public Task Back()
			=> Task.CompletedTask;

		public Task ClearAppState()
		{
			Session.Value.ResetApp();
			return Task.CompletedTask;
		}

		public Task<IDeviceInfo> GetDeviceInfo()
		{
			return Task.FromResult<IDeviceInfo>(new DeviceInfo(0, 0, 0));
		}

		public Task InputText(string text)
		{
			Session.Value.Keyboard.SendKeys(text);
			return Task.CompletedTask;
		}

		public Task InstallApp()
		{
			var moduleFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "Modules", "Appx", "Appx.psd1");
			var ps1 = $"Import-Module -SkipEditionCheck '{moduleFile}'; Add-AppxPackage -Path {Configuration.AppFilename} -AllowUnsigned -ForceApplicationShutdown -ForceUpdateFromAnyVersion";
			PowershellUtil.Run(ps1);
			return Task.CompletedTask;
		}

		public Task KeyPress(char keyCode)
		{
			Session.Value.PressKeyCode(keyCode);
			return Task.CompletedTask;
		}

		public Task LaunchApp()
		{
			//Session.LaunchApp();
			return Task.CompletedTask;
		}

		public Task LongPress(int x, int y)
		{
			var touch = new RemoteTouchScreen(Session.Value);
			touch.Down(x, y);
			Thread.Sleep(3000);
			touch.Up(x, y);
			return Task.CompletedTask;
		}

		public Task LongPress(Element element)
			=> Tap(element);


		public Task OpenUri(string uri)
		{
			Session.Value.Navigate().GoToUrl(uri);
			return Task.CompletedTask;
		}

		public Task PullFile(string remoteFile, string localDirectory)
		{
			var data = Session.Value.PullFile(remoteFile);

			if (data is not null)
			{
				var localFile = Path.Combine(localDirectory, Path.GetFileName(remoteFile));
				File.WriteAllBytes(localFile, data);
			}
			return Task.CompletedTask;
		}

		public Task PushFile(string localFile, string destinationDirectory)
		{
			throw new NotSupportedException();
		}

		public Task RemoveApp()
		{
			Session.Value.RemoveApp(Configuration.AppId);
			return Task.CompletedTask;
		}

		public Task StopApp()
		{
			Session.Value.CloseApp();
			return Task.CompletedTask;
		}

		public Task Swipe((int x, int y) start, (int x, int y) end)
		{
			var touch = new RemoteTouchScreen(Session.Value);
			touch.Down(start.x, start.y);
			Thread.Sleep(2000);
			touch.Up(end.x, end.y);
			return Task.CompletedTask;
		}

		public Task Tap(int x, int y)
		{
			//x = (int)(x * 2.25);
			//y = (int)(y * 2.25);

			var touch = new RemoteTouchScreen(Session.Value);
			//touch.d
			touch.Down(x, y);
			Thread.Sleep(100);
			touch.Up(x, y);
			return Task.CompletedTask;
		}

		public Task Tap(Element element)
		{
			var winElement = Session.Value.FindElementByAccessibilityId(element.AutomationId);
			winElement.Click();
			//var we = Session.FindElements(OpenQA.Selenium.By.XPath("//*"))?.ToList();

			//we?.FirstOrDefault()?.Click();

			//var platformElements = (await grpc.Client.GetElements(Platform.Winappsdk));

			//var windowsElement = platformElements?.FirstOrDefault(e => e.Id == element.Id);

			//var x = (int)(windowsElement.X * 2.25);
			//var y = (int)(windowsElement.Y * 2.25);
			//var touchContact = new PointerInputDevice(PointerKind.Touch);
			//var touchSequence = new ActionSequence(touchContact, 0);

			//touchSequence.AddAction(touchContact.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.Zero));
			//touchSequence.AddAction(touchContact.CreatePointerDown(PointerButton.TouchContact));
			//touchSequence.AddAction(touchContact.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.FromMilliseconds(200)));
			//touchSequence.AddAction(touchContact.CreatePointerUp(PointerButton.TouchContact));

			//Session.PerformActions(new List<ActionSequence> { touchSequence });
			return Task.CompletedTask;
		}

		public Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

		public Task<IEnumerable<Element>> GetElements()
			=> grpc.Client.GetElements(Configuration.AutomationPlatform);

		public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

        public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
			=> grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

        public async void Dispose()
		{
			try
			{
				Session?.Value?.Close();
				Session?.Value?.Dispose();
			}
			catch { }

			try
			{
				appDriverProcess?.Kill();
				appDriverProcess?.WaitForExit();
			}
			catch { }

			if (grpc is not null)
				await grpc.Stop();
		}
	}
}
