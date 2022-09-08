using System;
using AndroidSdk;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using System.Net;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Appium.Interactions;
using PointerInputDevice = OpenQA.Selenium.Appium.Interactions.PointerInputDevice;

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

			Session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
		}

		private void AppDriverProcess_OutputLine(object? sender, string e)
		{
			Console.WriteLine(e);
		}

		readonly ProcessRunner appDriverProcess;

		readonly WindowsDriver<WindowsElement> Session;

		readonly GrpcHost grpc;


		public string Name => "Windows";

		public IAutomationConfiguration Configuration { get; }

		public Task Back()
			=> Task.CompletedTask;

		public Task ClearAppState()
		{
			Session.ResetApp();
			return Task.CompletedTask;
		}

		public Task<IDeviceInfo> GetDeviceInfo()
		{
			return Task.FromResult<IDeviceInfo>(new DeviceInfo(0, 0, 0));
		}

		public Task InputText(string text)
		{
			Session.Keyboard.SendKeys(text);
			return Task.CompletedTask;
		}

		public Task InstallApp()
		{
			//Session.InstallApp(Configuration.AppFilename);

			return Task.CompletedTask;
		}

		public Task KeyPress(char keyCode)
		{
			Session.PressKeyCode(keyCode);
			return Task.CompletedTask;
		}

		public Task LaunchApp()
		{
			//Session.LaunchApp();
			return Task.CompletedTask;
		}

		public Task LongPress(int x, int y)
		{
			var touch = new RemoteTouchScreen(Session);
			touch.Down(x, y);
			Thread.Sleep(3000);
			touch.Up(x, y);
			return Task.CompletedTask;
		}

		public Task OpenUri(string uri)
		{
			Session.Navigate().GoToUrl(uri);
			return Task.CompletedTask;
		}

		public Task PullFile(string remoteFile, string localDirectory)
		{
			var data = Session.PullFile(remoteFile);

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
			Session.RemoveApp(Configuration.AppId);
			return Task.CompletedTask;
		}

		public Task StopApp()
		{
			Session.CloseApp();
			return Task.CompletedTask;
		}

		public Task Swipe((int x, int y) start, (int x, int y) end)
		{
			var touch = new RemoteTouchScreen(Session);
			touch.Down(start.x, start.y);
			Thread.Sleep(2000);
			touch.Up(end.x, end.y);
			return Task.CompletedTask;
		}

		public Task Tap(int x, int y)
		{
			//x = (int)(x * 2.25);
			//y = (int)(y * 2.25);
			
			var touch = new RemoteTouchScreen(Session);
			//touch.d
			touch.Down(x, y);
			Thread.Sleep(100);
			touch.Up(x, y);
			return Task.CompletedTask;
		}

		public async Task Tap(Element element)
		{
			var platformElements = (await grpc.Client.GetElements(Platform.Winappsdk));

			var windowsElement = platformElements?.FirstOrDefault(e => e.Id == element.Id);

			var x = (int)(windowsElement.X * 2.25);
			var y = (int)(windowsElement.Y * 2.25);
			var touchContact = new PointerInputDevice(PointerKind.Touch);
			var touchSequence = new ActionSequence(touchContact, 0);

			touchSequence.AddAction(touchContact.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.Zero));
			touchSequence.AddAction(touchContact.CreatePointerDown(PointerButton.TouchContact));
			touchSequence.AddAction(touchContact.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.FromMilliseconds(200)));
			touchSequence.AddAction(touchContact.CreatePointerUp(PointerButton.TouchContact));

			Session.PerformActions(new List<ActionSequence> { touchSequence });
			//return Task.CompletedTask;
		}

		public Task<string> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

		public Task<IEnumerable<Element>> GetElements()
			=> grpc.Client.GetElements(Configuration.AutomationPlatform);

		public Task<IEnumerable<Element>> FindElements(string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
			=> grpc.Client.FindElements(Configuration.AutomationPlatform, propertyName, pattern, isExpression, ancestorId);

		public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

		public async void Dispose()
		{
			try
			{
				Session?.Close();
				Session?.Dispose();
			} catch { }

			try
			{
				appDriverProcess?.Kill();
				appDriverProcess?.WaitForExit();
			} catch { }

			if (grpc is not null)
				await grpc.Stop();
		}
	}
}

