using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Maui.Automation;

internal static class WindowsExtensions
{
	public static Element GetElement(this FrameworkElement fwElement, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
	{
		var transform = fwElement.TransformToVisual(fwElement.XamlRoot.Content);
		var position = transform.TransformPoint(new global::Windows.Foundation.Point(0, 0));

		var element = new Element(application, Platform.Winappsdk, fwElement.GetHashCode().ToString(), fwElement, parentId)
		{
			AutomationId = fwElement.GetType().Name,
			Visible = fwElement.Visibility == UI.Xaml.Visibility.Visible,
			Enabled = fwElement.IsTapEnabled,
			Focused = fwElement.FocusState != FocusState.Unfocused,
			X = (int)position.X,
			Y = (int)position.Y,
			Width = (int)fwElement.ActualSize.X,
			Height = (int)fwElement.ActualSize.Y
		};

		if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
		{
			var children = fwElement.FindChildren(false);

			foreach (var child in children)
			{
				var c = child.GetElement(application, element.Id, currentDepth + 1, maxDepth);
				element.Children.Add(c);
			}
		}

		return element;
	}

	public static Element GetElement(this Microsoft.UI.Xaml.Window window, IApplication application, int currentDepth = -1, int maxDepth = -1)
	{
		var pos = window?.GetAppWindow()?.Position;
		var size = window?.GetAppWindow()?.Size;
		var x = pos?.X ?? -1;
		var y = pos?.Y ?? -1;
		var w = size?.Width ?? -1;
		var h = size?.Height ?? -1;

		var element = new Element(application, Platform.Winappsdk, window.GetHashCode().ToString(), window)
		{
			AutomationId = window.GetType().Name,
			X = x,
			Y = y,
			Width = w,
			Height = h,
			Text = window.Title ?? string.Empty
		};

		if (maxDepth <= 0 || (currentDepth + 1 <= maxDepth))
		{
			if (window.Content is FrameworkElement fwContent)
			{
				var c = fwContent.GetElement(application, element.Id, currentDepth + 1, maxDepth);
				element.Children.Add(c);
			}
		}

		return element;
	}

	

	public static Task<PerformActionResult> PerformAction(this UIElement uielement, string action, params string[] arguments)
	{
		if (action == Actions.Tap)
		{
		}
		return Task.FromResult(PerformActionResult.Error());
	}
}
