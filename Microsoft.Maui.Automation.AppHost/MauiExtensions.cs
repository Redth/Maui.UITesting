using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
			else if (view is IContentView content && content?.Content is Maui.IView contentView)
			{
				return new[] { contentView.GetMauiElement(application, parentId, currentDepth, maxDepth) };
			}

			return Array.Empty<Element>();
		}


		internal static Element GetPlatformElement(this Maui.IWindow window, IApplication application, int currentDepth = -1, int maxDepth = -1)
		{
#if ANDROID
			if (window.Handler.PlatformView is Android.App.Activity activity)
				return activity.GetElement(application, currentDepth, maxDepth);
#elif IOS || MACCATALYST
			if (window.Handler.PlatformView is UIKit.UIWindow uiwindow)
				return uiwindow.GetElement(application, currentDepth, maxDepth);
#elif WINDOWS
            if (window.Handler.PlatformView is Microsoft.UI.Xaml.Window xamlwindow)
				return xamlwindow.GetElement(application, currentDepth, maxDepth);
#endif
			return null;
		}

		internal static Element GetPlatformElement(this Maui.IView view, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
#if ANDROID
			if (view.Handler.PlatformView is Android.Views.View androidview)
				return androidview.GetElement(application, parentId, currentDepth, maxDepth);
#elif IOS || MACCATALYST
			if (view.Handler.PlatformView is UIKit.UIView uiview)
				return uiview.GetElement(application, parentId, currentDepth, maxDepth);
#elif WINDOWS
            if (view.Handler.PlatformView is Microsoft.UI.Xaml.UIElement uielement)
				return uielement.GetElement(application, parentId, currentDepth, maxDepth);
#endif

			return null;
		}


		internal static Element GetMauiElement(this Maui.IWindow window, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var platformElement = window.GetPlatformElement(application);

			var e = new Element(application, Platform.Maui, platformElement.Id, window, parentId)
			{
				Id = platformElement.Id,
				AutomationId = platformElement.AutomationId ?? platformElement.Id,
				Type = window.GetType().Name,

				Width = platformElement.Width,
				Height = platformElement.Height,
				Text = platformElement.Text ?? ""
			};

			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(window.GetChildren(application, e.Id, currentDepth + 1, maxDepth));

			return e;
		}

		internal static Element GetMauiElement(this Maui.IView view, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
		{
			var platformElement = view.GetPlatformElement(application, parentId, currentDepth, maxDepth);

			var e = new Element(application, Platform.Maui, platformElement.Id, view, parentId)
			{
				ParentId = parentId,
				AutomationId = view.AutomationId ?? platformElement.Id,
				Type = view.GetType().Name,
				FullType = view.GetType().FullName,
				Visible = view.Visibility == Visibility.Visible,
				Enabled = view.IsEnabled,
				Focused = view.IsFocused,

				X = (int)view.Frame.X,
				Y = (int)view.Frame.Y,
				Width = (int)view.Frame.Width,
				Height = (int)view.Frame.Height,
			};

			if (view is Microsoft.Maui.IText text && !string.IsNullOrEmpty(text.Text))
				e.Text = text.Text;

			if (view is Microsoft.Maui.ITextInput input && !string.IsNullOrEmpty(input.Text))
				e.Text = input.Text;

			if (view is IImage image && image.Source is not null)
				e.Text = image.Source?.ToString() ?? "";


			if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
				e.Children.AddRange(view.GetChildren(application, parentId, currentDepth + 1, maxDepth));

			return e;
		}
	}
}
