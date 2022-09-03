using Grpc.Core;
using Microsoft.Maui.Automation.RemoteGrpc;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Remote
{
	public class GrpcRemoteAppClient : RemoteGrpc.RemoteApp.RemoteAppBase
	{
		public GrpcRemoteAppClient()
		{
			server = new Server
			{
				Services = { RemoteApp.BindService(this) },
				Ports = { new ServerPort("127.0.0.1", 10882, ServerCredentials.Insecure) }
			};
			server.Start();
		}

		readonly Server server;
	
		Dictionary<string, TaskCompletionSource<IEnumerable<Element>>> pendingElementResponses = new();

		TaskCompletionSource<IAsyncStreamWriter<ElementsRequest>> elementsRequestStream = new ();
		TaskCompletionSource<IAsyncStreamWriter<FindElementsRequest>> findElementsRequestStream = new();

		public override async Task GetElementsRoute(IAsyncStreamReader<ElementsResponse> requestStream, IServerStreamWriter<ElementsRequest> responseStream, ServerCallContext context)
		{
			elementsRequestStream.TrySetResult(responseStream);

			while (await requestStream.MoveNext())
			{
				var requestId = requestStream.Current.RequestId;

				if (!string.IsNullOrEmpty(requestId) && pendingElementResponses.ContainsKey(requestId))
				{
					var tcs = pendingElementResponses[requestId];
					pendingElementResponses.Remove(requestId);
					tcs.TrySetResult(requestStream.Current.Elements);
				}
			}
		}

		public override async Task FindElementsRoute(IAsyncStreamReader<ElementsResponse> requestStream, IServerStreamWriter<FindElementsRequest> responseStream, ServerCallContext context)
		{
			findElementsRequestStream.TrySetResult(responseStream);

			while (await requestStream.MoveNext())
			{
				var requestId = requestStream.Current.RequestId;

				if (!string.IsNullOrEmpty(requestId) && pendingElementResponses.ContainsKey(requestId))
				{
					var tcs = pendingElementResponses[requestId];
					pendingElementResponses.Remove(requestId);
					tcs.TrySetResult(requestStream.Current.Elements);
				}
			}
		}

		public async Task<IEnumerable<Element>> GetElements(Platform platform)
		{
			var stream = await elementsRequestStream.Task;
			var requestId = Guid.NewGuid().ToString();
			var request = new ElementsRequest { Platform = platform, RequestId = requestId };

			var tcs = new TaskCompletionSource<IEnumerable<Element>>();
			pendingElementResponses.Add(requestId, tcs);

			await stream.WriteAsync(request);

			return await tcs.Task;
		}

		public async Task<IEnumerable<Element>> FindElements(Platform platform, string propertyName, string pattern, bool isExpression = false, string ancestorId = "")
		{
			var stream = await findElementsRequestStream.Task;
			var requestId = Guid.NewGuid().ToString();
			var request = new FindElementsRequest
			{
				Platform = platform,
				RequestId = requestId,
				AncestorId = ancestorId,
				IsExpression = isExpression,
				Pattern= pattern,
				PropertyName = propertyName
			};

			var tcs = new TaskCompletionSource<IEnumerable<Element>>();
			pendingElementResponses.Add(requestId, tcs);

			await stream.WriteAsync(request);

			return await tcs.Task;
		}

		public async Task Shutdown()
		{
			await server.ShutdownAsync();
		}


	}
}