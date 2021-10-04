using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.WebDriver.Host
{
	public static class AppBuilderExtensions
	{
#if ANDROID
		internal static Android.App.Activity CurrentActivity => Microsoft.Maui.Essentials.Platform.CurrentActivity;
#endif

		internal static void EnqueAll<T>(this Queue<T> q, IEnumerable<T> elems)
		{
			foreach (var elem in elems)
				q.Enqueue(elem);
		}

		internal static void PushAllReverse<T>(this Stack<T> st, IEnumerable<T> elems)
		{
			foreach (var elem in elems.Reverse())
				st.Push(elem);
		}

		internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> elems)
		{
			return new ReadOnlyCollection<T>(elems.ToList());
		}
	}
}
