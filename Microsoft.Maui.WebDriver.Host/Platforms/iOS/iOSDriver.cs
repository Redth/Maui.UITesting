using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
	public class iOSDriver : PlatformDriverBase
	{
		public override IEnumerable<IPlatformElement> Views
		{
			get
			{
				return InvokeOnMainThread(() =>
				{
					var window = KeyWindow;
					var subviews = window.Subviews;

					return subviews.Select(s => ElementFactory(s));
				});
			}
		}

		UIWindow KeyWindow
		{
			get
			{
				if (OperatingSystem.IsIOSVersionAtLeast(13))
				{
					return UIApplication.SharedApplication.Windows.FirstOrDefault(w => w.IsKeyWindow);
				}
				else
				{
					return UIKit.UIApplication.SharedApplication.KeyWindow;
				}
			}
		}

		protected virtual IPlatformElement ElementFactory (UIView view)
		{
			return new iOSElement(view);
		}

		// from Essentials
		internal static void BeginInvokeOnMainThread(Action action)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread(action.Invoke);
		}

		internal static T InvokeOnMainThread<T>(Func<T> factory)
        {
			T value = default;
			NSRunLoop.Main.InvokeOnMainThread(() => value = factory());
			return value;
        }
	}
}
