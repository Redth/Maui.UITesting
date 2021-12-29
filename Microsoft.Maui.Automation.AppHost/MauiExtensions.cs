using System;
using System.Collections.Generic;

namespace Microsoft.Maui.Automation
{
    internal static class MauiExtensions
    {
		internal static IElement[] GetChildren(this Maui.IWindow window, IApplication application, string? parentId = null)
		{
			if (window.Content == null)
				return Array.Empty<IElement>();

			return new[] { new MauiElement(application, window.Content, parentId) };
		}

		internal static IElement[] GetChildren(this Maui.IView view, IApplication application, string? parentId = null)
		{
			if (view is ILayout layout)
            {
				var children = new List<IElement>();

				foreach (var v in layout)
                {
					children.Add(new MauiElement(application, v, parentId));
                }

				return children.ToArray();
            }
			else if (view is IContentView content && content?.Content is Maui.IView contentView)
            {
				return new[] { new MauiElement(application, contentView, parentId) };
            }

			return Array.Empty<IElement>();
		}


		internal static IElement ToAutomationWindow(this Maui.IWindow window, IApplication application)
		{
#if ANDROID
			if (window.Handler.NativeView is Android.App.Activity activity)
				return new AndroidWindow(application, activity);
#elif IOS || MACCATALYST
			if (window.Handler.NativeView is UIKit.UIWindow uiwindow)
				return new iOSWindow(application, uiwindow);
#elif WINDOWS
			if (window.Handler.NativeView is Microsoft.UI.Xaml.Window xamlwindow)
				return new WindowsAppSdkWindow(application, xamlwindow);
#endif
			return null;
		}

		internal static IElement ToAutomationView(this Maui.IView view, IApplication application, string? parentId = null)
        {
#if ANDROID
			if (view.Handler.NativeView is Android.Views.View androidview)
				return new AndroidView(application, androidview, parentId);
#elif IOS || MACCATALYST
			if (view.Handler.NativeView is UIKit.UIView uiview)
				return new iOSView(application, uiview, parentId);
#elif WINDOWS
			if (view.Handler.NativeView is Microsoft.UI.Xaml.UIElement uielement)
				return new WindowsAppSdkView(application, uielement, parentId);
#endif

			return null;
		}
	}
}
