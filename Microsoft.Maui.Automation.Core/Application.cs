using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public abstract class Application : IApplication
    {
        

        public virtual async Task<IWindow?> CurrentWindow()
            => (await Windows())?.FirstOrDefault();

        public virtual void Close()
        {
        }

        ~Application()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        bool disposed;
        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeManagedResources();
                DisposeUnmanagedResources();
                disposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        public abstract Task<IWindow[]> Windows();

        // Find helpers
        public virtual IAsyncEnumerable<IView> Descendants(IElement of, IViewSelector? selector = null)
            => of.Children.FindDepthFirst(selector);

        public virtual async Task<IView?> Descendant(IElement of, IViewSelector? selector = null)
        {
            await foreach (var element in of.Children.FindDepthFirst(selector))
                return element;

            return null;
        }

        public abstract Task<IActionResult> Invoke(IView view, IAction action);

        public virtual Task<object?> GetProperty(IView view, string propertyName)
        {
            var t = view.PlatformElement?.GetType();

            if (t != null)
            {
                var prop = t.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (prop != null)
                {
                    return Task.FromResult(prop.GetValue(view.PlatformElement));
                }
            }

            return Task.FromResult<object?>(null);
        }

        public virtual async Task<IWindow?> Window(string windowId)
            => (await Windows()).FirstOrDefault(w => w.Id == windowId);

        public virtual async Task<IView?> View(string windowId, string viewId)
        {
            var window = await Window(windowId);

            if (window == null)
                return null;

            return await Descendant(window, new IdSelector(viewId));
        }

        public async IAsyncEnumerable<IView> Descendants(string windowId, string? viewId = null, IViewSelector? selector = null)
        {
            if (string.IsNullOrEmpty(viewId))
            {
                var window = await Window(windowId);

                if (window != null)
                {
                    await foreach (var d in Descendants(window, selector))
                        yield return d;
                }
            }
            else
            {
                var view = await View(windowId, viewId);

                if (view != null)
                {
                    await foreach (var d in Descendants(view, selector))
                        yield return d;
                }
            }
        }

        public async Task<IActionResult> Invoke(string windowId, string viewId, IAction action)
        {
            var view = await View(windowId, viewId);

            if (view != null)
                return await Invoke(view, action);

            return new ActionResult(ActionResultStatus.Error, $"No view found for: Window:{windowId} -> View:{viewId}");
        }

        public async Task<object?> GetProperty(string windowId, string viewId, string propertyName)
        {
            var view = await View(windowId, viewId);
            if (view != null)
                return await GetProperty(view, propertyName);
            return null;
        }
    }
}
