using Microsoft.Maui.Automation.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public class RemoteAutomationService : IRemoteAutomationService
    {
        public RemoteAutomationService(IMultiPlatformApplication app)
        {
            PlatformApp = app;
        }
        
        readonly IMultiPlatformApplication PlatformApp;
        

        public async Task Platform(Platform platform)
        {
            await PlatformApp.Platform(platform);
            currentPlatform = platform;
        }

        Platform? currentPlatform;
        public async Task<Platform> CurrentPlatform()
        {
            if (currentPlatform == null)
                currentPlatform = await PlatformApp.CurrentPlatform();

            return currentPlatform ?? throw new PlatformNotSupportedException();
        }

        public async Task<RemoteWindow?> CurrentWindow()
            => RemoteWindow.From(PlatformApp, await PlatformApp.CurrentWindow());

        public async Task<RemoteWindow?> Window(string id)
            => RemoteWindow.From(PlatformApp, await PlatformApp.Window(id));

        public async Task<RemoteWindow[]> Windows()
        {
            var windows = await PlatformApp.Windows();

            var r = new List<RemoteWindow>();

            foreach (var w in windows)
                r.Add(RemoteWindow.From(PlatformApp, w)!);

            return r.ToArray();
        }

        public async Task<RemoteView?> View(string windowId, string viewId)
            => RemoteView.From(PlatformApp, await PlatformApp.View(windowId, viewId));

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action)
            => PlatformApp.Invoke(windowId, elementId, action);

        public Task<object?> GetProperty(string windowId, string elementId, string propertyName)
            => PlatformApp.GetProperty(windowId, elementId, propertyName);

        void ConvertChildren(RemoteView parent, IView[] toConvert, IViewSelector? selector)
        {
            selector ??= new DefaultViewSelector();

            var converted = toConvert
                .Where(c => selector.Matches(c))
                .Select(c => RemoteView.From(PlatformApp, c)!);

            parent.Children = converted.ToArray<IView>();

            foreach (var v in converted)
                ConvertChildren(v, v.Children, selector);
        }

        public async Task<RemoteView[]> Descendants(string windowId, string? viewId = null, IViewSelector? selector = null)
        {
            if (!string.IsNullOrEmpty(viewId))
            {
                var view = await PlatformApp.View(windowId, viewId);

                if (view == null)
                    return Array.Empty<RemoteView>();

                var remoteView = RemoteView.From(PlatformApp, view)!;

                ConvertChildren(remoteView, remoteView.Children, selector);

                return remoteView.Children.Cast<RemoteView>().ToArray();
            }
            else
            {
                var window = await PlatformApp.Window(windowId);

                if (window == null)
                    return Array.Empty<RemoteView>();

                var results = new List<RemoteView>();

                foreach (var rootView in window.Children.Select(rv => RemoteView.From(PlatformApp, rv)!))
                {
                    ConvertChildren(rootView, rootView.Children, selector);
                    results.Add(rootView);
                }

                return results.ToArray();
            }
        }
    }
}
