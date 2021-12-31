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

        internal static IEnumerable<IElement> FindDepthFirst(this IEnumerable<IElement> elements, IElementSelector? selector)
        {
            var st = new Stack<IElement>();
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

        internal static IEnumerable<IElement> FindBreadthFirst(this IEnumerable<IElement> elements, IElementSelector? selector)
        {
            var q = new Queue<IElement>();

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

		public static IReadOnlyCollection<IElement> AsReadOnlyCollection(this IElement element)
        {
			var list = new List<IElement> { element };
			return list.AsReadOnly();
        }
	}
}
