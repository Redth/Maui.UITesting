using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Maui.Automation
{

    internal static class HelperExtensions
	{
		internal static void PushAllReverse<T>(this Stack<T> st, IEnumerable<T> elems)
		{
			foreach (var elem in elems.Reverse())
				st.Push(elem);
		}

        internal static IEnumerable<Element> FindDepthFirst(this IEnumerable<Element> elements, IElementSelector? selector)
        {
            var st = new Stack<Element>();
            st.PushAllReverse(elements);

            while (st.Count > 0)
            {
                var v = st.Pop();
                if (selector == null || selector.Matches(v))
                {
                    yield return v;
                }

                st.PushAllReverse(v.Children);
            }
        }

        internal static IEnumerable<Element> FindBreadthFirst(this IEnumerable<Element> elements, IElementSelector? selector)
        {
            var q = new Queue<Element>();

            foreach (var e in elements)
                q.Enqueue(e);
            
            while (q.Count > 0)
            {
                var v = q.Dequeue();
                if (selector == null || selector.Matches(v))
                {
                    yield return v;
                }
                foreach (var c in v.Children)
                    q.Enqueue(c);
            }
        }

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> elems)
		{
			return new ReadOnlyCollection<T>(elems.ToList());
		}

		public static IReadOnlyCollection<Element> AsReadOnlyCollection(this Element element)
        {
			var list = new List<Element> { element };
			return list.AsReadOnly();
        }
	}
}
