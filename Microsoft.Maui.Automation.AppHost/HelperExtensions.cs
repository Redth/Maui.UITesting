using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Maui.Automation
{
    public static class App
    {
        public static Maui.IApplication GetCurrentMauiApplication()
            =>
#if IOS || MACCATALYST
                Maui.MauiUIApplicationDelegate.Current.Application;
#elif ANDROID
                Maui.MauiApplication.Current.Application;
#elif WINDOWS
                Maui.MauiWinUIApplication.Current.Application;
#else
                null;
#endif

        public static Platform GetCurrentPlatform()
            =>
#if IOS || MACCATALYST
                Platform.iOS;
#elif ANDROID
                Platform.Android;
#elif WINDOWS
                Platform.WinAppSdk;
#else
                Platform.MAUI;
#endif

        public static IApplication CreateForCurrentPlatform
            (
#if ANDROID
            Android.App.Application application
#endif
            ) =>
#if IOS || MACCATALYST
                new iOSApplication();
#elif ANDROID
            new AndroidApplication(application);
#elif WINDOWS
            new WindowsAppSdkApplication();
#else
            throw new PlatformNotSupportedException();
#endif
    }

    internal static class HelperExtensions
	{
        internal static void PushAllReverse<T>(this Stack<T> st, IEnumerable<T> elems)
        {
            foreach (var elem in elems.Reverse())
                st.Push(elem);
        }

        internal static async IAsyncEnumerable<IElement> FindDepthFirst(this IAsyncEnumerable<IElement> elements, IElementSelector? selector)
        {
            var list = new List<IElement>();
            await foreach (var e in elements)
                list.Add(e);

            await foreach (var e in FindDepthFirst(list, selector))
                yield return e;
        }

        internal static async IAsyncEnumerable<IElement> FindDepthFirst(this IEnumerable<IElement> elements, IElementSelector? selector)
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

        internal static async IAsyncEnumerable<IElement> FindBreadthFirst(this IAsyncEnumerable<IElement> elements, IElementSelector? selector)
        {
            var list = new List<IElement>();
            await foreach (var e in elements)
                list.Add(e);

            await foreach (var e in FindBreadthFirst(list, selector))
                yield return e;
        }

        internal static async IAsyncEnumerable<IElement> FindBreadthFirst(this IEnumerable<IElement> elements, IElementSelector? selector)
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
