using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
	public class MacCatalystDriver : iOSDriver
	{
		protected virtual IPlatformElement ElementFactory(UIView view)
		{
			return new MacCatalystElement(view);
		}
	}
}
