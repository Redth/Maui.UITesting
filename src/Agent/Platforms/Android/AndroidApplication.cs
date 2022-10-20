using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using Kotlin.Reflect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	// All the code in this file is only included on Android.
	public class AndroidApplication : Application
	{
		public AndroidApplication(global::Android.App.Application app) : base()
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));

			LifecycleListener = new AutomationActivityLifecycleContextListener();

			app.RegisterActivityLifecycleCallbacks(LifecycleListener);
		}

		public override Platform DefaultPlatform => Platform.Android;

		AutomationActivityLifecycleContextListener LifecycleListener { get; }

		//public override Task<IWindow> CurrentWindow()
		//{
		//	var activity = LifecycleListener.Activity ?? LifecycleListener.Activities.FirstOrDefault();

		//	if (activity == null)
		//		return Task.FromResult<IWindow>(default);

		//	return Task.FromResult<IWindow>(new AndroidWindow(this, activity));
		//}

		public bool IsActivityCurrent(Activity activity)
			=> LifecycleListener.Activity == activity;

		public override Task<string> GetProperty(string elementId, string propertyName)
		{
			throw new NotImplementedException();
		}

		public override Task<IEnumerable<IElement>> GetElements()
		{
			var results = new List<IElement>();

			//foreach (var a in LifecycleListener.Activities)
			//{
			//	var window = a.GetElement(this, 1, -1);

			//	results.Add(window);
			//}
			
			// get the list from WindowManagerGlobal.mViews
			var wmgClass = Class.ForName("android.view.WindowManagerGlobal");
			var wmgInstance = wmgClass.GetMethod("getInstance").Invoke(null);
			var viewsField = wmgClass.GetDeclaredField("mViews");
			viewsField.Accessible = true;
				
			var views = viewsField.Get(wmgInstance).JavaCast<JavaList<View>>();

			foreach (var view in views)
			{
				results.Add(view.GetElement(this, view.EnsureUniqueId(), 1, -1));
			}

			return Task.FromResult<IEnumerable<IElement>>(results);
		}

		public override Task<IEnumerable<IElement>> FindElements(Predicate<IElement> matcher)
		{
			var windows = LifecycleListener.Activities.Select(a => a.GetElement(this, 1, 1));

			var matches = new List<Element>();
			Traverse(windows, matches, matcher);

			return Task.FromResult<IEnumerable<IElement>>(matches);
		}

		public override async Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments)
		{
            var elements = await this.GetElements();
            var element = elements.Find(e => e.Id == elementId).FirstOrDefault();

			if (element?.PlatformElement is Android.Views.View androidView)
				return await androidView.PerformAction(action, elementId, arguments);

			return PerformActionResult.Error($"Unrecognized action: {action}");
		}

		void Traverse(IEnumerable<Element> elements, IList<Element> matches, Predicate<Element> matcher)
		{
			foreach (var e in elements)
			{
				if (matcher(e))
					matches.Add(e);

				if (e.PlatformElement is View view)
				{
					var children = view.GetChildren(this, e.Id, 1, 1);
					Traverse(children, matches, matcher);
				}
			}
		}

		internal class AutomationActivityLifecycleContextListener : Java.Lang.Object, Android.App.Application.IActivityLifecycleCallbacks
		{
			public readonly List<Activity> Activities = new List<Activity>();

			WeakReference<Activity> currentActivity = new WeakReference<Activity>(null);

			internal Context Context =>
				Activity ?? Android.App.Application.Context;

			internal Activity Activity
			{
				get => currentActivity.TryGetTarget(out var a) ? a : null;
				set => currentActivity.SetTarget(value);
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityCreated(Activity activity, Bundle savedInstanceState)
			{
				if (!Activities.Contains(activity))
					Activities.Add(activity);

				Activity = activity;
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityDestroyed(Activity activity)
			{
				if (Activities.Contains(activity))
					Activities.Remove(activity);
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityPaused(Activity activity)
			{
				Activity = activity;
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityResumed(Activity activity)
			{
				Activity = activity;
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivitySaveInstanceState(Activity activity, Bundle outState)
			{ }

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityStarted(Activity activity)
			{
				if (!Activities.Contains(activity))
					Activities.Add(activity);
			}

			void Android.App.Application.IActivityLifecycleCallbacks.OnActivityStopped(Activity activity)
			{
				if (Activities.Contains(activity))
					Activities.Remove(activity);
			}
		}
	}
}
