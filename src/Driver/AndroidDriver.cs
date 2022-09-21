using AndroidSdk;
using Grpc.Net.Client;
using Idb;
using Microsoft.Maui.Automation.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Driver;

public class AndroidDriver : IDriver
{
	public AndroidDriver(IAutomationConfiguration configuration)
	{
		Configuration = configuration;

		if (string.IsNullOrEmpty(Configuration.AppId))
			Configuration.AppId = AppUtil.GetPackageId(Configuration.AppFilename)
				?? throw new Exception("AppId not found");

		int port = 5000;
		//var address = IPAddress.Any.ToString();
		var adbDeviceSerial = configuration.Device;

		androidSdkManager = new AndroidSdk.AndroidSdkManager();
		Adb = androidSdkManager.Adb;
		Pm = androidSdkManager.PackageManager;

		if (string.IsNullOrEmpty(adbDeviceSerial))
		{
			var anySerial = Adb.GetDevices()?.FirstOrDefault()?.Serial;
			if (!string.IsNullOrEmpty(anySerial))
				adbDeviceSerial = anySerial;
		}

		ArgumentNullException.ThrowIfNull(adbDeviceSerial);
		Device = adbDeviceSerial;
		Pm.AdbSerial = adbDeviceSerial;

		Name = $"Android ({Adb.GetDeviceName(Device)})";

		var forwardResult = Adb.RunCommand("-s", $"\"{Device}\"", "reverse", $"tcp:{port}", $"tcp:{port}")?.GetAllOutput();
		System.Diagnostics.Debug.WriteLine(forwardResult);

		grpc = new GrpcHost();
	}

	readonly GrpcHost grpc;

	protected readonly AndroidSdk.Adb Adb;
	protected readonly AndroidSdk.PackageManager Pm;

	readonly AndroidSdk.AndroidSdkManager androidSdkManager;
	protected readonly string Device;

	public IAutomationConfiguration Configuration { get; }

	public string Name { get; }

	string AppId => Configuration.AppId ?? throw new ArgumentNullException("AppId");

	public Task Back()
	{
		Adb.Shell($"input keyevent 4", Device);
		return Task.CompletedTask;
	}

	public Task ClearAppState()
	{
		if (!IsAppInstalled())
			return Task.CompletedTask;

		Adb.Shell($"pm clear {AppId}", Device);
		return Task.CompletedTask;
	}

	public Task InstallApp()
	{
		Adb.Install(new System.IO.FileInfo(Configuration.AppFilename ?? throw new FileNotFoundException()), Device);
		return Task.CompletedTask;
	}

	public Task RemoveApp()
	{
		Adb.Uninstall(AppId, false, Device);
		return Task.CompletedTask;
	}

	public Task<IDeviceInfo> GetDeviceInfo()
	{
		throw new NotImplementedException();
	}

	public Task InputText(string text)
	{
		Adb.Shell($"input text {text}", Device);
		return Task.CompletedTask;
	}

	public Task KeyPress(char keyCode)
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

		Adb.Shell($"input keyevent {code}", Device);
		return Task.CompletedTask;
	}

	public Task LaunchApp()
	{
		// First force stop existing
		Adb.Shell($"am force-stop {AppId}", Device);

		// Launch app's activity
		Adb.Shell($"monkey -p {AppId} -c android.intent.category.LAUNCHER 1", Device);
		//Adb.Shell($"monkey --pct-syskeys 0 -p {appId} 1", Device);
		return Task.CompletedTask;
	}

	public Task LongPress(int x, int y)
	{
		// Use a swipe that doesn't move as long press
		Adb.Shell($"input swipe {x} {y} {x} {y} 3000", Device);
		return Task.CompletedTask;
	}

	public Task LongPress(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	public Task OpenUri(string uri)
	{
		Adb.Shell($"am start -d {uri}", Device);
		return Task.CompletedTask;
	}

	public Task StopApp()
	{
		// Force the app to stop
		// am force-stop $appId"
		Adb.Shell($"am force-stop {AppId}", Device);
		return Task.CompletedTask;
	}

	public Task PushFile(string localFile, string destinationDirectory)
	{
		Adb.Push(new System.IO.FileInfo(localFile), new System.IO.DirectoryInfo(destinationDirectory), Device);
		return Task.CompletedTask;
	}

	public Task PullFile(string remoteFile, string localDirectory)
	{
		Adb.Pull(new System.IO.FileInfo(remoteFile), new System.IO.DirectoryInfo(localDirectory), Device);
		return Task.CompletedTask;
	}

	public Task Swipe((int x, int y) start, (int x, int y) end)
	{
		Adb.Shell($"input swipe {start.x} {start.y} {end.x} {end.y} 2000", Device);
		return Task.CompletedTask;
	}

	public Task Tap(int x, int y)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, string.Empty, x.ToString(), y.ToString());

	public Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	bool IsAppInstalled()
		=> androidSdkManager.PackageManager.ListPackages()
			.Any(p => p.PackageName?.Equals(AppId, StringComparison.OrdinalIgnoreCase) ?? false);


	public Task<IEnumerable<Element>> GetElements()
        => grpc.Client.GetElements(Configuration.AutomationPlatform);

    public Task<string?> GetProperty(string elementId, string propertyName)
		=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

    public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
        => grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public async void Dispose()
	{
		if (grpc is not null)
			await grpc.Stop();
	}
}
