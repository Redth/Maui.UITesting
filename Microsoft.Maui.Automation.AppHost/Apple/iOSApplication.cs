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
		public override Platform DefaultPlatform => Platform.iOS;

		public override async Task<object> GetProperty(Platform platform, string elementId, string propertyName)
		{
			var p = await base.GetProperty(platform, elementId, propertyName);

			if (p != null)
				return p;

			var selector = new ObjCRuntime.Selector(propertyName);
			var getSelector = new ObjCRuntime.Selector("get" + System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(propertyName));

			var element = await FindElementOrThrow(platform, elementId);

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

		public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
		{
			throw new NotImplementedException();
		}

		public override async IAsyncEnumerable<IElement> Children(Platform platform)
		{
			var scenes = UIApplication.SharedApplication.ConnectedScenes?.ToArray();

			var hadScenes = false;

			if (scenes?.Any() ?? false)
			{
				foreach (var scene in scenes)
				{
					if (scene is UIWindowScene windowScene)
					{
						foreach (var window in windowScene.Windows)
						{
							yield return new iOSWindow(this, window);
							hadScenes = true;
						}
					}
				}
			}


			if (!hadScenes)
			{
				if (!OperatingSystem.IsMacCatalystVersionAtLeast(15, 0) && !OperatingSystem.IsIOSVersionAtLeast(15, 0))
				{
					foreach (var window in UIApplication.SharedApplication.Windows)
					{
						yield return new iOSWindow(this, window);
					}
				}
			}
		}
	}
}
#endif