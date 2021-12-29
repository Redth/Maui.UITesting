using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
public class WindowsAppSdkApplication : Application
{
public override Platform DefaultPlatform => Platform.WinAppSdk;

public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
{
    throw new System.NotImplementedException();
}

public override async IAsyncEnumerable<IElement> Children(Platform platform)
{
    yield return new WindowsAppSdkWindow(this, Microsoft.UI.Xaml.Window.Current);
}
}
}