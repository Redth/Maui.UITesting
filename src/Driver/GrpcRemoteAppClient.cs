using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Automation.RemoteGrpc;

namespace Microsoft.Maui.Automation.Remote
{
	public class GrpcRemoteAppClient : RemoteApp.RemoteAppBase
	{
		public GrpcRemoteAppClient(ILogger<GrpcRemoteAppClient> logger)
			: base()
		{
			logger.LogInformation("GRPC Service Created");
		}
	
		Dictionary<string, TaskCompletionSource<IResponseMessage>> pendingResponses = new();
		
		TaskCompletionSource<IAsyncStreamWriter<ElementsRequest>> elementsRequestStream = new ();
		TaskCompletionSource<IAsyncStreamWriter<PerformActionRequest>> performActionRequestStream = new();
		
		public override Task GetElementsRoute(IAsyncStreamReader<ElementsResponse> requestStream, IServerStreamWriter<ElementsRequest> responseStream, ServerCallContext context)
			=> BuildRoute(elementsRequestStream, requestStream, responseStream, context, pendingResponses);

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

		public async Task<PerformActionResult> PerformAction(Platform platform, string action, string? elementId, params string[] arguments)
		{
			var request = new PerformActionRequest
			{
				Platform = platform,
				Action = action,
			};

			if (!string.IsNullOrEmpty(elementId))
				request.ElementId = elementId;

			if (arguments is not null && arguments.Length > 0)
				request.Arguments.AddRange(arguments);

			var response = await BuildRequest<PerformActionRequest, PerformActionResponse>(
				performActionRequestStream,
				request);

			if (response is not null)
			{
				return new PerformActionResult
				{
					Results = response.Results.ToArray(),
					Status = response.Status
				};
			}

			return new PerformActionResult
			{
				Status = -1
			};
		}

		public async Task<string[]> Backdoor(Platform platform, string fullyQualifiedTypeName, string staticMethodName, params string[] args)
		{
			var allArgs = new string[2 + args.Length];
			allArgs[0] = fullyQualifiedTypeName;
			allArgs[1] = staticMethodName;
			for (int i = 0; i < args.Length; i++)
				allArgs[2 + i] = args[i];

			var r = await PerformAction(platform, Actions.Backdoor, null, allArgs);
			return r.Results ?? new string[0];
		}

		public async Task<string?> GetProperty(Platform platform, string elementId, string propertyName)
		{
			var r = await PerformAction(platform, Actions.GetProperty, elementId, propertyName);
			return r.Results?.FirstOrDefault();
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

			tcsRequestStream = new TaskCompletionSource<IAsyncStreamWriter<TRequest>>();
		}
	}
}