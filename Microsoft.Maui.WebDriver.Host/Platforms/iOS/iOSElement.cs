using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
	public class iOSElement : IPlatformElement
	{
		public iOSElement(UIKit.UIView nativeView)
		{
			NativeView = nativeView;
		}

		public UIKit.UIView NativeView { get; set; }

		public string AutomationId
			=> NativeView.AccessibilityIdentifier;

		public bool Enabled
			=> false;

		public IEnumerable<IPlatformElement> Children
			=> NativeView.Subviews.Select(s => new iOSElement(s));

		public string TagName
			=> NativeView.GetType().Name;

		public string Text
		{
			get
			{
				if (NativeView is UIKit.IUITextInput ti)
				{
					var start = ti.BeginningOfDocument;
					var end = ti.EndOfDocument;
					var range = ti.GetTextRange(start, end);
					return ti.TextInRange(range);
				}

				return null;
			}
		}

		public bool Selected =>
			NativeView.Focused;

		public Point Location
			=> new Point((int)NativeView.Frame.X, (int)NativeView.Frame.Y);

		public Size Size
			=> new Size((int)NativeView.Frame.Width, (int)NativeView.Frame.Height);

		public bool Displayed
			=> !NativeView.Hidden;

		public void Clear()
		{
			if (NativeView is UIKit.IUITextInput ti)
			{
				var start = ti.BeginningOfDocument;
				var end = ti.EndOfDocument;
				var range = ti.GetTextRange(start, end);
				ti.ReplaceText(range, string.Empty);
			}
		}

		public void SendKeys(string text)
		{
			if (NativeView is UIKit.IUITextInput ti)
			{
				var start = ti.BeginningOfDocument;
				var end = ti.EndOfDocument;
				var range = ti.GetTextRange(start, end);
				ti.ReplaceText(range, text);
			}
		}

		public void Submit()
		{
			throw new NotImplementedException();
		}

		public void Click()
		{
			throw new NotImplementedException();
		}

		public string GetAttribute(string attributeName)
			=> GetProperty(attributeName);

		public string GetProperty(string propertyName)
		{
			var selector = new ObjCRuntime.Selector(propertyName);
			var getSelector = new ObjCRuntime.Selector("get" + System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(propertyName));

			if (NativeView.RespondsToSelector(selector))
			{
				if (NativeView.PerformSelector(selector) is Foundation.NSString str)
					return str.ToString();
			}

			if (NativeView.RespondsToSelector(getSelector))
			{
				if (NativeView.PerformSelector(getSelector) is Foundation.NSString str)
					return str.ToString();
			}

			return null;
		}

		public string GetCssValue(string propertyName)
			=> GetProperty(propertyName);

		public IWebElement FindElement(By by)
			=> by.FindElement(this);

		public ReadOnlyCollection<IWebElement> FindElements(By by)
			=> by.FindElements(this);
	}
}
