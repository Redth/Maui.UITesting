using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public static class ApiExtensions
    {

        //public static async Task<IView[]> ById(IApplication app, string id)
        //{

        //}


        public static async IAsyncEnumerable<IView> All(IApplication app, params IViewSelector[] selectors)
        {
            var windows = await app.Windows();

            foreach (var window in windows)
            {
                foreach (var view in await All(app, window, selectors))
                    yield return view;
            }
        }

        public static Task<IView[]> All(IApplication app, IWindow window, params IViewSelector[] selectors)
            => All(app, window.Id, selectors);

        public static async Task<IView[]> All(IApplication app, string windowId, params IViewSelector[] selectors)
        {
            var window = await app.Window(windowId);

            if (window != null)
                return await All(app, window, selectors);

            return Array.Empty<IView>();
        }
    }
}
