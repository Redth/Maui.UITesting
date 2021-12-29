#if IOS || MACCATALYST
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using UIKit;

namespace Microsoft.Maui.Automation
{
	public class iOSView : Element
	{
		public iOSView(IApplication application, UIKit.UIView platformView, string? parentId = null)
			: base(application, Platform.iOS, platformView.Handle.ToString(), parentId)
		{	
			PlatformView = platformView;
			PlatformElement = platformView;

			AutomationId = platformView.AccessibilityIdentifier;


			var children = platformView.Subviews?.Select(s => new iOSView(application, s, Id))?.ToList<IElement>() ?? new List<IElement>();
			Children = new ReadOnlyCollection<IElement>(children);

			Visible = !platformView.Hidden;
			Enabled = platformView.UserInteractionEnabled;
			Focused = platformView.Focused;

			X = (int)platformView.Frame.X;
			Y = (int)platformView.Frame.Y;

			Width = (int)platformView.Frame.Width;
			Height = (int)platformView.Frame.Height;

			Text = platformView.GetText();
		}

		[JsonIgnore]
		public UIKit.UIView PlatformView { get; set; }

		//public void Clear()
		//{
		//	if (PlatformElement is UIKit.IUITextInput ti)
		//	{
		//		var start = ti.BeginningOfDocument;
		//		var end = ti.EndOfDocument;
		//		var range = ti.GetTextRange(start, end);
		//		ti.ReplaceText(range, string.Empty);
		//	}
		//}

		//public void SendKeys(string text)
		//{
		//	if (PlatformElement is UIKit.IUITextInput ti)
		//	{
		//		var start = ti.BeginningOfDocument;
		//		var end = ti.EndOfDocument;
		//		var range = ti.GetTextRange(start, end);
		//		ti.ReplaceText(range, text);
		//	}
		//}

		//public bool Focus()
		//{
		//	if (!PlatformElement.CanBecomeFirstResponder)
		//		return false;

		//	return PlatformElement.BecomeFirstResponder();
		//}

		
	}
}
#endif