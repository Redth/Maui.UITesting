#if IOS || MACCATALYST
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.GTKSpecific;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UIKit;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Maui.Automation;

internal static class iOSExtensions
{
	static string[] possibleTextPropertyNames = new string[]
	{
		"Title", "Text",
	};

	internal static string GetText(this UIView view)
		=> view switch
		{
			IUITextInput ti => TextFromUIInput(ti),
			UIButton b => b.CurrentTitle,
			_ => TextViaReflection(view, possibleTextPropertyNames)
		};

	static string TextViaReflection(UIView view, string[] propertyNames)
	{
		foreach (var name in propertyNames)
		{
			var prop = view.GetType().GetProperty("Text", typeof(string));
			if (prop is null)
				continue;
			if (!prop.CanRead)
				continue;
			if (prop.PropertyType != typeof(string))
				continue;
			return prop.GetValue(view) as string ?? "";
		}
		return "";
	}

	static string TextFromUIInput(IUITextInput ti)
	{
		var start = ti.BeginningOfDocument;
		var end = ti.EndOfDocument;
		var range = ti.GetTextRange(start, end);
		return ti.TextInRange(range);
	}

	public static Element GetElement(this UIKit.UIView uiView, IApplication application, string parentId = "")
	{
		var e = new Element(application, Platform.Ios, uiView.Handle.ToString(), uiView, parentId)
		{
			AutomationId = uiView.AccessibilityIdentifier,
			Visible = !uiView.Hidden,
			Enabled = uiView.UserInteractionEnabled,
			Focused = uiView.Focused,

			X = (int)uiView.Frame.X,
			Y = (int)uiView.Frame.Y,

			Width = (int)uiView.Frame.Width,
			Height = (int)uiView.Frame.Height,
			Text = uiView.GetText()
		};

		var children = uiView.Subviews?.Select(s => s.GetElement(application, e.Id))?.ToList<Element>() ?? new List<Element>();

		e.Children.AddRange(children);
		return e;
	}

	public static Element GetElement(this UIWindow window, IApplication application)
	{
		var e = new Element(application, Platform.Ios, window.Handle.ToString(), window)
		{
			AutomationId = window.AccessibilityIdentifier ?? window.Handle.ToString(),
			Width = (int)window.Frame.Width,
			Height = (int)window.Frame.Height,
			Text = string.Empty
		};

		var children = window.Subviews?.Select(s => s.GetElement(application, e.Id))?.ToList<Element>() ?? new List<Element>();

		e.Children.AddRange(children);
		return e;
	}
}
#endif
