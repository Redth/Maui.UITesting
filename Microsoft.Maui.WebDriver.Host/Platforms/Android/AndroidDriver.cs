using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;

namespace Microsoft.Maui.WebDriver.Host
{
    // All the code in this file is only included on Android.
    public class AndroidDriver : PlatformDriverBase
    {
        public AndroidDriver(Application app)
            : base()
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            Init(app);
        }

        public override IEnumerable<IPlatformElement> Views
        {
            get
            {
                var rootView = CurrentActivity.Window?.DecorView?.RootView ??
                    CurrentActivity.FindViewById(Android.Resource.Id.Content)?.RootView ??
                    CurrentActivity.Window?.DecorView?.FindViewById(Android.Resource.Id.Content);

                if (rootView is not null)
                    yield return new AndroidElement(rootView);
            }
        }


        // from Essentials:

        static volatile Handler handler;

        internal static void BeginInvokeOnMainThread(Action action)
        {
            if (handler?.Looper != Looper.MainLooper)
                handler = new Handler(Looper.MainLooper);
            handler!.Post(action);
        }

        internal static T InvokeOnMainThread<T>(Func<T> factory)
        {
            T value = default;
            BeginInvokeOnMainThread(() => value = factory());
            return value;
        }

        static ActivityLifecycleContextListener? lifecycleListener;

        static void Init(Application application)
        {
            lifecycleListener = new ActivityLifecycleContextListener();
            application.RegisterActivityLifecycleCallbacks(lifecycleListener);
        }

        static Activity CurrentActivity => lifecycleListener?.Activity;

        class ActivityLifecycleContextListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            WeakReference<Activity> currentActivity = new WeakReference<Activity>(null);

            internal Activity Activity
            {
                get => currentActivity.TryGetTarget(out var a) ? a : null;
                set => currentActivity.SetTarget(value);
            }

            void Application.IActivityLifecycleCallbacks.OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                Activity = activity;
            }

            void Application.IActivityLifecycleCallbacks.OnActivityDestroyed(Activity activity) { }

            void Application.IActivityLifecycleCallbacks.OnActivityPaused(Activity activity)
            {
                Activity = activity;
            }

            void Application.IActivityLifecycleCallbacks.OnActivityResumed(Activity activity)
            {
                Activity = activity;
            }

            void Application.IActivityLifecycleCallbacks.OnActivitySaveInstanceState(Activity activity, Bundle outState) { }

            void Application.IActivityLifecycleCallbacks.OnActivityStarted(Activity activity) { }

            void Application.IActivityLifecycleCallbacks.OnActivityStopped(Activity activity) { }
        }
    }
}
