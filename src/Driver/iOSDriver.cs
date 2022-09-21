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

public class iOSDriver : Driver
{
	public iOSDriver(IAutomationConfiguration configuration) : base(configuration)
	{
		var port = configuration.AppAgentPort;
		var address = configuration.AppAgentAddress;

		ArgumentNullException.ThrowIfNull(configuration.Device);

		Name = $"iOS ({configuration.Device})";

		if (string.IsNullOrEmpty(configuration.AppId))
			configuration.AppId = AppUtil.GetBundleIdentifier(configuration.AppFilename)
				?? throw new Exception("AppId not found");

		idbCompanionPath = UnpackIdb();

		idbCompanionProcess = new ProcessRunner(idbCompanionPath, $"--boot {configuration.Device}");
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

	public override string Name { get; }

	public override async Task ClearAppState()
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

	public override Task InstallApp()
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

	public override Task RemoveApp()
		=> idb.uninstallAsync(new UninstallRequest
		{
			BundleId = Configuration.AppId
		}).ResponseAsync;

	public override async Task<IDeviceInfo> GetDeviceInfo()
	{
		var desc = await idb.describeAsync(new Idb.TargetDescriptionRequest { FetchDiagnostics = true });

		var width = desc.TargetDescription.ScreenDimensions.Width;
		var height = desc.TargetDescription.ScreenDimensions.Height;
		var density = desc.TargetDescription.ScreenDimensions.Density;

		return new DeviceInfo(width, height, density);
	}



	public override async Task LaunchApp()
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

	public override Task StopApp()
		=> idb.terminateAsync(new TerminateRequest
		{
			BundleId = Configuration.AppId
		}).ResponseAsync;

	public override Task OpenUri(string uri)
		=> idb.open_urlAsync(new OpenUrlRequest
		{
			Url = uri
		}).ResponseAsync;


	public override Task PushFile(string localFile, string destinationDirectory)
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

	public override Task PullFile(string remoteFile, string localDirectory)
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


	public override Task InputText(string text)
		=> idb.hid().SendStream<HIDEvent, HIDResponse>(text.AsHidEvents().ToArray());

	public override Task Back()
		=> Task.CompletedTask;

	public override Task KeyPress(char keyCode)
		=> idb.hid().SendStream<HIDEvent, HIDResponse>(keyCode.AsHidEvents().ToArray());

	public override Task Tap(int x, int y)
		=> press(x, y, TimeSpan.FromMilliseconds(50));

	public override Task Tap(Element element)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, Actions.Tap, element.Id);


	public override Task LongPress(int x, int y)
		=> press(x, y, TimeSpan.FromSeconds(3));

	public override Task LongPress(Element element)
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

	public override Task Swipe((int x, int y) start, (int x, int y) end)
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

	public override Task<string?> GetProperty(string elementId, string propertyName)
			=> grpc.Client.GetProperty(Configuration.AutomationPlatform, elementId, propertyName);

	public override Task<IEnumerable<Element>> GetElements()
		=> grpc.Client.GetElements(Configuration.AutomationPlatform);

	public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(Configuration.AutomationPlatform, action, elementId, arguments);

	public override Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
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

	public override async void Dispose()
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
