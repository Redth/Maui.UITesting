using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using Idb;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui.Automation.Driver;

public class iOSDriver : Driver
{
	public static class ConfigurationKeys
	{
		public const string IdbCompanionAddress = "iOS:IdbCompanionAddress";
		public const string IdbCompanionPort = "iOS:IdbCompanionPort";
		public const string AutoStartIdbCompanion = "iOS:AutoStartIdbCompanion";
	}
	public iOSDriver(IAutomationConfiguration configuration, ILoggerFactory? loggerProvider)
		: base(configuration, loggerProvider)
	{
		idbLogger = LoggerFactory.CreateLogger("idb");

		var idbPort = configuration.Get(ConfigurationKeys.IdbCompanionPort, 10882);
		var idbAddress = configuration.Get(ConfigurationKeys.IdbCompanionAddress, "127.0.0.1");
		var idbStart = configuration.Get(ConfigurationKeys.AutoStartIdbCompanion, true);

		ArgumentNullException.ThrowIfNull(configuration.Device);

		Name = $"iOS ({configuration.Device})";

		if (string.IsNullOrEmpty(configuration.AppId))
			configuration.AppId = AppUtil.GetBundleIdentifier(configuration.AppFilename)
				?? throw new Exception("AppId not found");

		if (idbStart)
		{
			idbCompanionPath = UnpackIdb();

			idbCompanionProcess = new ProcessRunner(idbLogger, idbCompanionPath, $"--boot {configuration.Device}");
			var bootResult = idbCompanionProcess.WaitForExit();

			idbCompanionProcess = new ProcessRunner(idbLogger, idbCompanionPath, $"--udid {configuration.Device}");

			// Sleep until idb exited or started
			while (true)
			{
				Thread.Sleep(500);

				if (idbCompanionProcess.HasExited)
					throw new Exception("Failed to start idb");

				var output = idbCompanionProcess.Output.ToList();

				if (output.Any(s => s?.Contains($"\"grpc_swift_port\":{idbPort}") ?? false))
					break;
			}
		}

		var channel = GrpcChannel.ForAddress($"http://{idbAddress}:{idbPort}");
		idb = new Idb.CompanionService.CompanionServiceClient(channel);

		var connectResponse = idb.connect(new Idb.ConnectRequest());
		Udid = connectResponse.Companion.Udid;

		Name = $"iOS ({configuration.Device} [{Udid}])";

		grpc = new GrpcHost(configuration, LoggerFactory);
	}

	public readonly string Udid;

	readonly CompanionService.CompanionServiceClient idb;
	readonly GrpcHost grpc;
	readonly string idbCompanionPath;
	readonly ProcessRunner idbCompanionProcess;

	readonly ILogger idbLogger;

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

	public override async Task StopApp()
	{
		var apps = await idb.list_appsAsync(new ListAppsRequest
		{
			SuppressProcessState = false
		}).ResponseAsync;

		var thisApp = apps.Apps.FirstOrDefault(a => a.BundleId.Equals(Configuration.AppId, StringComparison.OrdinalIgnoreCase));
		if (thisApp is not null)
		{
			if (thisApp.ProcessState == InstalledAppInfo.Types.AppProcessState.Running)
			{
				await idb.terminateAsync(new TerminateRequest
				{
					BundleId = Configuration.AppId
				}).ResponseAsync;
			}
		}
	}

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


	public override async Task InputText(IElement element, string text)
	{
		await Tap(element);
		await Task.Delay(250);
		var textEvents = text.AsHidEvents().ToArray();

		await idb.hid().SendStream<HIDEvent, HIDResponse>(TimeSpan.FromMilliseconds(100), textEvents);
	}

	public override async Task ClearText(IElement element)
	{
		await Tap(element);
		
		await Task.Delay(500);

		iOSGrpcTextExtensions.Backspace(element.Text.Length);
	}

	public override Task Back()
		=> Task.CompletedTask;

	public override Task KeyPress(char keyCode)
		=> idb.hid().SendStream<HIDEvent, HIDResponse>(keyCode.AsHidEvents().ToArray());

	public override Task Tap(int x, int y)
		=> press(x, y, TimeSpan.FromMilliseconds(100));

	public override Task Tap(IElement element)
		=> Tap(
			(int)((element.WindowFrame.X + (element.WindowFrame.Width / 2))),
			(int)((element.WindowFrame.Y + (element.WindowFrame.Height / 2))));

	public override Task LongPress(int x, int y)
		=> press(x, y, TimeSpan.FromSeconds(3));

	public override Task LongPress(IElement element)
			=> press(
				(int)((element.WindowFrame.X + (element.WindowFrame.Width / 2))),
				(int)((element.WindowFrame.Y + (element.WindowFrame.Height / 2))), TimeSpan.FromSeconds(3));

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
						Direction = HIDEvent.Types.HIDDirection.Up
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

	public override Task<string?> GetProperty(Platform automationPlatform, string elementId, string propertyName)
			=> grpc.Client.GetProperty(automationPlatform, elementId, propertyName);

	public override Task<IEnumerable<IElement>> GetElements(Platform automationPlatform)
		=> grpc.Client.GetElements(automationPlatform);

	public override Task<PerformActionResult> PerformAction(Platform automationPlatform, string action, string elementId, params string[] arguments)
		=> grpc.Client.PerformAction(automationPlatform, action, elementId, arguments);

	public override Task<string[]> Backdoor(Platform automationPlatform, string fullyQualifiedTypeName, string staticMethodName, string[] args)
		=> grpc.Client.Backdoor(automationPlatform, fullyQualifiedTypeName, staticMethodName, args);

	public override Task Screenshot(string? filename = null)
	{
		var fullFilename = base.GetScreenshotFilename(filename);
		
		var response = idb.screenshot(new ScreenshotRequest());
		var data = response.ImageData.ToByteArray();
		using var stream = File.Create(fullFilename);
		response.ImageData.WriteTo(stream);
		return Task.CompletedTask;
	}

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
