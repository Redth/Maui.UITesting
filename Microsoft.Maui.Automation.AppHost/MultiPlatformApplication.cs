using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class MultiPlatformApplication : Application
    {
		public MultiPlatformApplication
			(
			Platform defaultPlatform
#if ANDROID
			, Android.App.Application application
#endif
			)
		{
			DefaultPlatform = defaultPlatform;
			PlatformApps = new Dictionary<Platform, IApplication>
            {
				{ Platform.MAUI, new MauiApplication() },
				{ App.GetCurrentPlatform(), App.CreateForCurrentPlatform(
#if ANDROID
												application
#endif
				) }
            };
		}

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

        public override IAsyncEnumerable<IElement> Children(Platform platform)
			=> GetApp(platform).Children(platform);

		public override Task<IElement> Element(Platform platform, string elementId)
			=> GetApp(platform).Element(platform, elementId);

		public override Task<object> GetProperty(Platform platform, string elementId, string propertyName)
			=> GetApp(platform).GetProperty(platform, elementId, propertyName);

		public override Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
			=> GetApp(platform).Perform(platform, elementId, action);
    }
}

