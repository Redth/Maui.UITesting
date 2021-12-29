using Streamer;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Automation.Remote
{
    public class RemoteApplication : IApplication
    {
        public RemoteApplication(Platform defaultPlatform, Stream stream, IRemoteAutomationService? remoteAutomationService = null)
        {
            DefaultPlatform = defaultPlatform;
            Stream = stream;

            if (remoteAutomationService != null)
            {
                RemoteAutomationService = remoteAutomationService;
                Server = Streamer.Channel.CreateServer();
                Server.Bind(
                    new MethodHandler<RemoteElement[], Platform>(
                        nameof(IRemoteAutomationService.Children),
                        (platform) => RemoteAutomationService.Children(platform)),
                    new MethodHandler<RemoteElement?, Platform, string>(
                        nameof(IRemoteAutomationService.Element),
                        (platform, id) => RemoteAutomationService.Element(platform, id)),
                    new MethodHandler<RemoteElement[], Platform, string, IElementSelector>(
                        nameof(IRemoteAutomationService.Descendants),
                        (platform, id, selector) => RemoteAutomationService.Descendants(platform, id, selector)!),
                    new MethodHandler<IActionResult, Platform, string, IAction>(
                        nameof(IRemoteAutomationService.Perform),
                        (platform, viewId, action) => RemoteAutomationService.Perform(platform, viewId, action)!),
                    new MethodHandler<object?, Platform, string, string>(
                        nameof(IRemoteAutomationService.GetProperty),
                        (platform, viewId, propertyName) => RemoteAutomationService.GetProperty(platform, viewId, propertyName)!)
                 );

                _ = Task.Run(() => { _ = Server.StartAsync(Stream); });
            }
            else
            {
                client = new ClientChannel(Stream);
            }
        }

        public Platform DefaultPlatform { get; }

        protected readonly Streamer.ServerChannel? Server;
        readonly Streamer.ClientChannel? client;
        protected Streamer.ClientChannel Client => client ?? throw new NullReferenceException();



        protected readonly IRemoteAutomationService? RemoteAutomationService;
        public readonly Stream Stream;

        public async IAsyncEnumerable<IElement> Children(Platform platform)
        {
            var children = (await Client.InvokeAsync<Platform, RemoteElement[]>(
                    nameof(IRemoteAutomationService.Children),
                    platform))
                        ?? Array.Empty<IElement>();

            foreach (var c in children)
                yield return c;
        }

        public Task<IElement?> Element(Platform platform, string elementId)
            => Client.InvokeAsync<Platform, string, IElement?>(
                nameof(IRemoteAutomationService.Element),
                platform,
                elementId);

        public async IAsyncEnumerable<IElement> Descendants(Platform platform, string? elementId = null, IElementSelector? selector = null)
        {
            var views = await Client.InvokeAsync<Platform, string, IElementSelector, RemoteElement[]>(
                nameof(IRemoteAutomationService.Descendants),
                platform,
                elementId ?? string.Empty,
                selector ?? new DefaultViewSelector());

            if (views != null)
            {
                foreach (var v in views)
                    yield return v;
            }
        }

        public Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
            => Client.InvokeAsync<Platform, string, IAction, IActionResult>(
                nameof(IRemoteAutomationService.Perform),
                platform,
                elementId,
                action)!;

        public Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
             => Client.InvokeAsync<Platform, string, string, object?>(
                nameof(IRemoteAutomationService.GetProperty),
                platform,
                elementId,
                propertyName);
    }
}