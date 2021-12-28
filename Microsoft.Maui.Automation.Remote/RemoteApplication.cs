using Streamer;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteApplication : IApplication
    {
        public RemoteApplication(Stream stream, IRemoteAutomationService? remoteAutomationService = null)
        {
            Stream = stream;

            if (remoteAutomationService != null)
            {
                RemoteAutomationService = remoteAutomationService;
                Server = Streamer.Channel.CreateServer();
                Server.Bind(
                    new MethodHandler<RemoteWindow[]>(
                        nameof(IRemoteAutomationService.Windows),
                        () => RemoteAutomationService.Windows()),
                    new MethodHandler<RemoteWindow?, string>(
                        nameof(IRemoteAutomationService.Window),
                        (id) => RemoteAutomationService.Window(id)),
                    new MethodHandler<RemoteWindow?>(
                        nameof(IRemoteAutomationService.CurrentWindow),
                        () => RemoteAutomationService.CurrentWindow()),
                    new MethodHandler<RemoteView?, string, string>(
                        nameof(IRemoteAutomationService.View),
                        (windowId, viewId) => RemoteAutomationService.View(windowId, viewId)),
                    new MethodHandler<RemoteView[], string, string, IViewSelector>(
                        nameof(IRemoteAutomationService.Descendants),
                        (windowId, viewId, selector) => RemoteAutomationService.Descendants(windowId, viewId, selector)),
                    new MethodHandler<IActionResult, string, string, IAction>(
                        nameof(IRemoteAutomationService.Invoke),
                        (windowId, viewId, action) => RemoteAutomationService.Invoke(windowId, viewId, action)!),
                    new MethodHandler<object?, string, string, string>(
                        nameof(IRemoteAutomationService.GetProperty),
                        (windowId, viewId, propertyName) => RemoteAutomationService.GetProperty(windowId, viewId, propertyName)!),
                    new MethodHandler<object?, string>(
                        nameof(IRemoteAutomationService.Platform),
                        (platform) =>
                        {
                            RemoteAutomationService.Platform(Enum.Parse<Platform>(platform));
                            return Task.FromResult<object?>(null);
                        })
                 );

                _ = Task.Run(() => { _ = Server.StartAsync(Stream); });
            }
            else
            {
                client = new ClientChannel(Stream);
            }
        }

        protected readonly Streamer.ServerChannel? Server;
        readonly Streamer.ClientChannel? client;
        protected Streamer.ClientChannel Client => client ?? throw new NullReferenceException();



        protected readonly IRemoteAutomationService? RemoteAutomationService;
        public readonly Stream Stream;

        public Task Platform(Platform platform)
            => Client.InvokeAsync(nameof(IRemoteAutomationService.Platform), platform.ToString());

        public async Task<IWindow?> CurrentWindow()
            => await Client.InvokeAsync<RemoteWindow?>(nameof(IRemoteAutomationService.CurrentWindow));

        public async Task<IWindow[]> Windows()
            => (await Client.InvokeAsync<RemoteWindow[]>(nameof(IRemoteAutomationService.Windows)))
                ?? Array.Empty<IWindow>();

        public async IAsyncEnumerable<IView> Descendants(IElement of, IViewSelector? selector = null)
        {
            if (of is IWindow window)
            {
                var windowId = window.Id;

                var views = await Client.InvokeAsync<string, string, IViewSelector, RemoteView[]>(
                    nameof(IRemoteAutomationService.Descendants), windowId, string.Empty, new DefaultViewSelector())
                        ?? Array.Empty<IView>();

                foreach (var d in views)
                {
                    if (selector == null || selector.Matches(d))
                        yield return d;
                }
            }
            else if (of is IView view)
            {
                var windowId = view.WindowId;
                var id = view.Id;

                var views = await Client.InvokeAsync<string, string, IViewSelector, RemoteView[]>(
                    nameof(IRemoteAutomationService.Descendants), windowId, id, new DefaultViewSelector())
                        ?? Array.Empty<IView>();

                foreach (var d in views)
                {
                    if (selector == null || selector.Matches(d))
                        yield return d;
                }
            }
        }

        public async Task<IView?> Descendant(IElement of, IViewSelector? selector = null)
        {
            await foreach (var d in Descendants(of, selector))
                return d;

            return null;
        }


        public Task<IActionResult> Invoke(IView view, IAction action)
            => Client.InvokeAsync<string, string, IAction, IActionResult>(
                nameof(IRemoteAutomationService.Invoke),
                view.WindowId,
                view.Id,
                action)!;

        public Task<object?> GetProperty(IView view, string propertyName)
            => Client.InvokeAsync<string, string, string, object?>(
                nameof(IRemoteAutomationService.GetProperty),
                view.WindowId,
                view.Id,
                propertyName);

        public Task<IWindow?> Window(string windowId)
            => Client.InvokeAsync<string, IWindow?>(
                nameof(IRemoteAutomationService),
                windowId);

        public async Task<IView?> View(string windowId, string viewId)
            => await Client.InvokeAsync<string, string, RemoteView?>(
                nameof(IRemoteAutomationService.View),
                windowId,
                viewId);

        public async IAsyncEnumerable<IView> Descendants(string windowId, string? viewId = null, IViewSelector? selector = null)
        {
            var views = await Client.InvokeAsync<string, string, IViewSelector, RemoteView[]>(
                nameof(IRemoteAutomationService.Descendants),
                windowId,
                viewId ?? string.Empty,
                selector ?? new DefaultViewSelector());

            if (views != null)
            {
                foreach (var v in views)
                    yield return v;
            }
        }

        public Task<IActionResult> Invoke(string windowId, string viewId, IAction action)
            => Client.InvokeAsync<string, string, IAction, IActionResult>(
                nameof(IRemoteAutomationService.Invoke),
                windowId,
                viewId,
                action)!;

        public Task<object?> GetProperty(string windowId, string viewId, string propertyName)
             => Client.InvokeAsync<string, string, string, object?>(
                nameof(IRemoteAutomationService.GetProperty),
                windowId,
                viewId,
                propertyName);
    }
}