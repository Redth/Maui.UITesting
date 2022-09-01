using Android.App;
using Android.Content;
using Android.OS;
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

		public override Task<string> GetProperty(Platform platform, string elementId, string propertyName)
		{
			throw new NotImplementedException();
		}

		public override Task<IEnumerable<Element>> GetElements(Platform platform, string elementId = null, int depth = 0)
		{
			return Task.FromResult(LifecycleListener.Activities.Select(a => a.GetElement(this)));
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
