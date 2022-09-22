using Google.Protobuf.WellKnownTypes;
using Microsoft.Maui.Automation.Remote;
using OpenQA.Selenium.Appium.Interactions;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace Microsoft.Maui.Automation.Driver
{
	public class WindowsDriver : Driver
	{
		public static class ConfigurationKeys
		{
			public const string WinAppDriverExePath = "windows-winappdriver-exe-path";
		}

		public WindowsDriver(IAutomationConfiguration configuration) : base(configuration)
		{
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
			appDriverProcess = new ProcessRunner(appDriverPath, Array.Empty<string>(), CancellationToken.None, true);
			// appDriverProcess.OutputLine += AppDriverProcess_OutputLine;
			
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

		public override string Name => "Windows";

		public override Task Back()
		{
			Session.Value.Navigate().Back();
			return Task.CompletedTask;
		}

		public override Task<IDeviceInfo> GetDeviceInfo()
		{
			return Task.FromResult<IDeviceInfo>(new DeviceInfo(0, 0, 0));
		}

		public override async Task InputText(Element element, string text)
		{
			await Tap(element);
			Session.Value.Keyboard.SendKeys(text);
		}

		public override Task ClearAppState()
		{
			Session.Value.ResetApp();
			return Task.CompletedTask;
		}

		public override Task RemoveApp()
		{
			Session.Value.RemoveApp(Configuration.AppId);
			return Task.CompletedTask;
		}

		public override Task StopApp()
		{
			Session.Value.CloseApp();
			return Task.CompletedTask;
		}

		public override Task InstallApp()
		{
			// Will this work?
			//Session.Value.InstallApp();

			// TODO: Figure out msix installation
			//var moduleFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "Modules", "Appx", "Appx.psd1");
			//var ps1 = $"Import-Module -SkipEditionCheck '{moduleFile}'; Add-AppxPackage -Path {Configuration.AppFilename} -AllowUnsigned -ForceApplicationShutdown -ForceUpdateFromAnyVersion";
			//PowershellUtil.Run(ps1);
			return Task.CompletedTask;
		}

		public override Task LaunchApp()
		{
			Session.Value.LaunchApp();
			return Task.CompletedTask;
		}

		public override async Task Start(bool forceReInstall = false, bool clearAppState = false)
		{
			var isRestart = Session.IsValueCreated;

			// Ensure it's created
			_ = Session.Value;

			if (isRestart)
			{
				await StopApp();
				if (forceReInstall)
					await RemoveApp();
				if (clearAppState)
					await ClearAppState();

				await InstallApp();
				await LaunchApp();
			}
		}

		public override Task OpenUri(string uri)
		{
			Session.Value.Navigate().GoToUrl(uri);
			return Task.CompletedTask;
		}

		public override Task PullFile(string remoteFile, string localDirectory)
		{
			var data = Session.Value.PullFile(remoteFile);

			if (data is not null)
			{
				var localFile = Path.Combine(localDirectory, Path.GetFileName(remoteFile));
				File.WriteAllBytes(localFile, data);
			}
			return Task.CompletedTask;
		}

		public override Task PushFile(string localFile, string destinationDirectory)
		{
			throw new NotSupportedException();
		}

		public override Task KeyPress(char keyCode)
		{
			Session.Value.PressKeyCode(keyCode);
			return Task.CompletedTask;
		}

		public override Task Swipe((int x, int y) start, (int x, int y) end)
		{
			var touchContact = new PointerInputDevice(OpenQA.Selenium.Interactions.PointerKind.Touch);
			var touchSequence = new OpenQA.Selenium.Interactions.ActionSequence(touchContact, 0);

			touchSequence.AddAction(touchContact.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, start.x, start.y, TimeSpan.Zero));
			touchSequence.AddAction(touchContact.CreatePointerDown(PointerButton.TouchContact));
			touchSequence.AddAction(touchContact.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, end.x, end.y, TimeSpan.FromMilliseconds(3000)));
			touchSequence.AddAction(touchContact.CreatePointerUp(PointerButton.TouchContact));

			Session.Value.PerformActions(new List<OpenQA.Selenium.Interactions.ActionSequence> { touchSequence });
			return Task.CompletedTask;
		}

		public override Task Tap(int x, int y)
			=> Tap(x, y, 400, 400);

		async Task Tap(int x, int y, int duration, int delay)
		{
			await Task.Delay(delay).ConfigureAwait(false);

			var touchContact = new PointerInputDevice(OpenQA.Selenium.Interactions.PointerKind.Touch);
			var touchSequence = new OpenQA.Selenium.Interactions.ActionSequence(touchContact, 0);

			touchSequence.AddAction(touchContact.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, x, y, TimeSpan.Zero));
			touchSequence.AddAction(touchContact.CreatePointerDown(PointerButton.TouchContact));
			touchSequence.AddAction(touchContact.CreatePointerMove(OpenQA.Selenium.Interactions.CoordinateOrigin.Viewport, x, y, TimeSpan.FromMilliseconds(duration)));
			touchSequence.AddAction(touchContact.CreatePointerUp(PointerButton.TouchContact));

			Session.Value.PerformActions(new List<OpenQA.Selenium.Interactions.ActionSequence> { touchSequence });
		}

		public override async Task Tap(Element element)
		{
			await Task.Delay(400);

			try
			{
				var winElement = Session.Value.FindElementByAccessibilityId(element.AutomationId);

				if (winElement is not null)
				{
					winElement.Click();
					return;
				}
			}
			catch { }

			var scale = element.Density;
			var x = element.WindowFrame.X + (element.WindowFrame.Width / 2);
			var y = element.WindowFrame.Y + (element.WindowFrame.Height / 2);
			

			await Tap((int)(x * scale), (int)(y * scale), 400, 50).ConfigureAwait(false);
		}

		public override Task LongPress(int x, int y)
		{
			var touch = new RemoteTouchScreen(Session.Value);
			touch.Down(x, y);
			Thread.Sleep(3000);
			touch.Up(x, y);
			return Task.CompletedTask;
		}

		public override Task LongPress(Element element)
			=> Tap(element);

		public override Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

		public override Task<IEnumerable<Element>> GetElements()
			=> base.SetDriver(grpc.Client.GetElements(Configuration.AutomationPlatform));

		public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

		public override Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
			=> grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

		public override async void Dispose()
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
