using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    // All the code in this file is only included on Windows.
    public class WindowsAppSdkWindow : Element
    {
        public WindowsAppSdkWindow(IApplication application, Microsoft.UI.Xaml.Window window)
            : base(application, Platform.WinAppSdk, window.GetHashCode().ToString())
        {
            PlatformWindow = window;

            AutomationId = Id;

            var children = new List<IElement> { new WindowsAppSdkView(application, PlatformWindow.Content, Id) };

            Children = new ReadOnlyCollection<IElement>(children);

            Width = (int)window.Bounds.Width;
            Height = (int)window.Bounds.Height;
            Text = window.Title;
        }

        [JsonIgnore]
        public readonly UI.Xaml.Window PlatformWindow;
    }
}