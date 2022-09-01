#if IOS || MACCATALYST
using Foundation;
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
		public override Platform DefaultPlatform => Platform.Ios;

		public override Task<string> GetProperty(Platform platform, string elementId, string propertyName)
		{
			var selector = new ObjCRuntime.Selector(propertyName);
			var getSelector = new ObjCRuntime.Selector("get" + System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(propertyName));

			var roots = GetRootElements();

			var element = roots.FindDepthFirst(new IdSelector(elementId))?.FirstOrDefault();

			if (element is not null && element.PlatformElement is NSObject nsobj)
			{
				if (nsobj.RespondsToSelector(selector))
				{
					var v = nsobj.PerformSelector(selector)?.ToString();
					if (v != null)
						return Task.FromResult(v);
				}

				if (nsobj.RespondsToSelector(getSelector))
				{
					var v = nsobj.PerformSelector(getSelector)?.ToString();
					if (v != null)
						return Task.FromResult(v);
				}
			}

			return Task.FromResult<string>(string.Empty);
		}

		public override Task<IEnumerable<Element>> GetElements(Platform platform, string elementId = null, int depth = 0)
		{
			var root = GetRootElements();

			if (string.IsNullOrEmpty(elementId))
				return Task.FromResult(root);

			return Task.FromResult(root.FindDepthFirst(new IdSelector(elementId)));
		}


		IEnumerable<Element> GetRootElements()
		{
			var children = new List<Element>();

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
							children.Add(window.GetElement(this));
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
						children.Add(window.GetElement(this));
					}
				}
			}

			return children;
		}
	}
}
#endif