using System;
using System.Collections.Generic;

namespace Microsoft.Maui.Automation
{
    internal static class MauiExtensions
    {
		internal static IView[] GetChildren(this Maui.IWindow window, string windowId)
		{
			if (window.Content == null)
				return Array.Empty<IView>();

			return new[] { new MauiView(windowId, window.Content) };
		}

		internal static IView[] GetChildren(this Maui.IView view, string windowId)
		{
			if (view is ILayout layout)
            {
				var children = new List<IView>();

				foreach (var v in layout)
                {
					children.Add(new MauiView(windowId, v));
                }

				return children.ToArray();
            }
			else if (view is IContentView content && content?.Content is Maui.IView contentView)
            {
				return new[] { new MauiView(windowId, contentView) };
            }

			return Array.Empty<IView>();
		}


		internal static IWindow ToAutomationWindow(this Maui.IWindow window)
		{
#if ANDROID
			if (window.Handler.NativeView is Android.App.Activity activity)
				return new AndroidWindow(activity);
#elif IOS || MACCATALYST
			if (window.Handler.NativeView is UIKit.UIWindow uiwindow)
				return new iOSWindow(uiwindow);
#elif WINDOWS
			if (window.Handler.NativeView is Microsoft.UI.Xaml.Window xamlwindow)
				return new WindowsAppSdkWindow(xamlwindow);
#endif
			return null;
		}

		internal static IView ToAutomationView(this Maui.IView view, string windowId)
        {
#if ANDROID
			if (view.Handler.NativeView is Android.Views.View androidview)
				return new AndroidView(windowId, androidview);
#elif IOS || MACCATALYST
			if (view.Handler.NativeView is UIKit.UIView uiview)
				return new iOSView(windowId, uiview);
#elif WINDOWS
			if (view.Handler.NativeView is Microsoft.UI.Xaml.UIElement uielement)
				return new WindowsAppSdkView(windowId, uielement);
#endif

			return null;
		}
	}
}
