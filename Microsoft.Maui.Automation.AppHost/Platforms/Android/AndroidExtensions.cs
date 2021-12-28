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

		public static string GetText(this Android.Views.View view)
        {
			if (view is Android.Widget.TextView tv)
				return tv.Text;

			return null;
		}

		internal static string EnsureUniqueId(this Android.Views.View view)
		{
			var id = view.GetTag(AppHost.Resource.Id.maui_automation_unique_identifier)?.ToString();

			if (string.IsNullOrEmpty(id))
            {
				id = Guid.NewGuid().ToString();
				view.SetTag(AppHost.Resource.Id.maui_automation_unique_identifier, id);
            }

			return id;
		}

		public static string GetAutomationId(this Android.App.Activity activity)
			=> activity.GetRootView().GetAutomationId();

		public static string GetAutomationId(this Android.Views.View view)
		{
			var id = view.GetTag(Resource.Id.automation_tag_id)?.ToString();

			if (string.IsNullOrEmpty(id))
				id = view.ContentDescription;

			if (string.IsNullOrEmpty(id) && view.Tag is Java.Lang.String str)
				id = str.ToString();

			if (string.IsNullOrEmpty(id))
				id = view.EnsureUniqueId();

			return id;
		}

		static Android.Views.View GetRootView(this Android.App.Activity activity)
			=> activity.Window?.DecorView?.RootView ??
				activity.FindViewById(Android.Resource.Id.Content)?.RootView ??
					activity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

		public static string GetWindowId(this Android.App.Activity activity)
		{
			var rootView = activity.GetRootView();

			return rootView.EnsureUniqueId();
		}
	}
}
