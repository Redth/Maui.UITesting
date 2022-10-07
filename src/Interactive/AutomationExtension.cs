using System.CommandLine;
using System.CommandLine.Binding;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Interactive;

public class AutomationExtensions : IKernelExtension
{
	

	public Task OnLoadAsync(Kernel kernel)
	{
        if (kernel is CSharpKernel csharpKernel)
        {
            var platformOption = new Option<Platform>(
                new[] {"--platform"},
                "The device platform to test on.")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var appIdOption = new Option<string>(
                new[] {"--app-id"},
                "The application id (bundle id, package id, etc) to test.")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var appOption = new Option<string>(
                new[] {"--app"},
                "The application file (.app, .apk, etc.) to test.")
            {
                Arity = ArgumentArity.ExactlyOne
            };
            var automationPlatformOption = new Option<Platform>(
                new[] {"--automation-platform"},
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
                        break;
                    case Platform.Ios:
                        break;
                    case Platform.Maccatalyst:
                        break;
                    case Platform.Macos:
                        break;
                    case Platform.Tvos:
                        break;
                    case Platform.Android:
                        break;
                    case Platform.Winappsdk:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                // get list of valid devices string and return it
                return devices;
            });


            var nameOption = new Option<string>(
                new[] { "--name" },
                getDefaultValue: () => "device",
                description:
                "Name for the local variable.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };


            var testOption = new Command("#!uitest", "Starts a UI Testing session")
            {
                platformOption,
                automationPlatformOption,
                appIdOption,
                appOption,
                deviceOption,
                nameOption
            };

            testOption.SetHandler(
                async (Platform platform, Platform automationPlatform, string appId, string app, string device, string name) =>
                {
                    // if there is an already used dirver with such name dispose
                    if (csharpKernel.TryGetValue(name, out AppDriver oldDriver) && oldDriver is {})
                    {
                        await DisposeAppDriver(oldDriver);
                    }

                    var driver = new AppDriver(
                        new AutomationConfiguration(
                            appId, app, platform, device, automationPlatform));

                    csharpKernel.RegisterForDisposal(async() =>
                    {
                        await DisposeAppDriver(driver);
                    });

                    await driver.InstallApp();

                    await csharpKernel.SetValueAsync(name, driver);

                    //KernelInvocationContext.Current.Display(SvgClock.DrawSvgClock(hour, minute, second));
                },
                platformOption,
                automationPlatformOption,
                appIdOption,
                appOption,
                deviceOption,
                nameOption);

            csharpKernel.AddDirective(testOption);
        }

        return Task.CompletedTask;

        async Task DisposeAppDriver(AppDriver driver)
        {
            try
            {
                await driver.StopApp();
                await driver.ClearAppState();
                driver.Dispose();
            }
            catch
            {

            }
        }
    }
}

