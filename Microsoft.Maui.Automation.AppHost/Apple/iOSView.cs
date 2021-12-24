#if IOS || MACCATALYST
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UIKit;

namespace Microsoft.Maui.Automation
{
    public class iOSView : View
	{
		public iOSView(string windowId, UIKit.UIView platformView)
			: base(Platform.iOS, windowId, platformView.Handle.ToString())
		{	
			PlatformView = platformView;
			PlatformElement = platformView;

			AutomationId = platformView.AccessibilityIdentifier;
			Children = platformView.Subviews?.Select(s => new iOSView(windowId, s))?.ToArray<IView>() ?? Array.Empty<IView>();

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

		//public void Return()
		//{
		//	throw new NotImplementedException();
		//}

		//public bool Focus()
		//{
		//	if (!PlatformElement.CanBecomeFirstResponder)
		//		return false;

		//	return PlatformElement.BecomeFirstResponder();
		//}

		//public void Click()
		//{
		//	throw new NotImplementedException();
		//}

	}
}
#endif