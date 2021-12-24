using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Maui.Automation
{
    public static class AndroidExtensions
    {
		public static IView[] GetChildren(this Android.Views.View nativeView, string windowId)
        {
			var c = new List<IView>();

			if (nativeView is ViewGroup vg)
			{
				for (int i = 0; i < vg.ChildCount; i++)
					c.Add(new AndroidView(windowId, vg.GetChildAt(i)));
			}

			return c.ToArray();
		}

		public static IView[] GetChildren(this Android.App.Activity activity, string windowId)
        {
			var rootView = activity.Window?.DecorView?.RootView ??
						activity.FindViewById(Android.Resource.Id.Content)?.RootView ??
						activity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

			if (rootView is not null)
				return new [] { new AndroidView(windowId, rootView) };

			return Array.Empty<IView>();
		}

		public static string GetAutomationId(this Android.Views.View view)
        {
			if (view.Tag is Java.Lang.String str)
				return str.ToString();

			return view.ContentDescription ?? view.Id.ToString();
		}

		public static string GetText(this Android.Views.View view)
        {
			if (view is Android.Widget.TextView tv)
				return tv.Text;

			return null;
		}
    }
}
