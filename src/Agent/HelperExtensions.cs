using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Maui.Automation
{
	public static class App
	{
		public static Maui.IApplication GetCurrentMauiApplication()
			=>
#if IOS || MACCATALYST
				Maui.MauiUIApplicationDelegate.Current.Application;
#elif ANDROID
				Maui.MauiApplication.Current.Application;
#elif WINDOWS
				Maui.MauiWinUIApplication.Current.Application;
#else
				null;
#endif

		public static Platform GetCurrentPlatform()
			=>
#if IOS || MACCATALYST
				Platform.Ios;
#elif ANDROID
				Platform.Android;
#elif WINDOWS
				Platform.Winappsdk;
#else
				Platform.Maui;
#endif

		public static IApplication CreateForCurrentPlatform
			(
#if ANDROID
				Android.App.Application application
#endif
			) =>
#if IOS || MACCATALYST
				new iOSApplication();
#elif ANDROID
				new AndroidApplication(application);
#elif WINDOWS
				new WindowsAppSdkApplication();
#else
			throw new PlatformNotSupportedException();
#endif
	}
}
