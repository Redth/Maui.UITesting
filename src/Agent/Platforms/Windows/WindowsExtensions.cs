using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using System;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using static Idb.XctraceRecordRequest.Types;

namespace Microsoft.Maui.Automation;

internal static class WindowsExtensions
{
	public static Element GetElement(this FrameworkElement fwElement, IApplication application, string parentId = "", int currentDepth = -1, int maxDepth = -1)
	{
		var uid = fwElement.EnsureUniqueId();
		var automationId = fwElement.EnsureAutomationId();

		var transform = fwElement.TransformToVisual(fwElement.XamlRoot.Content);
		var position = transform.TransformPoint(new global::Windows.Foundation.Point(0, 0));

		var element = new Element(application, Platform.Winappsdk, uid, fwElement, parentId)
		{
			AutomationId = automationId,
			Visible = fwElement.Visibility == UI.Xaml.Visibility.Visible,
			Enabled = fwElement.IsTapEnabled,
			Focused = fwElement.FocusState != FocusState.Unfocused,
			ViewFrame = new Frame
			{
				X = (int)fwElement.ActualOffset.X,
				Y = (int)fwElement.ActualOffset.Y,
				Width = (int)fwElement.ActualSize.X,
				Height = (int)fwElement.ActualSize.Y
			},
			WindowFrame = new Frame
			{
				X = (int)position.X,
				Y = (int)position.Y,
				Width = (int)fwElement.ActualSize.X,
				Height = (int)fwElement.ActualSize.Y
			},
			ScreenFrame = new Frame {
				X = (int)position.X,
				Y = (int)position.Y,
				Width = (int)fwElement.ActualSize.X,
				Height = (int)fwElement.ActualSize.Y
			}
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
		var uid = window.Content.EnsureUniqueId();
		var automationId = window.Content.EnsureAutomationId();

		var pos = window?.GetAppWindow()?.Position;
		var size = window?.GetAppWindow()?.Size;
		var x = pos?.X ?? -1;
		var y = pos?.Y ?? -1;
		var w = size?.Width ?? -1;
		var h = size?.Height ?? -1;

		var element = new Element(application, Platform.Winappsdk, uid, window)
		{
			AutomationId = automationId,
			ViewFrame = new Frame
			{
				X = x,
				Y = y,
				Width = w,
				Height = h
			},
			WindowFrame = new Frame
			{
				X = x,
				Y = y,
				Width = w,
				Height = h
			},
			ScreenFrame = new Frame
			{
				X = x,
				Y = y,
				Width = w,
				Height = h
			},
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


	internal static string EnsureAutomationId(this UIElement uielement)
	{
		var automationPeer = UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer.FromElement(uielement);

		var existingAutomationId = automationPeer?.GetAutomationId();

		if (string.IsNullOrEmpty(existingAutomationId))
		{
			existingAutomationId = uielement.GetValue(AutomationProperties.AutomationIdProperty)?.ToString();

			if (string.IsNullOrEmpty(existingAutomationId))
			{
				existingAutomationId = Guid.NewGuid().ToString();
				uielement.SetValue(AutomationProperties.AutomationIdProperty, existingAutomationId);
			}
		}

		return existingAutomationId ?? string.Empty;
	}

	internal static string EnsureUniqueId(this UIElement uielement)
	{
		var id = uielement.GetAutomationUid();

		if (string.IsNullOrEmpty(id))
		{
			id = Guid.NewGuid().ToString();
			uielement.SetAutomationUid(id);
		}

		return id;
	}

	public static Task<PerformActionResult> PerformAction(this UIElement uielement, string action, params string[] arguments)
	{
		if (action == Actions.Tap)
		{
		}
		return Task.FromResult(PerformActionResult.Error());
	}
}
