using Android.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Maui.Automation
{
    public class AndroidView : View
	{
		public AndroidView(string windowId, Android.Views.View platformView)
			: base(Platform.Android, windowId, platformView.Id.ToString())
		{
			AutomationId = platformView.GetAutomationId();
			Children = platformView.GetChildren(windowId);

			Visible = platformView.Visibility == ViewStates.Visible;
			Enabled = platformView.Enabled;
			Focused = platformView.Selected;

			var loc = new int[2];
			platformView?.GetLocationInWindow(loc);

			if (loc != null && loc.Length >= 2)
            {
				X = loc[0];
				Y = loc[1];
			}

			Width = platformView.MeasuredWidth;
			Height = platformView.MeasuredHeight;

			Text = platformView.GetText();
		}

		[JsonIgnore]
		protected Android.Views.View PlatformView { get; set; }

		Point Location
		{
			get
			{
				var loc = new int[2];
				PlatformView.GetLocationInWindow(loc);

				if (loc != null && loc.Length >= 2)
					return new Point(loc[0], loc[1]);

				return Point.Empty;
			}
		}


		//public void Clear()
		//{
		//	if (NativeView is Android.Widget.TextView tv)
		//		tv.Text = String.Empty;
		//}

		//public void SendKeys(string text)
		//{
		//	if (NativeView is Android.Widget.TextView tv)
		//		tv.Text = text;
		//}

		//public void Return()
		//{
		//	throw new NotImplementedException();
		//}

		//public bool Focus()
		//{
		//	return NativeView.RequestFocus();
		//}

		//public void Click()
		//{
		//	NativeView.CallOnClick();
		//}

		//public string GetProperty(string propertyName)
		//{
		//	// TODO:
		//	return null;
		//}
	}
}
