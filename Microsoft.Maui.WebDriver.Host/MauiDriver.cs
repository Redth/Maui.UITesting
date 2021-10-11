using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Essentials;

namespace Microsoft.Maui.WebDriver.Host
{
	public class MauiDriver : PlatformDriverBase
	{
		public override IEnumerable<IPlatformElement> Views
		{
			get
			{
				var windows =
#if ANDROID
			MauiApplication.Current.Application.Windows;

#elif IOS || __MACCATALYST__
			MauiUIApplicationDelegate.Current.Application.Windows;
#else
			new IWindow[0];
#endif

				return windows.Select(window => new MauiElement(window.Content));
			}
		}

		internal static T InvokeOnMainThread<T> (Func<T> func)
        {
			T t = default(T);
			MainThread.BeginInvokeOnMainThread(() => t = func());
			return t;
        }
	}
}

