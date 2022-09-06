using System.CommandLine;
using Microsoft.DotNet.Interactive;

namespace Microsoft.Maui.Automation.Interactive;

public class AutomationExtensions : IKernelExtension
{
    public Task OnLoadAsync(Kernel kernel)
    {
        var platformOption = new Option<Platform>(
            new[] { "-p", "--platform" },
            "The device platform to test on.")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        var automationPlatformOption = new Option<Platform>(
            new[] { "-a", "--automation-platform" },
            "The automation platform to interact with, if different than the device platform (ie: Maui).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        var deviceOption = new Option<string>(
            new[] { "-d", "--device" },
            getDefaultValue: () => string.Empty,
            description: "Device ID to test on (eg: ADB Serial for an android device, UDID of emulator for iOS/tvOS/ipadOS).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        
        var testOption = new Command("#!uitesting", "Starts a UI Testing session")
        {
            platformOption,
            automationPlatformOption,
            deviceOption
        };

        testOption.SetHandler(
            (Platform platform, Platform automationPlatform, string device) =>
            {
                
                //KernelInvocationContext.Current.Display(SvgClock.DrawSvgClock(hour, minute, second));
            },
            platformOption,
            automationPlatformOption,
            deviceOption);

        kernel.AddDirective(testOption);
    }
}

