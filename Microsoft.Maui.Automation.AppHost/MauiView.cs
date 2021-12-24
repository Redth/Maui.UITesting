using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Microsoft.Maui;
using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public class MauiView : View
	{
		public MauiView(string windowId, Maui.IView view)
			: base(Platform.MAUI, windowId, "")
		{
			PlatformElement = view;
			PlatformView = view.ToAutomationView(windowId) ?? throw new PlatformNotSupportedException();

			WindowId = PlatformView.WindowId;
			Id = PlatformView.Id;
			AutomationId = PlatformView.AutomationId;
			Type = view.GetType().Name;

			Visible = PlatformView.Visible;
			Enabled = PlatformView.Enabled;
			Focused = PlatformView.Focused;

			X = PlatformView.X;
			X = PlatformView.Y;
			Width = PlatformView.Width;
			Height = PlatformView.Height;

			Children = view.GetChildren(windowId);
		}

		[JsonIgnore]
		protected IView PlatformView { get; set; }

  //      public void Clear()
		//{
		//	if (NativeView is ITextInput ti)
		//		ti.Text = string.Empty;
		//}

		//public void SendKeys(string text)
		//{
		//	if (NativeView is ITextInput ti)
		//		ti.Text = text;
		//}

		//public void Return()
		//{
		//	throw new NotImplementedException();
		//}

		//public bool Focus()
		//	=> PlatformView.Focus();

		//public void Click()
		//{
		//	if (NativeView is IButton button)
		//		button.Clicked();
		//}

	}
}
