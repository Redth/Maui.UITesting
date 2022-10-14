using AndroidSdk;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation.Remote;

namespace Microsoft.Maui.Automation.Driver;

public class AndroidDriver : Driver
{
	public static class ConfigurationKeys
	{
		public const string AndroidSdkRoot = "Android:AndroidSdkRoot";
		public const string AutoStartEmulator = "Android:AutoStartEmulator";
	}

	public AndroidDriver(IAutomationConfiguration configuration, ILoggerFactory? loggerFactory = null)
		: base(configuration, loggerFactory)
	{
		adbLogger = LoggerFactory.CreateLogger("adb");

		if (string.IsNullOrEmpty(configuration.AppId))
			configuration.AppId = AppUtil.GetPackageId(configuration.AppFilename)
				?? throw new Exception("AppId not found");

		int port = configuration.Get(Microsoft.Maui.Automation.ConfigurationKeys.GrpcHostListenPort, 5000);
		var adbDeviceSerial = configuration.Device;

		string? androidSdkRoot = configuration.Get(ConfigurationKeys.AndroidSdkRoot, string.Empty);
		if (!string.IsNullOrEmpty(androidSdkRoot) && Directory.Exists(androidSdkRoot))
			androidSdkManager = new AndroidSdkManager(new DirectoryInfo(androidSdkRoot));
		else
			androidSdkManager = new AndroidSdkManager();

		Adb = androidSdkManager.Adb;
		Pm = androidSdkManager.PackageManager;
		Avd = androidSdkManager.AvdManager;
		Emulator = androidSdkManager.Emulator;

		deviceInfo = new Lazy<Task<IDeviceInfo>>(GetDeviceInfo);

		List<Adb.AdbDevice> adbDevices = new();

		WrapAdbTool(() => adbDevices = Adb.GetDevices());
		var foundDevice = false;

		// Use first if none were specified
		if (string.IsNullOrEmpty(adbDeviceSerial))
		{
			adbDeviceSerial = adbDevices.FirstOrDefault()?.Serial;
			if (!string.IsNullOrEmpty(adbDeviceSerial))
				AdbDeviceName = GetDeviceName(adbDeviceSerial) ?? adbDeviceSerial;
			foundDevice = true;
		}

		if (string.IsNullOrEmpty(adbDeviceSerial))
			throw new ArgumentNullException("Could not find an Android device");

		if (adbDevices.Any(d => d.Serial.Equals(adbDeviceSerial, StringComparison.OrdinalIgnoreCase)))
		{
			AdbDeviceName = GetDeviceName(adbDeviceSerial) ?? adbDeviceSerial;
			foundDevice = true;
		}

		// If we still haven't found a device, try looking for an emulator by name using the config device value
		if (!foundDevice)
		{
			// First let's see if there's a matching emulator running with the same avd name
			foreach (var adbDevice in adbDevices)
			{
				if (adbDevice.IsEmulator)
				{
					var name = string.Empty;
					try { name = GetDeviceName(adbDevice.Serial); }
					catch { }

					// Check if the device specified is actually the emulator name
					if (name?.Equals(adbDeviceSerial) ?? false)
					{
						// Use the serial from the device not the name
						adbDeviceSerial = adbDevice.Serial;
						AdbDeviceName = name;
						foundDevice = true;
					}
				}
			}

			// If still not found, let's look for a non-running avd with this name
			if (!foundDevice)
			{
				WrapAdbTool(() =>
				{
					var avds = Emulator.ListAvds();
					foreach (var avd in avds)
					{
						if (avd.Equals(adbDeviceSerial, StringComparison.OrdinalIgnoreCase))
						{
							emulatorProcess = Emulator.Start(avd);
							emulatorProcess.WaitForBootComplete(TimeSpan.FromSeconds(120));
							adbDeviceSerial = emulatorProcess.Serial;
							break;
						}
					}
				});
			}
		}

		ArgumentNullException.ThrowIfNull(adbDeviceSerial);
		if (string.IsNullOrEmpty(AdbDeviceName))
			AdbDeviceName = GetDeviceName(adbDeviceSerial) ?? adbDeviceSerial;

		Device = adbDeviceSerial;
		Pm.AdbSerial = adbDeviceSerial;

		Name = $"Android ({AdbDeviceName})";

		WrapAdbTool(() => 
			Adb.RunCommand("-s", $"\"{Device}\"", "reverse", $"tcp:{port}", $"tcp:{port}"));

		grpc = new GrpcHost(configuration, LoggerFactory);
	}

	readonly GrpcHost grpc;

	protected readonly AndroidSdk.Adb Adb;
	protected readonly AndroidSdk.PackageManager Pm;
	protected readonly AndroidSdk.AvdManager Avd;
	protected readonly AndroidSdk.Emulator Emulator;
	protected readonly ILogger adbLogger;

	readonly AndroidSdk.AndroidSdkManager androidSdkManager;

	AndroidSdk.Emulator.AndroidEmulatorProcess? emulatorProcess;

	protected readonly string Device;

	Lazy<Task<IDeviceInfo>> deviceInfo;

	public readonly string AdbDeviceName;

	public override string Name { get; }

	string AppId => Configuration.AppId ?? throw new ArgumentNullException("AppId");

	public override Task Back()
	{
		Shell($"input keyevent 4");
		return Task.CompletedTask;
	}

	public override Task ClearAppState()
	{
		if (!IsAppInstalled())
			return Task.CompletedTask;

		Shell($"pm clear {AppId}");
		return Task.CompletedTask;
	}

	public override Task InstallApp()
	{
		WrapAdbTool(() => 
			Adb.Install(new System.IO.FileInfo(Configuration.AppFilename ?? throw new FileNotFoundException()), Device));
		return Task.CompletedTask;
	}

	public override Task RemoveApp()
	{
		WrapAdbTool(() =>
			Adb.Uninstall(AppId, false, Device));
		return Task.CompletedTask;
	}

	public override Task<IDeviceInfo> GetDeviceInfo()
	{
		var density = new RegexParser(Shell("wm density")?.FirstOrDefault() ?? ": 1", @":\s+(?<density>[0-9]+)").GetGroup("density", 1);
		var rxSize = new RegexParser(Shell("wm size")?.FirstOrDefault() ?? ": 0x0", @":\s+(?<width>[0-9]+)x(?<height>[0-9]+)");

		return Task.FromResult<IDeviceInfo>(new DeviceInfo(
			(ulong)rxSize.GetGroup("width", 1),
			(ulong)rxSize.GetGroup("height", 1),
			(ulong)density));
	}

	public override async Task InputText(IElement element, string text)
	{
		// Hide keyboard first
		Shell($"input keyevent 111");
		
		// Tap the element to focus it
		await Tap(element);

		// Give it a moment
		await Task.Delay(500);

		// Send the text char by char
		// If you do it all at once, sometimes it gets jumbled up
		foreach (var c in text)
		{
			Shell($"input text {c}");
			await Task.Delay(100);
		}
	}

	public override async Task ClearText(IElement element)
	{
		await Tap(element);

		await Task.Delay(500);

		// 67 is backspace
		var deletes = string.Join(" ", Enumerable.Repeat("67", element.Text.Length));

		// Move cursor to end of text
		Shell($"input keyevent {deletes}");
	}

	public override Task KeyPress(char keyCode)
	{
		// Enter = 66
		// Backspace = 67
		// Back = 4
		// Volume Up = 24
		// VolumeDown = 25
		// Home = 3
		// Lock 276

		var code = keyCode switch
		{
			'\n' => 66,
			//"backspace" => 67,
			//"back" => 4,
			//"volumeup" => 24,
			//"volumedown" => 25,
			//"home" => 3,
			//"lock" => 276,
			_ => 0
		};

		// adb shell input keyevent $code
		if (code <= 0)
			throw new ArgumentOutOfRangeException(nameof(keyCode), "Unknown KeyCode");

		Shell($"input keyevent {code}");
		return Task.CompletedTask;
	}

	public override Task LaunchApp()
	{
		// First force stop existing
		Shell($"am force-stop {AppId}");

		// Launch app's activity
		Shell($"monkey -p {AppId} -c android.intent.category.LAUNCHER 1");
		//Shell($"monkey --pct-syskeys 0 -p {appId} 1");
		return Task.CompletedTask;
	}

	public override Task LongPress(int x, int y)
	{
		// Use a swipe that doesn't move as long press
		Shell($"input swipe {x} {y} {x} {y} 3000");
		return Task.CompletedTask;
	}

	public override Task LongPress(IElement element)
		=> LongPress(element.ScreenFrame.X, element.ScreenFrame.Y);

	public override Task OpenUri(string uri)
	{
		Shell($"am start -d {uri}");
		return Task.CompletedTask;
	}

	public override Task StopApp()
	{
		// Force the app to stop
		// am force-stop $appId"
		Shell($"am force-stop {AppId}");
		return Task.CompletedTask;
	}

	public override Task PushFile(string localFile, string destinationDirectory)
	{
		WrapAdbTool(() =>
			Adb.Push(new System.IO.FileInfo(localFile), new System.IO.DirectoryInfo(destinationDirectory), Device));
		return Task.CompletedTask;
	}

	public override Task PullFile(string remoteFile, string localDirectory)
	{
		WrapAdbTool(() =>
			Adb.Pull(new System.IO.FileInfo(remoteFile), new System.IO.DirectoryInfo(localDirectory), Device));
		return Task.CompletedTask;
	}

	public override Task Swipe((int x, int y) start, (int x, int y) end)
	{
		Shell($"input swipe {start.x} {start.y} {end.x} {end.y} 2000");
		return Task.CompletedTask;
	}

	public override Task Tap(int x, int y)
	{
		Shell($"input tap {x} {y}");
		return Task.CompletedTask;
	}

	public override Task Tap(IElement element)
		=> Tap(element.ScreenFrame.X, element.ScreenFrame.Y);


	bool IsAppInstalled()
		=> Pm.ListPackages()
			.Any(p => p.PackageName?.Equals(AppId, StringComparison.OrdinalIgnoreCase) ?? false);


	public override Task<IEnumerable<IElement>> GetElements(Platform automationPlatform)
		=> grpc.Client.GetElements(automationPlatform);

	public override Task<string?> GetProperty(Platform automationPlatform, string elementId, string propertyName)
		=> grpc.Client.GetProperty(automationPlatform, elementId, propertyName);

	public override Task<PerformActionResult> PerformAction(Platform automationPlatform, string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(automationPlatform, action, elementId, arguments);

	public override Task<string[]> Backdoor(Platform automationPlatform, string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> grpc.Client.Backdoor(automationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public override Task Screenshot(string? filename = null)
	{
		var fullFilename = base.GetScreenshotFilename(filename);
		var localDir = Path.Combine(Path.GetTempPath(), "adbshellpull");
		Directory.CreateDirectory(localDir);

		var remoteFile = $"/sdcard/{Guid.NewGuid().ToString("N")}.png";
		
		Shell($"screencap {remoteFile}");

		WrapAdbTool(() => Adb.Run("-s", $"\"{Device}\"", "pull", remoteFile, localDir));

		File.Move(Path.Combine(localDir, Path.GetFileName(remoteFile)), fullFilename, true);

		Shell($"rm {remoteFile}");

		return Task.CompletedTask;
	}

	string? GetDeviceName(string? serial)
	{
		if (string.IsNullOrEmpty(serial))
			return null;
		try
		{
			return WrapAdbTool(() => Adb.GetDeviceName(serial))?.FirstOrDefault();
		}
		catch
		{
			return serial;
		}
	}

	IEnumerable<string> Shell(string command)
		=> WrapAdbTool(() => Adb.Shell(command, Device));

	IEnumerable<string> WrapAdbTool(Func<string> cmd)
		=> WrapAdbTool(() => new string[] { cmd() });

	IEnumerable<string> WrapAdbTool(Action cmd)
		=> WrapAdbTool(() => {
			cmd();
			return Array.Empty<string>();
		});

	IEnumerable<string> WrapAdbTool(Func<AndroidSdk.ProcessResult> cmd)
		=> WrapAdbTool(() =>
			cmd().StandardOutput);

	IEnumerable<string> WrapAdbTool(Func<IEnumerable<string>> cmd)
	{
		try
		{
			var lines = cmd();

			foreach (var o in lines)
				adbLogger.LogInformation(o);

			return lines;
		}
		catch (SdkToolFailedExitException toolException)
		{
			foreach (var o in toolException.StdOut)
				adbLogger.LogInformation(o);

			adbLogger.LogError(toolException.Message);

			foreach (var o in toolException.StdErr)
				adbLogger.LogError(o);

			throw toolException;
		}
	}

	public override async ValueTask DisposeAsync()
	{
		await Task.WhenAll(
			grpc.DisposeAsync().AsTask(),
			Task.Run(() =>
			{
				emulatorProcess?.Shutdown();
				emulatorProcess?.WaitForExit();
			}));
	}
}
