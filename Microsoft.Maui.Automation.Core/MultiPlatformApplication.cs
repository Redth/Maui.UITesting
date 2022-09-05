using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public class MultiPlatformApplication : Application
	{
		public MultiPlatformApplication(Platform defaultPlatform, params (Platform platform, IApplication app)[] apps)
		{
			DefaultPlatform = defaultPlatform;

			PlatformApps = new Dictionary<Platform, IApplication>();

			foreach (var app in apps)
				PlatformApps[app.platform] = app.app;
		}

		public readonly IDictionary<Platform, IApplication> PlatformApps;


		public override Platform DefaultPlatform { get; }

		IApplication GetApp(Platform platform)
		{
			if (!PlatformApps.ContainsKey(platform))
				throw new PlatformNotSupportedException();

			return PlatformApps[platform];
		}

		public override Task<IEnumerable<Element>> GetElements(Platform platform)
			=> GetApp(platform).GetElements(platform);

		public override Task<string> GetProperty(Platform platform, string elementId, string propertyName)
			=> GetApp(platform).GetProperty(platform, elementId, propertyName);

		public override Task<IEnumerable<Element>> FindElements(Platform platform, Func<Element, bool> matcher)
			=> GetApp(platform).FindElements(platform, matcher);


		public override Task<PerformActionResult> PerformAction(Platform platform, string action, string elementId, params string[] arguments)
			=> GetApp(platform).PerformAction(platform, action, elementId, arguments);

    }
}

