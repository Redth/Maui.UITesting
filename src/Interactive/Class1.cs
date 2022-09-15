using System.CommandLine;
using System.CommandLine.Binding;
using Microsoft.DotNet.Interactive;

namespace Microsoft.Maui.Automation.Interactive;

public class AutomationExtensions : IKernelExtension
{
    Driver.AppDriver? driver;

    public Task OnLoadAsync(Kernel kernel)
    {

        var platformOption = new Option<Platform>(
            new[] { "--platform" },
            "The device platform to test on.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var appIdOption = new Option<string>(
            new[] { "--app-id" },
            "The application id (bundle id, package id, etc) to test.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var appOption = new Option<string>(
            new[] { "--app" },
            "The application file (.app, .apk, etc.) to test.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var automationPlatformOption = new Option<Platform>(
            new[] { "--automation-platform" },
            "The automation platform to interact with, if different than the device platform (ie: Maui).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        var deviceOption = new Option<string>(
            new[] { "--device" },
            getDefaultValue: () => string.Empty,
            description: "Device ID to test on (eg: ADB Serial for an android device, UDID of emulator for iOS/tvOS/ipadOS).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        
        var testOption = new Command("#!uitest", "Starts a UI Testing session")
        {
            platformOption,
            automationPlatformOption,
            appIdOption,
            appOption,
            deviceOption
        };

        testOption.SetHandler(
            async (Platform platform, Platform automationPlatform, string appId, string app, string device) =>
            {
                
                if (driver is not null)
                {
                    await driver.StopApp();
                    await driver.ClearAppState();
                    driver.Dispose();
                    driver = null;
                }

                driver = new Driver.AppDriver(
                    new Driver.AutomationConfiguration(
                        appId, app, platform, device, automationPlatform));

                await driver.InstallApp();

                //KernelInvocationContext.Current.Display(SvgClock.DrawSvgClock(hour, minute, second));
            },
            platformOption,
            automationPlatformOption,
            appIdOption,
            appOption,
            deviceOption);

        kernel.AddDirective(testOption);

        return Task.CompletedTask;
    }
}

