#if IOS || MACCATALYST
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Microsoft.Maui.Automation
{
	public class iOSApplication : Application
	{
        public override async Task<object> GetProperty(IView element, string propertyName)
        {	
			var p = await base.GetProperty(element, propertyName);

			if (p != null)
				return p;

			var selector = new ObjCRuntime.Selector(propertyName);
			var getSelector = new ObjCRuntime.Selector("get" + System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(propertyName));

			if (element is iOSView view)
			{
				if (view.PlatformView.RespondsToSelector(selector))
				{
					var v = view.PlatformView.PerformSelector(selector)?.ToString();
					if (v != null)
						return v;
				}

				if (view.PlatformView.RespondsToSelector(getSelector))
				{
					var v = view.PlatformView.PerformSelector(getSelector)?.ToString();
					if (v != null)
						return v;
				}
			}

			return Task.FromResult<object>(null);
		}

        public override Task<IActionResult> Invoke(IView view, IAction action)
        {
            throw new NotImplementedException();
        }

        public override Task<IWindow[]> Windows()
        {
			var windows = new List<IWindow>();

			var scenes = UIApplication.SharedApplication.ConnectedScenes?.ToArray();

			if (scenes?.Any() ?? false)
			{
				foreach (var scene in scenes)
				{
					if (scene is UIWindowScene windowScene)
					{
						foreach (var window in windowScene.Windows)
							windows.Add(new iOSWindow(window));
					}
				}
			}

			if (windows.Any())
				return Task.FromResult(windows.ToArray());

			if (!OperatingSystem.IsMacCatalystVersionAtLeast(15, 0) && !OperatingSystem.IsIOSVersionAtLeast(15, 0))
			{
				foreach (var window in UIApplication.SharedApplication.Windows)
				{
					windows.Add(new iOSWindow(window));
				}
			}

			return Task.FromResult(windows.ToArray());
		}
	}
}
#endif