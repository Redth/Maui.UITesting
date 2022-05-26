using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Microsoft.Maui.Automation
{
    public class WindowsAppSdkView : Element
    {
        public WindowsAppSdkView(IApplication application, UIElement platformView, string? parentId = null)
            : base(application, Platform.WinAppSdk, platformView.GetHashCode().ToString(), parentId)
        {
            PlatformView = platformView;
            PlatformElement = platformView;

            AutomationId = platformView.GetType().Name;

            var children = (platformView as Panel)?.Children?.Select(c => new WindowsAppSdkView(application, c, Id))?.ToList<IElement>() ?? new List<IElement>();
            Children = new ReadOnlyCollection<IElement>(children);

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