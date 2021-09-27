using System;

namespace Microsoft.Maui.WebDriver.Host
{
    // All the code in this file is only included on Android.
    public class AndroidDriver : PlatformDriverBase
    {
        public override IPlatformElement[] GetViews()
        {
            var rootView = AppBuilderExtensions.CurrentActivity.Window?.DecorView?.RootView;

            if (rootView == null)
                rootView = AppBuilderExtensions.CurrentActivity.FindViewById(Android.Resource.Id.Content)?.RootView;

            if (rootView == null)
                rootView = AppBuilderExtensions.CurrentActivity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

            if (rootView == null)
                return Array.Empty<AndroidElement>();

            return new[] { new AndroidElement(rootView) };
        }
    }
}