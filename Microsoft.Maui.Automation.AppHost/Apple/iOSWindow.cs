#if IOS || MACCATALYST
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UIKit;

namespace Microsoft.Maui.Automation
{
	public class iOSWindow : Element
	{
		public iOSWindow(IApplication application, UIWindow window)
			: base(application, Platform.iOS, window.Handle.ToString())
		{
			PlatformWindow = window;
			PlatformElement = window;
			AutomationId = window.AccessibilityIdentifier ?? Id;

			var children = window.Subviews?.Select(s => new iOSView(application, s, Id))?.ToList<IElement>() ?? new List<IElement>();
			Children = new ReadOnlyCollection<IElement>(children);
			Width = (int)PlatformWindow.Frame.Width;
			Height = (int)PlatformWindow.Frame.Height;
			Text = string.Empty;
		}

		[JsonIgnore]
		public readonly UIWindow PlatformWindow;
	}
}
#endif