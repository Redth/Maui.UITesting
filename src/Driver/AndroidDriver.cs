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

public class AndroidDriver : Driver
{
	public AndroidDriver(IAutomationConfiguration configuration) : base(configuration)
	{
		if (string.IsNullOrEmpty(configuration.AppId))
			configuration.AppId = AppUtil.GetPackageId(configuration.AppFilename)
				?? throw new Exception("AppId not found");

		int port = 5000;
		//var address = IPAddress.Any.ToString();
		var adbDeviceSerial = configuration.Device;

		androidSdkManager = new AndroidSdkManager();
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

		var adbName = adbDeviceSerial;
		try {
			adbName = Adb.GetDeviceName(Device);
		} catch { }

		Name = $"Android ({adbName})";

		var forwardResult = Adb.RunCommand("-s", $"\"{Device}\"", "reverse", $"tcp:{port}", $"tcp:{port}")?.GetAllOutput();
		System.Diagnostics.Debug.WriteLine(forwardResult);

		grpc = new GrpcHost();
	}

	readonly GrpcHost grpc;

	protected readonly AndroidSdk.Adb Adb;
	protected readonly AndroidSdk.PackageManager Pm;

	readonly AndroidSdk.AndroidSdkManager androidSdkManager;
	protected readonly string Device;

	public override string Name { get; }

	string AppId => Configuration.AppId ?? throw new ArgumentNullException("AppId");

	public override Task Back()
	{
		Adb.Shell($"input keyevent 4", Device);
		return Task.CompletedTask;
	}

	public override Task ClearAppState()
	{
		if (!IsAppInstalled())
			return Task.CompletedTask;

		Adb.Shell($"pm clear {AppId}", Device);
		return Task.CompletedTask;
	}

	public override Task InstallApp()
	{
		try
		{
			Adb.Install(new System.IO.FileInfo(Configuration.AppFilename ?? throw new FileNotFoundException()), Device);
		}
		catch (SdkToolFailedExitException adbException)
		{
			throw adbException;
		}
		return Task.CompletedTask;
	}

	public override Task RemoveApp()
	{
		Adb.Uninstall(AppId, false, Device);
		return Task.CompletedTask;
	}

	public override Task<IDeviceInfo> GetDeviceInfo()
	{
		throw new NotImplementedException();
	}

	public override async Task InputText(Element element, string text)
	{
		await Tap(element);
		Adb.Shell($"input text {text}", Device);
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

		Adb.Shell($"input keyevent {code}", Device);
		return Task.CompletedTask;
	}

	public override Task LaunchApp()
	{
		// First force stop existing
		Adb.Shell($"am force-stop {AppId}", Device);

		// Launch app's activity
		Adb.Shell($"monkey -p {AppId} -c android.intent.category.LAUNCHER 1", Device);
		//Adb.Shell($"monkey --pct-syskeys 0 -p {appId} 1", Device);
		return Task.CompletedTask;
	}

	public override Task LongPress(int x, int y)
	{
		// Use a swipe that doesn't move as long press
		Adb.Shell($"input swipe {x} {y} {x} {y} 3000", Device);
		return Task.CompletedTask;
	}

	public override Task LongPress(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	public override Task OpenUri(string uri)
	{
		Adb.Shell($"am start -d {uri}", Device);
		return Task.CompletedTask;
	}

	public override Task StopApp()
	{
		// Force the app to stop
		// am force-stop $appId"
		Adb.Shell($"am force-stop {AppId}", Device);
		return Task.CompletedTask;
	}

	public override Task PushFile(string localFile, string destinationDirectory)
	{
		Adb.Push(new System.IO.FileInfo(localFile), new System.IO.DirectoryInfo(destinationDirectory), Device);
		return Task.CompletedTask;
	}

	public override Task PullFile(string remoteFile, string localDirectory)
	{
		Adb.Pull(new System.IO.FileInfo(remoteFile), new System.IO.DirectoryInfo(localDirectory), Device);
		return Task.CompletedTask;
	}

	public override Task Swipe((int x, int y) start, (int x, int y) end)
	{
		Adb.Shell($"input swipe {start.x} {start.y} {end.x} {end.y} 2000", Device);
		return Task.CompletedTask;
	}

	public override Task Tap(int x, int y)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, string.Empty, x.ToString(), y.ToString());

	public override Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	bool IsAppInstalled()
		=> androidSdkManager.PackageManager.ListPackages()
			.Any(p => p.PackageName?.Equals(AppId, StringComparison.OrdinalIgnoreCase) ?? false);


	public override Task<IEnumerable<Element>> GetElements()
		=> base.SetDriver(grpc.Client.GetElements(Configuration.AutomationPlatform));

	public override Task<string?> GetProperty(string elementId, string propertyName)
		=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

	public override Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public override async void Dispose()
	{
		if (grpc is not null)
			await grpc.Stop();
	}
}
