using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Maui;

namespace Microsoft.Maui.Automation
{
    public class MauiElement : Element
	{
		public MauiElement(IApplication application, Maui.IView view, string? parentId = null)
			: base(application, Platform.MAUI, string.Empty, parentId)
		{
			PlatformElement = view;
			PlatformView = view.ToAutomationView(application, parentId) ?? throw new PlatformNotSupportedException();

			ParentId = parentId;
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

			if (view is Microsoft.Maui.IText text)
				Text = text.Text;

			if (view is Microsoft.Maui.ITextInput input)
				Text = input.Text;

			if (view is IImage image)
				Text = image.Source?.ToString();

			Children = view.GetChildren(application, parentId);
		}

		[Newtonsoft.Json.JsonIgnore]
		[JsonIgnore]
		protected IElement PlatformView { get; set; }

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

		//public bool Focus()
		//	=> PlatformView.Focus();

		//public void Click()
		//{
		//	if (NativeView is IButton button)
		//		button.Clicked();
		//}

	}
}
