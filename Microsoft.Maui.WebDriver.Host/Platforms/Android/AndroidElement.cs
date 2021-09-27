using Android.Views;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
	public class AndroidElement : IPlatformElement
	{
		public AndroidElement(View nativeView)
		{
			NativeView = nativeView;
		}

		public View NativeView { get; set; }

		public string AutomationId
		{
			get
			{
				if (NativeView.Tag is Java.Lang.String str)
					return str.ToString();

				return NativeView.ContentDescription;
			}
		}

		public bool Enabled
			=> NativeView.Enabled;

		public IPlatformElement[] Children
		{
			get
			{
				if (NativeView is ViewGroup vg)
				{
					var views = new List<View>();
					for (int i = 0; i < vg.ChildCount; i++)
						views.Add(vg.GetChildAt(i));
					return views.Select(v => new AndroidElement(v)).ToArray();
				}

				return null;
			}
		}

		public string TagName
			=> NativeView.GetType().Name;

		public string Text
		{
			get
			{
				if (NativeView is Android.Widget.TextView tv)
					return tv.Text;

				return null;
			}
		}

		public bool Selected =>
			NativeView.Selected;

		public Point Location
        {
			get
            {
				var loc = new int[2];
				NativeView.GetLocationInWindow(loc);

				if (loc != null && loc.Length >= 2)
                	return new Point(loc[0], loc[1]);

				return Point.Empty;
            }
        }

		public Size Size
			=> new Size(NativeView.MeasuredWidth, NativeView.MeasuredHeight);

		public bool Displayed
			=> NativeView.Visibility == ViewStates.Visible;

		public void Clear()
		{
			if (NativeView is Android.Widget.TextView tv)
				tv.Text = String.Empty;
		}

		public void SendKeys(string text)
		{
			if (NativeView is Android.Widget.TextView tv)
				tv.Text = text;
		}

		public void Submit()
		{
			throw new NotImplementedException();
		}

		public void Click()
		{
			NativeView.CallOnClick();
		}

		public string GetAttribute(string attributeName)
			=> GetProperty(attributeName);

		public string GetProperty(string propertyName)
		{
			// TODO:
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