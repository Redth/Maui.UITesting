using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    // All the code in this file is only included on Windows.
    public class WindowsAppSdkWindow : Window
    {
        public WindowsAppSdkWindow(IApplication application, Microsoft.UI.Xaml.Window window)
            : base(application, Platform.WinAppSdk, window.GetHashCode().ToString())
        {
            PlatformWindow = window;

            AutomationId = Id;
            Children = new [] { new WindowsAppSdkView(application, Id, PlatformWindow.Content) };
            Width = (int)window.Bounds.Width;
            Height = (int)window.Bounds.Height;
            Text = window.Title;
        }

        [JsonIgnore]
        public readonly UI.Xaml.Window PlatformWindow;
    }
}