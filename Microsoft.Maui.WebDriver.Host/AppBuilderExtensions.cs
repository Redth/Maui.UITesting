using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui.WebDriver.Host
{
	public static class AppBuilderExtensions
	{
#if ANDROID
		internal static Android.App.Activity CurrentActivity { get; set; }
#endif

		public static MauiAppBuilder UseWebDriverHost(this MauiAppBuilder builder)
		{
			builder.ConfigureLifecycleEvents(l =>
			{
#if ANDROID
				l.AddAndroid(android =>
				{
					android.AddEvent("OnResume", new AndroidLifecycle.OnResume(activity =>
					{
						CurrentActivity = activity;
					}));
				});
#endif
			});
		
			return builder;
		}
	}
}
