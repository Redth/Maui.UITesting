using System;
using System.Collections.Generic;

namespace Microsoft.Maui.Automation
{
	internal static class MauiExtensions
	{
		internal static Element[] GetChildren(this Maui.IWindow window, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			if (window.Content == null)
				return Array.Empty<Element>();

			return new[] { window.Content.GetMauiElement(application, parentId, currentDepth, maxDepth) };
		}

		internal static Element[] GetChildren(this Maui.IView view, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			if (view is ILayout layout)
			{
				var children = new List<Element>();

				foreach (var v in layout)
				{
					children.Add(v.GetMauiElement(application, parentId, currentDepth, maxDepth));
				}

				return children.ToArray();
			}
			else if (view is Microsoft.Maui.Controls.NavigationPage navigationPage)
			{
                return new[] { navigationPage.CurrentPage.GetMauiElement(application, parentId, currentDepth, maxDepth) };
            }
			else if (view is IContentView content && content?.Content is Maui.IView contentView)
			{
				return new[] { contentView.GetMauiElement(application, parentId, currentDepth, maxDepth) };
			}

			return Array.Empty<Element>();
		}


		internal static Element GetPlatformWindowElement(this Maui.IWindow window, IApplication application)
		{
#if ANDROID
			if (window.Handler.PlatformView is Android.App.Activity activity)
				return activity.GetElement(application, 1, 1);
#elif IOS || MACCATALYST
			if (window.Handler.PlatformView is UIKit.UIWindow uiwindow)
				return uiwindow.GetElement(application, 1, 1);
#elif WINDOWS
			if (window.Handler.PlatformView is Microsoft.UI.Xaml.Window xamlwindow)
				return xamlwindow.GetElement(application, 1, 1);
#endif
			return null;
		}

		internal static Element GetPlatformViewElement(this Maui.IView view, IApplication application, string parentId = "")
		{
#if ANDROID
			if (view.Handler.PlatformView is Android.Views.View androidview)
				return androidview.GetElement(application, parentId, 1, 1);
#elif IOS || MACCATALYST
			if (view.Handler.PlatformView is UIKit.UIView uiview)
				return uiview.GetElement(application, parentId, 1, 1);
#elif WINDOWS
			if (view.Handler.PlatformView is Microsoft.UI.Xaml.FrameworkElement fwelement)
				return fwelement.GetElement(application, parentId, 1, 1);
#endif
			return null;
		}

//		internal static string GetPlatformElementId(this Maui.IView view, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
//		{
//#if ANDROID
//			if (view.Handler.PlatformView is Android.Views.View androidview)
//				return androidview.EnsureUniqueId();
//#elif IOS || MACCATALYST
//			if (view.Handler.PlatformView is UIKit.UIView uiview)
//				return uiview.Handle.ToString();
//#elif WINDOWS
//			if (view.Handler.PlatformView is Microsoft.UI.Xaml.FrameworkElement fwElement)
//				return fwElement.GetAutomationUid();
//#endif

//			return null;
//		}


		internal static Element GetMauiElement(this Maui.IWindow window, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var platformElement = window.GetPlatformWindowElement(application);

			var e = new Element(application, Platform.Maui, platformElement.Id, window, parentId)
			{
				Id = platformElement.Id,
				AutomationId = platformElement.AutomationId ?? platformElement.Id,
				Type = window.GetType().Name,
				WindowFrame = platformElement.WindowFrame,
				ScreenFrame = platformElement.ScreenFrame,
#if NET7_0_OR_GREATER
				ViewFrame = new Frame
				{
					X = (int)window.X,
					Y = (int)window.Y,
					Width = (int)window.Width,
					Height = (int)window.Height,
				},
#else
				ViewFrame = platformElement.ViewFrame,
#endif
				Text = platformElement.Text ?? ""
			};

			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(window.GetChildren(application, e.Id, currentDepth + 1, maxDepth));

			return e;
		}

		internal static Element GetMauiElement(this Maui.IView view, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var platformElement = view.GetPlatformViewElement(application, parentId);

			var e = new Element(application, Platform.Maui, platformElement.Id, view, parentId)
			{
				ParentId = parentId,
				AutomationId = view.AutomationId ?? platformElement.AutomationId,
				Type = view.GetType().Name,
				FullType = view.GetType().FullName,
				Visible = view.Visibility == Visibility.Visible,
				Enabled = view.IsEnabled,
				Focused = view.IsFocused,
				WindowFrame = platformElement.WindowFrame,
				ScreenFrame = platformElement.ScreenFrame,
				ViewFrame = new Frame
				{
					X = (int)view.Frame.X,
					Y = (int)view.Frame.X,
					Width = (int)view.Frame.Width,
					Height = (int)view.Frame.Height,
				}
			};

			if (view is Microsoft.Maui.IText text && !string.IsNullOrEmpty(text.Text))
				e.Text = text.Text;

			if (view is Microsoft.Maui.ITextInput input && !string.IsNullOrEmpty(input.Text))
				e.Text = input.Text;

			if (view is IImage image && image.Source is not null)
				e.Text = image.Source?.ToString() ?? "";


			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(view.GetChildren(application, e.Id, currentDepth + 1, maxDepth));

			return e;
		}
	}
}
