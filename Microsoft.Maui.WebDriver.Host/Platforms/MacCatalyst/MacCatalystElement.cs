using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using UIKit;

namespace Microsoft.Maui.WebDriver.Host
{
	public class MacCatalystElement : iOSElement
	{
		public MacCatalystElement(UIKit.UIView nativeView)
			: base(nativeView)
		{
		}

		public override IEnumerable<IPlatformElement> Children
			=> NativeView.Subviews.Select(s => new MacCatalystElement(s));
	}
}
