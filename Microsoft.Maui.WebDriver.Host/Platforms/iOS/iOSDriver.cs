using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
	public class iOSDriver : PlatformDriverBase
	{
		public override IEnumerable<IPlatformElement> Views
		{
			get {
				var window = UIKit.UIApplication.SharedApplication.KeyWindow;

				return window.Subviews.Select(s => new iOSElement(s));
			}
		}
	}
}