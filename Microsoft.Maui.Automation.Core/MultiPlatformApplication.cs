using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class MultiPlatformApplication : IMultiPlatformApplication
    {
        public MultiPlatformApplication(params (Platform platform, IApplication app)[] apps) : base()
        {
            PlatformApps = new Dictionary<Platform, IApplication>();

            if (apps != null)
            {
                foreach (var app in apps)
                    PlatformApps.Add(app.platform, app.app);
            }

            CurrentPlatform = PlatformApps.Any() ? PlatformApps.First().Key : Platform.MAUI;
        }

        protected readonly IDictionary<Platform, IApplication> PlatformApps;

        public Platform CurrentPlatform { get; set; }

        IApplication CurrentApp => PlatformApps[CurrentPlatform];

        public virtual Task<IWindow?> CurrentWindow()
            => CurrentApp.CurrentWindow();

        public virtual Task<IWindow[]> Windows()
            => CurrentApp.Windows();

        public virtual Task<IActionResult> Invoke(IView view, IAction action)
            => CurrentApp.Invoke(view, action);

        Task IMultiPlatformApplication.Platform(Platform platform)
        {
            CurrentPlatform = platform;
            return Task.CompletedTask;
        }

        Task<Platform> IMultiPlatformApplication.CurrentPlatform()
            => Task.FromResult(CurrentPlatform);

        public virtual IAsyncEnumerable<IView> Descendants(IElement of, Predicate<IView>? selector = null)
            => CurrentApp.Descendants(of, selector);

        public virtual Task<IView?> Descendant(IElement of, Predicate<IView>? selector = null)
            => CurrentApp.Descendant(of, selector);

        public virtual Task<IView[]> Tree(IElement of)
            => CurrentApp.Tree(of);

        public virtual Task<object?> GetProperty(IView element, string propertyName)
            => CurrentApp.GetProperty(element, propertyName);

        public virtual Task<IWindow?> Window(string id)
            => CurrentApp.Window(id);

        public virtual Task<IView?> View(string windowId, string viewId)
            => CurrentApp.View(windowId, viewId);

        public virtual IAsyncEnumerable<IView> Descendants(string windowId)
            => CurrentApp.Descendants(windowId);

        public virtual IAsyncEnumerable<IView> Descendants(string windowId, string elementId)
            => CurrentApp.Descendants(windowId, elementId);

        public virtual Task<IActionResult> Invoke(string windowId, string elementId, IAction action)
            => CurrentApp.Invoke(windowId, elementId, action);

        public virtual Task<object?> GetProperty(string windowId, string elementId, string propertyName)
            => CurrentApp.GetProperty(windowId, elementId, propertyName);
    }
}
