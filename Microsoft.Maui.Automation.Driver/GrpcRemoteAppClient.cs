using Grpc.Core;
using Microsoft.Maui.Automation.RemoteGrpc;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Remote
{
	public class GrpcRemoteAppClient : RemoteGrpc.RemoteApp.RemoteAppBase
	{
		public GrpcRemoteAppClient(GrpcHost parentHost)
		{
            parentHost.SetCurrentClient(this);
            Console.WriteLine("New grpc Service");
		}

        //readonly Server server;
	
		Dictionary<string, TaskCompletionSource<IResponseMessage>> pendingResponses = new();
        
        TaskCompletionSource<IAsyncStreamWriter<ElementsRequest>> elementsRequestStream = new ();
		TaskCompletionSource<IAsyncStreamWriter<FindElementsRequest>> findElementsRequestStream = new();
        TaskCompletionSource<IAsyncStreamWriter<PropertyRequest>> getPropertyRequestStream = new();
        TaskCompletionSource<IAsyncStreamWriter<PerformActionRequest>> performActionRequestStream = new();

		public override Task GetElementsRoute(IAsyncStreamReader<ElementsResponse> requestStream, IServerStreamWriter<ElementsRequest> responseStream, ServerCallContext context)
			=> BuildRoute(elementsRequestStream, requestStream, responseStream, context, pendingResponses);

		public override Task FindElementsRoute(IAsyncStreamReader<ElementsResponse> requestStream, IServerStreamWriter<FindElementsRequest> responseStream, ServerCallContext context)
			=> BuildRoute(findElementsRequestStream, requestStream, responseStream, context, pendingResponses);

		public override Task GetPropertyRoute(IAsyncStreamReader<PropertyResponse> requestStream, IServerStreamWriter<PropertyRequest> responseStream, ServerCallContext context)
			=> BuildRoute(getPropertyRequestStream, requestStream, responseStream, context, pendingResponses);

        public override Task PerformActionRoute(IAsyncStreamReader<PerformActionResponse> requestStream, IServerStreamWriter<PerformActionRequest> responseStream, ServerCallContext context)
            => BuildRoute(performActionRequestStream, requestStream, responseStream, context, pendingResponses);



        public async Task<IEnumerable<Element>> GetElements(Platform platform)
		{
            var response = await BuildRequest<ElementsRequest, ElementsResponse>(
                elementsRequestStream,
                new ElementsRequest
                {
                    Platform = platform,
                });

            return response?.Elements ?? Enumerable.Empty<Element>();
		}

		public async Task<IEnumerable<Element>> FindElements(Platform platform, string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
		{
			var response = await BuildRequest<FindElementsRequest, ElementsResponse>(
				findElementsRequestStream,
				new FindElementsRequest
				{
                    Platform = platform,
                    AncestorId = ancestorId,
                    IsExpression = isExpression,
                    Pattern = pattern,
                    PropertyName = propertyName
                });

			return response?.Elements ?? Enumerable.Empty<Element>();
        }

		public async Task<string> GetProperty(Platform platform, string elementId, string propertyName)
		{
			var response = await BuildRequest<PropertyRequest, PropertyResponse>(
				getPropertyRequestStream,
				new PropertyRequest
				{
					Platform = platform,
					ElementId = elementId,
					PropertyName = propertyName
				});

			return response?.Value ?? string.Empty;
		}

        public async Task<PerformActionResult> PerformAction(Platform platform, string action, string elementId, params string[] arguments)
        {
            var request = new PerformActionRequest
            {
                Platform = platform,
                Action = action,
                ElementId = elementId,
            };

            if (arguments is not null && arguments.Length > 0)
                request.Arguments.AddRange(arguments);

            var response = await BuildRequest<PerformActionRequest, PerformActionResponse>(
                performActionRequestStream,
                request);

            if (response is not null)
            {
                return new PerformActionResult
                {
                    Result = response.Result,
                    Status = response.Status
                };
            }

            return new PerformActionResult
            {
                Result = string.Empty,
                Status = -1
            };
        }


        async Task<TResponse?> BuildRequest<TRequest, TResponse>(
			TaskCompletionSource<IAsyncStreamWriter<TRequest>> tcsRequestStream,
			TRequest request)
			where TResponse : class, IResponseMessage
			where TRequest : IRequestMessage
        {
			var stream = await tcsRequestStream.Task;

			var requestId = Guid.NewGuid().ToString();
			request.RequestId = requestId;

            var tcs = new TaskCompletionSource<IResponseMessage>();
            pendingResponses.Add(requestId, tcs);

            await stream.WriteAsync(request);

            var response = await tcs.Task;

			return response as TResponse;
        }

        async Task BuildRoute<TRequest, TResponse>(
            TaskCompletionSource<IAsyncStreamWriter<TRequest>> tcsRequestStream,
            IAsyncStreamReader<TResponse> requestStream,
            IServerStreamWriter<TRequest> responseStream,
            ServerCallContext context,
            Dictionary<string, TaskCompletionSource<TResponse>> pendingResponses)
            where TResponse : IResponseMessage
            where TRequest : IRequestMessage
        {

            tcsRequestStream.TrySetResult(responseStream);

            while (await requestStream.MoveNext(CancellationToken.None))
            {
                var requestId = requestStream.Current.RequestId;

                if (!string.IsNullOrEmpty(requestId) && pendingResponses.ContainsKey(requestId))
                {
                    var tcs = pendingResponses[requestId];
                    pendingResponses.Remove(requestId);
                    tcs.TrySetResult(requestStream.Current);
                }
            }
        }
	}
}