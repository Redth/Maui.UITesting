using AndroidSdk;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Idb;
using Grpc.Core;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Microsoft.Maui.Automation.Driver;

public class iOSDriver : IDriver
{
	public iOSDriver(IAutomationConfiguration configuration)
	{
		Configuration = configuration;

		var port = configuration.AppAgentPort;
		var address = configuration.AppAgentAddress;

		if (configuration.DevicePlatform == Platform.Maccatalyst
			|| configuration.DevicePlatform == Platform.Macos)
			configuration.Device = "mac";

		ArgumentNullException.ThrowIfNull(configuration.Device);

		Name = $"iOS ({configuration.Device})";

		if (string.IsNullOrEmpty(Configuration.AppId))
			Configuration.AppId = AppUtil.GetBundleIdentifier(Configuration.AppFilename)
				?? throw new Exception("AppId not found");

		idbCompanionPath = UnpackIdb();

		idbCompanionProcess = new ProcessRunner(idbCompanionPath, $"--boot {Configuration.Device}");
		var bootResult = idbCompanionProcess.WaitForExit();
		Console.WriteLine(bootResult.GetAllOutput());


		idbCompanionProcess = new ProcessRunner(idbCompanionPath, $"--udid {configuration.Device}");

		// Sleep until idb exited or started
		while (true)
		{
			Thread.Sleep(500);

			if (idbCompanionProcess.HasExited)
				throw new Exception("Failed to start idb");

			var output = idbCompanionProcess.Output.ToList();

			if (output.Any(s => s?.Contains("\"grpc_swift_port\":10882") ?? false))
				break;
		}

		var channel = GrpcChannel.ForAddress($"http://{address}:10882");
		idb = new Idb.CompanionService.CompanionServiceClient(channel);

		var connectResponse = idb.connect(new Idb.ConnectRequest());
		Udid = connectResponse.Companion.Udid;

		Name = $"iOS ({configuration.Device} [{Udid}])";

		grpc = new GrpcHost();
	}

	public readonly string Udid;

	readonly CompanionService.CompanionServiceClient idb;
	readonly GrpcHost grpc;
	readonly string idbCompanionPath;
	readonly ProcessRunner idbCompanionProcess;

	public string Name { get; }

	public IAutomationConfiguration Configuration { get; }

	public async Task ClearAppState()
	{
		var req = new RmRequest
		{
			Container = new FileContainer
			{
				Kind = FileContainer.Types.Kind.Application,
				BundleId = Configuration.AppId
			}
		};
		req.Paths.Add("/");

		await idb.rmAsync(req);

		await idb.mkdirAsync(new MkdirRequest
		{
			Container = new FileContainer
			{
				Kind = FileContainer.Types.Kind.Application,
				BundleId = Configuration.AppId
			},
			Path = "tmp"
		});
	}

	public Task InstallApp()
	{
		if (Configuration.DevicePlatform == Platform.Maccatalyst
			|| Configuration.DevicePlatform == Platform.Macos)
			return Task.CompletedTask;

		var payload = new Payload();
		payload.FilePath = Configuration.AppFilename;


		return idb.install().RequestStream<InstallRequest, InstallResponse>(
			new[]
			{
				new InstallRequest { Destination = InstallRequest.Types.Destination.App },
				new InstallRequest { Payload = payload }
			},
			response =>
			{
				Console.WriteLine($"{response.Uuid} -> {response.Name} -> {response.Progress}");
			});
	}

	public Task RemoveApp()
		=> idb.uninstallAsync(new UninstallRequest
		{
			BundleId = Configuration.AppId
		}).ResponseAsync;

	public async Task<IDeviceInfo> GetDeviceInfo()
	{
		var desc = await idb.describeAsync(new Idb.TargetDescriptionRequest { FetchDiagnostics = true });

		var width = desc.TargetDescription.ScreenDimensions.Width;
		var height = desc.TargetDescription.ScreenDimensions.Height;
		var density = desc.TargetDescription.ScreenDimensions.Density;

		return new DeviceInfo(width, height, density);
	}



	public async Task LaunchApp()
	{
		if (Configuration.DevicePlatform == Platform.Maccatalyst
			|| Configuration.DevicePlatform == Platform.Macos)
		{
			await Process.Start(new ProcessStartInfo("/usr/bin/open", $"-n \"{Configuration.AppFilename}\"")
			{
				CreateNoWindow = true,
				UseShellExecute = true
			})!.WaitForExitAsync();

			return;
		}

		await idb.launch().RequestStream<LaunchRequest, LaunchResponse>(
			new LaunchRequest
			{
				Start = new LaunchRequest.Types.Start
				{
					BundleId = Configuration.AppId,
					ForegroundIfRunning = true,
				}
			});
	}

	public Task StopApp()
		=> idb.terminateAsync(new TerminateRequest
		{
			BundleId = Configuration.AppId
		}).ResponseAsync;

	public Task OpenUri(string uri)
		=> idb.open_urlAsync(new OpenUrlRequest
		{
			Url = uri
		}).ResponseAsync;


	public Task PushFile(string localFile, string destinationDirectory)
		=> idb.push().SendStream(new PushRequest
		{
			Inner = new PushRequest.Types.Inner
			{
				Container = new FileContainer
				{
					BundleId = Configuration.AppId,
					Kind = FileContainer.Types.Kind.ApplicationContainer,
				},
				DstPath = destinationDirectory
			},
			Payload = new Payload
			{
				FilePath = localFile,
			}
		});

	public Task PullFile(string remoteFile, string localDirectory)
		=> idb.pull(new PullRequest
		{
			Container = new FileContainer
			{
				BundleId = Configuration.AppId,
				Kind = FileContainer.Types.Kind.Application
			},
			DstPath = localDirectory,
			SrcPath = remoteFile
		}).ReceiveStream(r =>
		{
			Console.WriteLine($"{r.Payload.FilePath}");
		});


	public Task InputText(string text)
		=> idb.hid().SendStream<HIDEvent, HIDResponse>(text.AsHidEvents().ToArray());

	public Task Back()
		=> Task.CompletedTask;

	public Task KeyPress(char keyCode)
		=> idb.hid().SendStream<HIDEvent, HIDResponse>(keyCode.AsHidEvents().ToArray());

	public Task Tap(int x, int y)
		=> press(x, y, TimeSpan.FromMilliseconds(50));

	public Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	public Task LongPress(int x, int y)
		=> press(x, y, TimeSpan.FromSeconds(3));

	public Task LongPress(Element element)
			=> Tap(element);

	async Task press(int x, int y, TimeSpan holdDelay)
	{
		var pressAction = new HIDEvent.Types.HIDPressAction
		{
			Touch = new HIDEvent.Types.HIDTouch
			{
				Point = new Idb.Point
				{
					X = x,
					Y = y
				}
			}
		};

		await idb.hid().SendStream(
			holdDelay,
			new[] {
					new HIDEvent
				{
					Press = new HIDEvent.Types.HIDPress
					{
						Action = pressAction,
						Direction = HIDEvent.Types.HIDDirection.Down
					}
				},
				new HIDEvent
				{
					Press = new HIDEvent.Types.HIDPress
					{
						Action = pressAction,
						Direction = HIDEvent.Types.HIDDirection.Down
					}
				}
			});
	}

	public Task Swipe((int x, int y) start, (int x, int y) end)
		=> idb.hid().SendStream(
			new HIDEvent
			{
				Swipe = new HIDEvent.Types.HIDSwipe
				{
					Start = new Idb.Point
					{
						X = start.x,
						Y = start.y
					},
					End = new Idb.Point
					{
						X = end.x,
						Y = end.y
					}
				}
			});

	public Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public Task<IEnumerable<Element>> GetElements()
		=> grpc.Client.GetElements(Configuration.AutomationPlatform);

	public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

	public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> grpc.Client.Backdoor(Configuration.AutomationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	string UnpackIdb()
	{
		var outputDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"idb");

		if (!Directory.Exists(outputDir))
			Directory.CreateDirectory(outputDir);

		var exePath = Path.Combine(outputDir, "idb-companion.universal", "bin", "idb_companion");
		if (!File.Exists(exePath))
			EmbeddedToolUtil.ExtractEmbeddedResourceTarGz("idb-companion.tar.gz", outputDir);

		return exePath;
	}

	public async void Dispose()
	{
		try
		{
			idbCompanionProcess?.Kill();
			idbCompanionProcess?.WaitForExit();
		}
		catch { }

		if (grpc is not null)
			await grpc.Stop();
	}
}
