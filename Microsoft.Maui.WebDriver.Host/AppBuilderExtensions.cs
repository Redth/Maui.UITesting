using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public static void EnqueAll<T>(this Queue<T> q, IEnumerable<T> elems)
		{
			foreach (var elem in elems)
				q.Enqueue(elem);
		}

		public static void PushAllReverse<T>(this Stack<T> st, IEnumerable<T> elems)
		{
			foreach (var elem in elems.Reverse())
				st.Push(elem);
		}

		public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> elems)
		{
			return new ReadOnlyCollection<T>(elems.ToList());
		}
	}
}
