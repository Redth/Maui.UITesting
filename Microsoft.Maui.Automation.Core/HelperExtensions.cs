using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Maui.Automation
{

    internal static class HelperExtensions
	{
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

        internal static async IAsyncEnumerable<IView> FindDepthFirst(this IReadOnlyCollection<IView> elements, Predicate<IView>? selector)
        {
            var st = new Stack<IView>();
            st.PushAllReverse(elements);

            while (st.Count > 0)
            {
                var v = st.Pop();
                if (selector == null || selector(v))
                {
                    yield return v;
                }

                st.PushAllReverse(v.Children);
            }
        }

        internal static IEnumerable<IView> FindBreadthFirst(this IEnumerable<IView> views, Predicate<IView>? selector)
        {
            var q = new Queue<IView>();
            q.EnqueAll(views);
            while (q.Count > 0)
            {
                var v = q.Dequeue();
                if (selector == null || selector(v))
                {
                    yield return v;
                }
                q.EnqueAll(v.Children);
            }
        }

        internal static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> elems)
		{
			return new ReadOnlyCollection<T>(elems.ToList());
		}

		internal static IReadOnlyCollection<IView> AsReadOnlyCollection(this IView element)
        {
			var list = new List<IView> { element };
			return list.AsReadOnly();
        }

		internal static IReadOnlyCollection<IWindow> AsReadOnlyCollection(this IWindow window)
		{
			var list = new List<IWindow> { window };
			return list.AsReadOnly();
		}

	}
}
