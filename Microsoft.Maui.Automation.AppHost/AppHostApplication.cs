using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class AppHostApplication : MultiPlatformApplication
	{
		public AppHostApplication
			(
			Platform defaultPlatform
#if ANDROID
			, Android.App.Application application
#endif
			)
			: base(defaultPlatform, new[] { 
				( Platform.Maui, new MauiApplication() ),
				( App.GetCurrentPlatform(), App.CreateForCurrentPlatform(
#if ANDROID
												application
#endif
				)) 
			})
		{
		}
    }
}

