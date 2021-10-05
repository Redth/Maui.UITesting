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
				var window = KeyWindow;
				if (window == null)
					return Enumerable.Empty<IPlatformElement>();

				return window.Subviews.Select(s => new iOSElement(s));
			}
		}

		UIWindow KeyWindow
        {
			get
            {
				if (OperatingSystem.IsIOSVersionAtLeast (13))
				{
					return UIApplication.SharedApplication.Windows.FirstOrDefault(w => w.IsKeyWindow);
				}
				else
                {
					return UIKit.UIApplication.SharedApplication.KeyWindow;
                }
            }
        }
	}
}