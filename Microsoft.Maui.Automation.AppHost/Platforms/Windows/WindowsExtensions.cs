using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Maui.Automation;

internal static class WindowsExtensions
{
	public static Element GetElement(this UIElement uiElement, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
	{
		var element = new Element(application, Platform.Winappsdk, uiElement.GetHashCode().ToString(), uiElement, parentId)
		{
			AutomationId = uiElement.GetType().Name,
			Visible = uiElement.Visibility == UI.Xaml.Visibility.Visible,
			Enabled = uiElement.IsTapEnabled,
			Focused = uiElement.FocusState != FocusState.Unfocused,
			X = (int)uiElement.ActualOffset.X,
			Y = (int)uiElement.ActualOffset.Y,
			Width = (int)uiElement.ActualSize.X,
			Height = (int)uiElement.ActualSize.Y
		};

		if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
		{
			foreach (var child in (uiElement as Panel)?.Children ?? Enumerable.Empty<UIElement>())
			{
				var c = child.GetElement(application, element.Id, currentDepth + 1, maxDepth);
					element.Children.Add(c);
			}
		}

		return element;
	}

	public static Element GetElement(this Microsoft.UI.Xaml.Window window, IApplication application, int currentDepth = -1, int maxDepth = -1)
	{
		var element = new Element(application, Platform.Winappsdk, window.GetHashCode().ToString(), window)
		{
			PlatformElement = window,
			AutomationId = window.GetType().Name,
			X = (int)window.Bounds.X,
			Y = (int)window.Bounds.Y,
			Width = (int)window.Bounds.Width,
			Height = (int)window.Bounds.Height,
			Text = window.Title

		};

		if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
		{
			var c = window.Content.GetElement(application, element.Id, currentDepth + 1, maxDepth);
			element.Children.Add(c);
		}

		return element;
	}
}
