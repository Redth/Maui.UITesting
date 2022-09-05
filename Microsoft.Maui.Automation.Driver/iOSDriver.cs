using AndroidSdk;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.Remote;
using Spectre.Console.Cli;
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

        ArgumentNullException.ThrowIfNull(configuration.Device);

        Name = $"iOS ({configuration.Device})";

        var channel = GrpcChannel.ForAddress($"http://{address}:{port}");
        idb = new Idb.CompanionService.CompanionServiceClient(channel);
        grpc = new GrpcRemoteAppClient();

        var connectResponse = idb.connect(new Idb.ConnectRequest());
        Udid = connectResponse.Companion.Udid;

        Name = $"iOS ({configuration.Device} [{Udid}])";
    }

    public readonly string Udid;

    readonly CompanionService.CompanionServiceClient idb;
    readonly GrpcRemoteAppClient grpc;

    public string Name { get; }

    public IAutomationConfiguration Configuration { get; }

    public async Task ClearAppState(string appId)
    {
        var req = new RmRequest
        {
            Container = new FileContainer
            {
                Kind = FileContainer.Types.Kind.Application,
                BundleId = appId
            }
        };
        req.Paths.Add("/");

        await idb.rmAsync(req);

        await idb.mkdirAsync(new MkdirRequest
        {
            Container = new FileContainer
            {
                Kind = FileContainer.Types.Kind.Application,
                BundleId = appId
            },
            Path = "tmp"
        });
    }

    public Task InstallApp(string file, string appId)
        => idb.install().RequestStream<InstallRequest, InstallResponse>(
            new Idb.InstallRequest()
            {
                Destination = Idb.InstallRequest.Types.Destination.App,

            }, response =>
            {
                var progress = response.Progress;
                Console.WriteLine(progress);
            });

    public Task RemoveApp(string appId)
        => idb.uninstallAsync(new UninstallRequest
        {
            BundleId = appId
        }).ResponseAsync;

    public async Task<IDeviceInfo> GetDeviceInfo()
    {
        var desc = await idb.describeAsync(new Idb.TargetDescriptionRequest { FetchDiagnostics = true });

        var width = desc.TargetDescription.ScreenDimensions.Width;
        var height = desc.TargetDescription.ScreenDimensions.Height;
        var density = desc.TargetDescription.ScreenDimensions.Density;

        return new DeviceInfo(width, height, density);
    }

    

    public Task LaunchApp(string appId)
        => idb.launch().RequestStream<LaunchRequest, LaunchResponse>(
            new LaunchRequest
            {
                Start = new LaunchRequest.Types.Start
                {
                    BundleId = appId,
                    ForegroundIfRunning = true,
                }
            });

    public Task StopApp(string appId)
        => idb.terminateAsync(new TerminateRequest
        {
            BundleId = appId
        }).ResponseAsync;

    public Task OpenUri(string uri)
        => idb.open_urlAsync(new OpenUrlRequest
        {
            Url = uri
        }).ResponseAsync;


    public Task PushFile(string appId, string localFile, string destinationDirectory)
        => idb.push().SendStream(new PushRequest
        {
            Inner = new PushRequest.Types.Inner
            {
                Container = new FileContainer
                {
                    BundleId = appId,
                    Kind = FileContainer.Types.Kind.Application
                },
                DstPath = destinationDirectory
            }
        });

    public Task PullFile(string appId, string remoteFile, string localDirectory)
        => idb.pull(new PullRequest
        {
            Container = new FileContainer
            {
                BundleId = appId,
                Kind = FileContainer.Types.Kind.Application
            },
            DstPath = localDirectory,
            SrcPath = remoteFile
        }).ReceiveStream<PullResponse>(r =>
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

    public Task LongPress(int x, int y)
        => press(x, y, TimeSpan.FromSeconds(3));

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

    public Task<string> GetProperty(Platform platform, string elementId, string propertyName)
        => grpc.GetProperty(platform, elementId, propertyName);

    public Task<IEnumerable<Element>> GetElements(Platform platform)
        => grpc.GetElements(platform);

    public Task<IEnumerable<Element>> FindElements(Platform platform, string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
        => grpc.FindElements(platform, propertyName, pattern, isExpression, ancestorId);
}
