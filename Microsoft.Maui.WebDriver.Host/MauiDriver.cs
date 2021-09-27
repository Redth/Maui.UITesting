using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

#elif IOS
			MauiUIApplicationDelegate.Current.Application.Windows;
#else
			new IWindow[0];
#endif

				return windows.Select(window => new MauiElement(window.Content));
			}
		}
	}
}

