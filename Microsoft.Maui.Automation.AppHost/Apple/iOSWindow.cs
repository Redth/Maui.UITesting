#if IOS || MACCATALYST
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UIKit;

namespace Microsoft.Maui.Automation
{
    public class iOSWindow : Window
    {
        public iOSWindow(IApplication application, UIWindow window)
            : base(application, Platform.iOS, window.Handle.ToString())
        {
            PlatformWindow = window;
            PlatformElement = window;
            AutomationId = window.AccessibilityIdentifier ?? Id;

            Children = window.Subviews?.Select(s => new iOSView(application, Id, s))?.ToArray<IView>() ?? Array.Empty<IView>();
            Width = (int)PlatformWindow.Frame.Width;
            Height = (int)PlatformWindow.Frame.Height;
            Text = string.Empty;
        }

        [JsonIgnore]
        public readonly UIWindow PlatformWindow;
    }
}
#endif