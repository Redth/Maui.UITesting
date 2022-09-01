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
	public static Element GetElement(this UIElement uiElement, IApplication application, string parentId = "")
	{
		var e = new Element(application, Platform.Winappsdk, uiElement.GetHashCode().ToString(), uiElement, parentId)
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

		var children = (uiElement as Panel)?.Children?.Select(c => c.GetElement(application, e.Id))?.ToList() ?? new List<Element>();
		e.Children.AddRange(children);

		return e;
	}

	public static Element GetElement(this Microsoft.UI.Xaml.Window window, IApplication application, string parentId = "")
	{
		var e = new Element(application, Platform.Winappsdk, window.GetHashCode().ToString(), window, parentId)
		{
			PlatformElement = window,
			AutomationId = window.GetType().Name,
			X = (int)window.Bounds.X,
			Y = (int)window.Bounds.Y,
			Width = (int)window.Bounds.Width,
			Height = (int)window.Bounds.Height,
			Text = window.Title

		};

		e.Children.Add(window.Content.GetElement(application, e.Id));

		return e;
	}
}
