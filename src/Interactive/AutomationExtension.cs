using System.CommandLine;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Maui.Automation.Driver;
using AndroidSdk;
using Microsoft.Maui.Automation.Util;
using Microsoft.DotNet.Interactive.Formatting;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

namespace Microsoft.Maui.Automation.Interactive;

public class AutomationExtensions : IKernelExtension
{
    public Task OnLoadAsync(Kernel kernel)
    {
        var candidateKernels = kernel.FindKernels(k => k is CSharpKernel).Cast<CSharpKernel>().ToList();
        foreach (var csharpKernel in candidateKernels)
        {
            if (!csharpKernel.Directives.Any(d => d.Name == "#!uitest"))
            {
                ConfigureKernelDirective(csharpKernel);
            }
        }
        if (KernelInvocationContext.Current is { } context)
        {
            PocketView view = div(
                code("Maui.UITesting"),
                " is loaded. It adds commands for UI Testing/Automation.",
                code("#!uitest -h")
            );

            context.Display(view);
        }


        return Task.CompletedTask;
    }

    private void ConfigureKernelDirective(CSharpKernel csharpKernel)
    {
        var platformOption = new Option<Platform>(
            new[] {"--platform"},
            () => Platform.Maui,
            "The device platform to test on.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        platformOption.AddCompletions("Android", "iOS", "MacCatalsyt", "tvOS", "WinAppSDK");

        var appIdOption = new Option<string>(
            new[] {"--app-id"},
            "The application id (bundle id, package id, etc) to test.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var appOption = new Option<FileInfo>(
            new[] {"--app"},
            "The application file (.app, .apk, etc.) to test.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var automationPlatformOption = new Option<Platform>(
            new[] {"--automation-platform"},
            () => Platform.Maui,
            "The automation platform to interact with, if different than the device platform (ie: Maui).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        var deviceOption = new Option<string>(
            new[] {"--device"},
            getDefaultValue: () => string.Empty,
            description:
            "Device ID to test on (eg: ADB Serial for an android device, UDID of emulator for iOS/tvOS/ipadOS).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        deviceOption.AddCompletions((context) =>
        {
            var platform = context.ParseResult.GetValueForOption(automationPlatformOption);
            var devices = new List<string>();
            switch (platform)
            {
                case Platform.Maui:
                    AddAndroidDevices(devices);
                    AddAppleDevices("ios", devices);
                    AddAppleDevices("macos", devices);
                    AddAppleDevices("tvos", devices);
                    devices.Add("Windows");
                    break;
                case Platform.Ios:
                    AddAppleDevices("ios", devices);
                    break;
                case Platform.Maccatalyst:
                case Platform.Macos:
                    AddAppleDevices("macos", devices);
                    break;
                case Platform.Tvos:
                    AddAppleDevices("tvos", devices);
                    break;
                case Platform.Android:
                    AddAndroidDevices(devices);
                    break;
                case Platform.Winappsdk:
                    devices.Add("Windows");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // get list of valid devices string and return it
            return devices;
        });


        var nameOption = new Option<string>(
            new[] {"--name"},
            getDefaultValue: () => "driver",
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

        testCommand.SetHandler<Platform, Platform, string, FileInfo, string, string>(
            async (Platform platform, Platform automationPlatform, string appId, FileInfo app, string device,
                string name) =>
            {
                // if there is an already used dirver with such name dispose
                if (csharpKernel.TryGetValue(name, out AppDriver oldDriver) && oldDriver is { })
                {
                    await DisposeAppDriver(oldDriver);
                }

                var driver = new AppDriver(
                    new AutomationConfiguration(
                        appId, app, platform, device, automationPlatform));

                csharpKernel.RegisterForDisposal(async () => { await DisposeAppDriver(driver); });

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

    Task DisposeAppDriver(AppDriver driver)
    {
        try
        {
            driver.Dispose();
        }
        catch
        { }

        return Task.CompletedTask;
    }

    void AddAndroidDevices(List<string> devices)
    {
        var adb = new Adb();
        var adbDevices = adb.GetDevices();
        devices.AddRange(adbDevices.Select(d => d.Serial));

        var avd = new AvdManager();
        var avds = avd.ListAvds();
        devices.AddRange(avds.Select(a => a.Name));
    }

    void AddAppleDevices(string platform, List<string> devices)
    {
        var appleDevices = Xcode.GetDevices(platform).Select(d => d.Name);
        if (appleDevices.Any())
            devices.AddRange(appleDevices);
    }
}

