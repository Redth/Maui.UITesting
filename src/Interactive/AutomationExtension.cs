using System.CommandLine;
using System.CommandLine.Binding;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Maui.Automation.Driver;
using AndroidSdk;
using Microsoft.Maui.Automation.Util;
using Microsoft.DotNet.Interactive.Formatting;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Automation.Interactive;

public class AutomationExtensions : IKernelExtension
{
	TaskCompletionSource<IDictionary<string, List<string>>> tcsDevices = new();
	public Task<IDictionary<string, List<string>>> Devices
		=> tcsDevices.Task;

	public async Task OnLoadAsync(Kernel kernel)
	{
		var startedLoadingDevices = false;

		var installedOnKernels = new List<string>();
		var candidateKernels = kernel.FindKernels(k => k is CSharpKernel).Cast<CSharpKernel>();
		foreach (var csKernel in candidateKernels)
		{
			//if (csKernel.Directives.Any(d => d.Name.Equals("#!uitest", StringComparison.OrdinalIgnoreCase)))
			//{
				if (!startedLoadingDevices)
				{
					startedLoadingDevices = true;
					var _ = LoadDevices().ContinueWith(t => tcsDevices.TrySetResult(t.Result));
				}

				await ConfigureKernelDirective(csKernel);
				installedOnKernels.Add(csKernel.Name);
			//}
		}

		if (installedOnKernels.Count > 0 && KernelInvocationContext.Current is { } context)
		{
			PocketView view = div(
				code("Maui.UITesting"),
				$" is loaded. It adds commands for UI Testing/Automation.  Available on {string.Join(", ", installedOnKernels)}.",
				code("#!uitest -h")
			);

			context.Display(view);
		}

	}

	async Task ConfigureKernelDirective(CSharpKernel csharpKernel)
	{
		var platformOption = new Option<Platform>(
				new[] { "--platform" },
				() => Platform.Maui,
				"The device platform to test on.")
		{
			Arity = ArgumentArity.ExactlyOne
		};
		platformOption.AddCompletions("Android", "iOS", "MacCatalsyt", "tvOS", "WinAppSDK");

		var appIdOption = new Option<string>(
			new[] { "--app-id" },
			"The application id (bundle id, package id, etc) to test.")
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		var appOption = new Option<FileInfo?>(
			new[] { "--app" },
			"The application file (.app, .apk, etc.) to test.")
		{
			Arity = ArgumentArity.ZeroOrOne
		};
		var automationPlatformOption = new Option<Platform>(
			new[] { "--automation-platform" },
			() => Platform.Maui,
			"The automation platform to interact with, if different than the device platform (ie: Maui).")
		{
			Arity = ArgumentArity.ZeroOrOne
		};
		var deviceOption = new Option<string>(
			new[] { "--device" },
			getDefaultValue: () => string.Empty,
			description:
			"Device ID to test on (eg: ADB Serial for an android device, UDID of emulator for iOS/tvOS/ipadOS).")
		{
			Arity = ArgumentArity.ZeroOrOne
		};


		var allDevices = await Devices;

		deviceOption.AddCompletions(context =>
			context.ParseResult.GetValueForOption(automationPlatformOption) switch
			{
				Platform.Maui => allDevices.Values.SelectMany(v => v),
				Platform.Ios => allDevices["ios"],
				Platform.Tvos => allDevices["tvos"],
				Platform.Maccatalyst => allDevices["macos"],
				Platform.Macos => allDevices["macos"],
				Platform.Winappsdk => allDevices["windows"],
				_ => Enumerable.Empty<string>()
			});

		var nameOption = new Option<string>(
			new[] { "--name" },
			getDefaultValue: () => "Driver",
			description:
			"Name for the local variable.")
		{
			Arity = ArgumentArity.ZeroOrOne
		};


		var testCommand = new Command("#!uitest", "Starts a UI Testing Driver session")
			{
				platformOption,
				automationPlatformOption,
				appIdOption,
				appOption,
				deviceOption,
				nameOption
			};

		testCommand.SetHandler<Platform, Platform, string, FileInfo?, string, string>(
			async (Platform platform, Platform automationPlatform, string appId, FileInfo? app, string device, string name) =>
			{
				// if there is an already used dirver with such name dispose
				if (csharpKernel.TryGetValue(name, out IDriver oldDriver) && oldDriver is { })
				{
					await DisposeAppDriver(oldDriver);
				}

				var builder = new AppDriverBuilder()
					.DevicePlatform(platform)
					.AutomationPlatform(automationPlatform);

				if (app is not null)
					builder = builder.AppFilename(app.FullName);
				if (!string.IsNullOrEmpty(appId))
					builder = builder.AppId(appId);
				if (!string.IsNullOrEmpty(device))
					builder = builder.Device(device);

				var driver = builder.Build();

				csharpKernel.RegisterForDisposal(async () =>
				{
					await DisposeAppDriver(driver);
				});

				await driver.Start();

				await csharpKernel.SetValueAsync(name, driver);
			},
			platformOption,
			automationPlatformOption,
			appIdOption,
			appOption,
			deviceOption,
			nameOption);

		csharpKernel.AddDirective(testCommand);
	}

	Task DisposeAppDriver(IDriver driver)
	{
		try
		{
			driver.Dispose();
		}
		catch
		{ }

		return Task.CompletedTask;
	}

	async Task<IDictionary<string, List<string>>> LoadDevices()
	{
		var devices = new ConcurrentDictionary<string, List<string>>();
		devices["android"] = new();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			devices["ios"] = new();
			devices["tvos"] = new();
			devices["macos"] = new();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			devices["windows"] = new() { "Windows" };
		}

		var deviceTasks = new List<Task>();
		deviceTasks.Add(Task.Run(() => AddAndroidDevices(devices["android"])));

		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			deviceTasks.Add(Task.Run(() => AddAppleDevices("ios", devices["ios"])));
			deviceTasks.Add(Task.Run(() => AddAppleDevices("tvos", devices["tvos"])));
			deviceTasks.Add(Task.Run(() => AddAppleDevices("macos", devices["macos"])));
		}

		await Task.WhenAll(deviceTasks).ConfigureAwait(false);

		return devices;
	}

	void AddAndroidDevices(IList<string> devices)
	{
		try
		{
			var adb = new Adb();
			var adbDevices = adb.GetDevices();
			foreach (var device in adbDevices)
				devices.Add(device.Serial);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
		}

		try
		{
			var avd = new AvdManager();
			var avds = avd.ListAvds();
			foreach (var device in avds)
				devices.Add(device.Name);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
		}
	}

	void AddAppleDevices(string platform, IList<string> devices)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			try
			{
				var appleDevices = Xcode.GetDevices(platform).Select(d => d.Name);
				foreach (var device in appleDevices)
					devices.Add(device);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}
}

