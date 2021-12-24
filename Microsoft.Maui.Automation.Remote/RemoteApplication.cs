using StreamJsonRpc;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteApplication : IApplication
    {
        public RemoteApplication(Stream stream, IRemoteAutomationService? remoteAutomationService = null)
        {
            Stream = stream;

            var messageFormatter = new JsonMessageFormatter();

            messageFormatter.JsonSerializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
           
            var messageHandler = new HeaderDelimitedMessageHandler(Stream, messageFormatter);
            

            if (remoteAutomationService != null)
            {
                // Attaching as a server, provided an implementation to fulfill requests
                RemoteAutomationService = remoteAutomationService;
                Rpc = new JsonRpc(messageHandler, RemoteAutomationService);
                Rpc.StartListening();
            }
            else
            {
                // Attaching as a client
                Rpc = new JsonRpc(messageHandler);
                RemoteAutomationService = Rpc.Attach<IRemoteAutomationService>();
                Rpc.StartListening();
            }
        }

        protected JsonRpc Rpc { get; set; }

        public Task Completion => Rpc?.Completion ?? Task.CompletedTask;

        public readonly IRemoteAutomationService RemoteAutomationService;
        public readonly Stream Stream;

        public Task Platform(Platform platform)
            => RemoteAutomationService.Platform(platform);

        public async Task<IWindow?> CurrentWindow()
        {
            var remoteWindow = await RemoteAutomationService.CurrentWindow();

            if (remoteWindow == null)
                return null;

            return RemoteWindow.From(remoteWindow);
        }

        public async Task<IWindow[]> Windows()
        {
            var remoteWindows = await RemoteAutomationService.Windows();

            return remoteWindows.ToArray<IWindow>();
        }

        public async IAsyncEnumerable<IView> Descendants(IElement of, Predicate<IView>? selector = null)
        {
            if (of is IWindow window)
            {
                var windowId = window.Id;

                await foreach (var d in RemoteAutomationService.Descendants(windowId))
                {
                    if (selector == null || selector(d))
                        yield return d;
                }
            }
            else if (of is IView view)
            {
                var windowId = view.WindowId;
                var id = view.Id;

                await foreach (var d in RemoteAutomationService.Descendants(windowId, id))
                {
                    if (selector == null || selector(d))
                        yield return d;
                }
            }
        }

        public async Task<IView?> Descendant(IElement of, Predicate<IView>? selector = null)
        {
            if (of is IWindow window)
            {
                var windowId = window.Id;

                await foreach (var d in RemoteAutomationService.Descendants(windowId))
                {
                    if (selector == null || selector(d))
                        return d;
                }
            }
            else if (of is IView view)
            {
                var windowId = view.WindowId;
                var id = view.Id;

                await foreach (var d in RemoteAutomationService.Descendants(windowId, id))
                {
                    if (selector == null || selector(d))
                        return d;
                }
            }

            return null;
        }

        public async Task<IView[]> Tree(IElement of)
        {
            if (of is IWindow window)
            {
                return await RemoteAutomationService.Tree(window.Id);
            }
            else if (of is IView view)
            {
                return await RemoteAutomationService.Tree(view.WindowId, view.Id);
            }

            return null;
        }

        public Task<IActionResult> Invoke(IView view, IAction action)
            => RemoteAutomationService.Invoke(view.WindowId, view.Id, action);

        public Task<object?> GetProperty(IView element, string propertyName)
            => RemoteAutomationService.GetProperty(element.WindowId, element.Id, propertyName);

        public async Task<IWindow?> Window(string windowId)
            => await RemoteAutomationService.Window(windowId);

        public async Task<IView?> View(string windowId, string viewId)
            => await RemoteAutomationService.View(windowId, viewId);

        public IAsyncEnumerable<IView> Descendants(string windowId)
            => RemoteAutomationService.Descendants(windowId);
        
        public IAsyncEnumerable<IView> Descendants(string windowId, string elementId)
            => RemoteAutomationService.Descendants(windowId, elementId);

        public Task<IActionResult> Invoke(string windowId, string elementId, IAction action)
            => RemoteAutomationService.Invoke(windowId, elementId, action);

        public Task<object> GetProperty(string windowId, string elementId, string propertyName)
            => RemoteAutomationService.GetProperty(windowId, elementId, propertyName);
    }
}