using Android.Views;
using AndroidX.Core.View.Accessibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Maui.Automation
{
	public static class AndroidExtensions
	{
		public static IReadOnlyCollection<IElement> GetChildren(this Android.Views.View nativeView, IApplication application, string parentId)
		{
			var c = new List<IElement>();

			if (nativeView is ViewGroup vg)
			{
				for (int i = 0; i < vg.ChildCount; i++)
					c.Add(new AndroidView(application, vg.GetChildAt(i), parentId));
			}

			return new ReadOnlyCollection<IElement>(c.ToList());
		}

		public static IReadOnlyCollection<IElement> GetChildren(this Android.App.Activity activity, IApplication application, string parentId)
		{
			var rootView = activity.Window?.DecorView?.RootView ??
						activity.FindViewById(Android.Resource.Id.Content)?.RootView ??
						activity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

			if (rootView is not null)
				return new ReadOnlyCollection<IElement>(new List<IElement> { new AndroidView(application, rootView, parentId) });

			return new ReadOnlyCollection<IElement>(new List<IElement>());
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

		public static string GetAutomationId(this View view)
		{
			var id = string.Empty;

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
