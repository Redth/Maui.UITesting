using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.WebDriver.Host
{
	public class MauiDriver : PlatformDriverBase
	{
		public override IPlatformElement[] GetViews()
		{
			var windows =
#if ANDROID
			MauiApplication.Current.Application.Windows;

#elif IOS
			MauiUIApplicationDelegate.Current.Application.Windows;
#else
			new IWindow[0];
#endif

			var elements = new List<IPlatformElement>();

			foreach (var window in windows)
				elements.Add(new MauiElement(window.Content));

			return elements.ToArray();
		}
	}
}
