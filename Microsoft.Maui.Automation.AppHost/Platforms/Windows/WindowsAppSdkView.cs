using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkView : View
    {
        public WindowsAppSdkView(IApplication application, string windowId, UIElement platformView)
            : base(application, Platform.WinAppSdk, windowId, platformView.GetHashCode().ToString())
        {
            PlatformView = platformView;
            PlatformElement = platformView;

            AutomationId = platformView.GetType().Name;
            Children = (platformView as Panel)?.Children?.Select(c => new WindowsAppSdkView(application, windowId, c))?.ToArray<IView>() ?? Array.Empty<IView>();

            Visible = PlatformView.Visibility == UI.Xaml.Visibility.Visible;
            Enabled = PlatformView.IsTapEnabled;
            Focused = PlatformView.FocusState != FocusState.Unfocused;
            X = (int)PlatformView.ActualOffset.X;
            Y = (int)PlatformView.ActualOffset.Y;
            Width = (int)PlatformView.ActualSize.X;
            Height = (int)PlatformView.ActualSize.Y;
        }

        [JsonIgnore]
        protected UIElement PlatformView { get; set; }
    }
}