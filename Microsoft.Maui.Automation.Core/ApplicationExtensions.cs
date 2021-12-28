using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public static class ApiExtensions
    {

        public static IAsyncEnumerable<IView> By(IWindow window, params IViewSelector[] selectors)
            => All(window.Application!, window.Id, selectors);

        public static IAsyncEnumerable<IView> By(IWindow window, IViewSelector selector)
            => All(window.Application!, window.Id, selector);

        public static IAsyncEnumerable<IView> ByAutomationId(IWindow window, string automationId, StringComparison comparison = StringComparison.Ordinal)
            => By(window, new AutomationIdSelector(automationId, comparison));

        public static IAsyncEnumerable<IView> ById(IWindow window, string id, StringComparison comparison = StringComparison.Ordinal)
            => By(window, new IdSelector(id, comparison));

        public static async IAsyncEnumerable<IView> All(IApplication app, params IViewSelector[] selectors)
        {
            var windows = await app.Windows();

            foreach (var window in windows)
            {
                await foreach (var view in All(app, window.Id, selectors))
                    yield return view;
            }
        }

        public static async IAsyncEnumerable<IView> Any(IApplication app, params IViewSelector[] selectors)
        {
            var windows = await app.Windows();

            foreach (var window in windows)
            {
                await foreach (var view in Any(app, window.Id, selectors))
                    yield return view;
            }
        }

        public static IAsyncEnumerable<IView> All(IApplication app, IWindow window, params IViewSelector[] selectors)
            => All(app, window.Id, selectors);

        public static async IAsyncEnumerable<IView> All(IApplication app, string windowId, params IViewSelector[] selectors)
        {
            await foreach(var view in app.Descendants(windowId, viewId: null, new CompoundSelector(any: false, selectors)))
            {
                yield return view;
            }
        }

        public static IAsyncEnumerable<IView> Any(IApplication app, IWindow window, params IViewSelector[] selectors)
            => Any(app, window.Id, selectors);

        public static async IAsyncEnumerable<IView> Any(IApplication app, string windowId, params IViewSelector[] selectors)
        {
            await foreach (var view in app.Descendants(windowId, viewId: null, new CompoundSelector(any: true, selectors)))
            {
                yield return view;
            }
        }

        public static async Task<IView?> First(IApplication app, string windowId, bool matchesAnySelector, params IViewSelector[] selectors)
        {
            await foreach (var view in app.Descendants(windowId, viewId: null, new CompoundSelector(any: matchesAnySelector, selectors)))
            {
                return view;
            }

            return null;
        }
    }
}
