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

        public virtual IAsyncEnumerable<IView> Descendants(IElement of, IViewSelector? selector = null)
            => CurrentApp.Descendants(of, selector);

        public virtual Task<IView?> Descendant(IElement of, IViewSelector? selector = null)
            => CurrentApp.Descendant(of, selector);

        public virtual Task<object?> GetProperty(IView view, string propertyName)
            => CurrentApp.GetProperty(view, propertyName);

        public virtual Task<IWindow?> Window(string id)
            => CurrentApp.Window(id);

        public virtual Task<IView?> View(string windowId, string viewId)
            => CurrentApp.View(windowId, viewId);

        public virtual IAsyncEnumerable<IView> Descendants(string windowId, string? viewId = null, IViewSelector? selector = null)
            => CurrentApp.Descendants(windowId, viewId, selector);

        public virtual Task<IActionResult> Invoke(string windowId, string viewId, IAction action)
            => CurrentApp.Invoke(windowId, viewId, action);

        public virtual Task<object?> GetProperty(string windowId, string viewId, string propertyName)
            => CurrentApp.GetProperty(windowId, viewId, propertyName);
    }
}
