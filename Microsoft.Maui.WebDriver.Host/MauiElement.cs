using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Essentials;

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

		public IEnumerable<IPlatformElement> Children
		{
			get
			{
				if (NativeView is IContentView cv && cv.Content is not null)
					return Enumerable.Repeat(new MauiElement(cv.Content), 1);

				if (NativeView is ILayout layout)
					return layout.Select(v => new MauiElement(v));

				return Enumerable.Empty<IPlatformElement>();
			}
		}

		public string TagName
			=> NativeView.GetType().Name;

		public string Text
		{
			get
			{
				if (NativeView is IText tv)
					return MauiDriver.InvokeOnMainThread (() => tv.Text);

				var possibleResult = MauiDriver.InvokeOnMainThread (() => GetViaProperty<string>(NativeView, "Text"));
				return possibleResult;
			}
		}

		public bool Selected
		{
			get
			{
#if ANDROID
				if (NativeView.Handler.NativeView is Android.Views.View aview)
					return aview.HasFocus;
#elif IOS || __MACCATALYST__
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
			{
				MainThread.BeginInvokeOnMainThread(() => ti.Text = text);
			} else
            {
				MainThread.BeginInvokeOnMainThread (() => SetViaProperty(NativeView, "Text", text));
            }
		}

		public void Submit()
		{
			throw new NotImplementedException();
		}

		public void Click()
		{
			if (NativeView is IButton button)
			{
				MainThread.BeginInvokeOnMainThread(button.Clicked);
			}
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

		bool SetViaProperty<T>(IView view, string propertyName, T newValue)
		{
			// look for a public property named propertyName with type T and with a getter.
			var prop = view.GetType().GetProperty(propertyName, typeof(T));
			if (prop == null || (prop.SetMethod == null || !prop.SetMethod.IsPublic))
				return false;
			try
			{
				prop.SetValue(view, newValue);
			}
			catch
			{
				return false;
			}
			return true;
		}

		T GetViaProperty<T>(IView view, string propertyName)
		{
			// look for a public property named propertyName with type T and with a getter.
			var prop = view.GetType().GetProperty(propertyName, typeof(T));
			if (prop == null || (prop.GetMethod == null || !prop.GetMethod.IsPublic))
				return default(T);
			try
			{
				return (T)prop.GetValue(view);
			}
			catch
			{
			}
			return default(T);
		}
	}
}
