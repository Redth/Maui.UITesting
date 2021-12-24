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
            => RemoteWindow.From(await PlatformApp.CurrentWindow());

        public async Task<RemoteWindow?> Window(string id)
            => RemoteWindow.From(await PlatformApp.Window(id));

        public async Task<RemoteWindow[]> Windows()
        {
            var windows = await PlatformApp.Windows();

            var r = new List<RemoteWindow>();

            foreach (var w in windows)
                r.Add(RemoteWindow.From(w)!);

            return r.ToArray();
        }

        public async Task<RemoteView?> View(string windowId, string viewId)
            => RemoteView.From(await PlatformApp.View(windowId, viewId));

        public async IAsyncEnumerable<RemoteView> Descendants(string windowId)
        {
            await foreach (var d in PlatformApp.Descendants(windowId))
                yield return RemoteView.From(d)!;
        }

        public async IAsyncEnumerable<RemoteView> Descendants(string windowId, string elementId)
        {
            await foreach (var d in PlatformApp.Descendants(windowId, elementId))
                yield return RemoteView.From(d)!;
        }

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action)
            => PlatformApp.Invoke(windowId, elementId, action);

        public Task<object?> GetProperty(string windowId, string elementId, string propertyName)
            => PlatformApp.GetProperty(windowId, elementId, propertyName);

        public async Task<RemoteView[]> Tree(string windowId)
        {
            var window = await PlatformApp.Window(windowId);

            if (window == null)
                return Array.Empty<RemoteView>();

            var results = new List<RemoteView>();

            foreach (var rootView in window.Children.Select(rv => RemoteView.From(rv)!))
            {
                ConvertChildren(rootView, rootView.Children);
                results.Add(rootView);
            }

            return results.ToArray();
        }

        
        void ConvertChildren(RemoteView parent, IView[] toConvert)
        {
            var converted = toConvert.Select(c => RemoteView.From(c)!);
            parent.Children = converted.ToArray<IView>();

            foreach (var v in converted)
                ConvertChildren(v, v.Children);
        }
    
        public async Task<RemoteView[]> Tree(string windowId, string elementId)
        {
            var view = await PlatformApp.View(windowId, elementId);

            if (view == null)
                return Array.Empty<RemoteView>();

            var remoteView = RemoteView.From(view)!;

            ConvertChildren(remoteView, remoteView.Children);

            return remoteView.Children.Cast<RemoteView>().ToArray();
        }
    }
}
