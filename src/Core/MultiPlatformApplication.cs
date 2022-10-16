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

		public override Task<IEnumerable<IElement>> GetElements()
			=> GetApp(DefaultPlatform).GetElements();

		public override Task<string> GetProperty(string elementId, string propertyName)
			=> GetApp(DefaultPlatform).GetProperty(elementId, propertyName);

		public override Task<IEnumerable<IElement>> FindElements(Predicate<IElement> matcher)
			=> GetApp(DefaultPlatform).FindElements(matcher);


		public override Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
			=> GetApp(DefaultPlatform).PerformAction(action, elementId, arguments);

	}
}

