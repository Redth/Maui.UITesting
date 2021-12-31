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
                    new MethodHandler<ChildrenResponse, ChildrenRequest>(
                        nameof(IRemoteAutomationService.Children),
                        async req => new ChildrenResponse(await RemoteAutomationService.Children(req.Platform))),
                    new MethodHandler<ElementResponse, ElementRequest>(
                        nameof(IRemoteAutomationService.Element),
                        async req => new ElementResponse(await RemoteAutomationService.Element(req.Platform, req.ElementId))),
                    new MethodHandler<DescendantsResponse, DescendantsRequest>(
                        nameof(IRemoteAutomationService.Descendants),
                        async req => new DescendantsResponse(await RemoteAutomationService.Descendants(req.Platform, req.ElementId, req.Selector))),
                    new MethodHandler<PerformResponse, PerformRequest>(
                        nameof(IRemoteAutomationService.Perform),
                        async req => new PerformResponse(await RemoteAutomationService.Perform(req.Platform, req.ElementId, req.Action))),
                    new MethodHandler<GetPropertyResponse, GetPropertyRequest>(
                        nameof(IRemoteAutomationService.GetProperty),
                        async req => new GetPropertyResponse(await RemoteAutomationService.GetProperty(req.Platform, req.ElementId, req.PropertyName)))
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

        public async Task<IEnumerable<IElement>> Children(Platform platform)
        {
            var response = await Client.InvokeAsync<ChildrenRequest, ChildrenResponse>(new ChildrenRequest(platform));
            return response?.Result ?? Array.Empty<RemoteElement>();
        }

        public async Task<IElement?> Element(Platform platform, string elementId)
        {
            var response = await Client.InvokeAsync<ElementRequest, ElementResponse>(new ElementRequest(platform, elementId));
            return response?.Element;
        }

        public async Task<IEnumerable<IElement>> Descendants(Platform platform, string? elementId = null, IElementSelector? selector = null)
        {
            var response = await Client.InvokeAsync<DescendantsRequest, DescendantsResponse>(new DescendantsRequest(platform, elementId, selector));

            return response?.Result ?? Enumerable.Empty<IElement>();
        }

        public async Task<IActionResult> Perform(Platform platform, string elementId, IAction action)
        {
            var response = await Client.InvokeAsync<PerformRequest, PerformResponse>(new PerformRequest(platform, elementId, action));

            return response?.Result ?? new ActionResult(ActionResultStatus.Error, "Unknown");
        }

        public async Task<object?> GetProperty(Platform platform, string elementId, string propertyName)
        {
            var response = await Client.InvokeAsync<GetPropertyRequest, GetPropertyResponse>(new GetPropertyRequest(platform, elementId, propertyName));

            return response?.Result;
        }
    }
}