using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Microsoft.Maui;

namespace Microsoft.Maui.WebDriver.Host
{
	public class MauiElement : IPlatformElement
	{
		public MauiElement(IView nativeView)
		{
			NativeView = nativeView;
		}

		public IView NativeView { get; set; }

		public string AutomationId
			=> NativeView.AutomationId;

		public bool Enabled
			=> NativeView.IsEnabled;

		public IPlatformElement[] Children
		{
			get
			{
				if (NativeView is ILayout layout)
					return layout.Select(v => new MauiElement(v)).ToArray();

				return null;
			}
		}

		public string TagName
			=> NativeView.GetType().Name;

		public string Text
		{
			get
			{
				if (NativeView is IText tv)
					return tv.Text;

				return null;
			}
		}

		public bool Selected
		{
			get
            {
#if ANDROID
				if (NativeView.Handler.NativeView is Android.Views.View aview)
					return aview.HasFocus;
#elif IOS
				if (NativeView.Handler.NativeView is UIKit.UIView iosview)
					return iosview.Focused;
#endif

				return false;
			}
		}


		public Point Location
			=> new Point((int)NativeView.Frame.X, (int)NativeView.Frame.Y);

		public Size Size
			=> new Size((int)NativeView.Width, (int)NativeView.Height);

		public bool Displayed
			=> NativeView.Visibility == Visibility.Visible;

		public void Clear()
		{
			if (NativeView is ITextInput ti)
				ti.Text = string.Empty;
		}

		public void SendKeys(string text)
		{
			if (NativeView is ITextInput ti)
				ti.Text = text;
		}

		public void Submit()
		{
			throw new NotImplementedException();
		}

		public void Click()
		{
			if (NativeView is IButton button)
				button.Clicked();
		}

		public string GetAttribute(string attributeName)
			=> GetProperty(attributeName);

		public string GetProperty(string propertyName)
			=> NativeView.GetType().GetProperty(propertyName)?.GetValue(NativeView)?.ToString();

		public string GetCssValue(string propertyName)
			=> GetProperty(propertyName);

		public IWebElement FindElement(By by)
			=> by.FindElement(this);

		public ReadOnlyCollection<IWebElement> FindElements(By by)
			=> by.FindElements(this);
	}
}