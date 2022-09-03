using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Maui.Automation.RemoteGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public class GrpcRemoteAppAgent : IDisposable
	{
		readonly RemoteApp.RemoteAppClient client;
		readonly IApplication Application;

		readonly AsyncDuplexStreamingCall<ElementsResponse, ElementsRequest> elementsCall;
		readonly AsyncDuplexStreamingCall<ElementsResponse, FindElementsRequest> findElementsCall;

		readonly Task elementsCallTask;
		readonly Task findElementsCallTask;
		private bool disposedValue;

		public GrpcRemoteAppAgent(IApplication application, string address)
		{
			Application = application;

			var grpc = GrpcChannel.ForAddress(address);
			client = new RemoteApp.RemoteAppClient(grpc);

			elementsCall = client.GetElementsRoute();
			findElementsCall = client.FindElementsRoute();

			elementsCallTask = Task.Run(async () =>
			{
				while (await elementsCall.ResponseStream.MoveNext())
				{
					var response = await HandleElementsRequest(elementsCall.ResponseStream.Current);
					await elementsCall.RequestStream.WriteAsync(response);
				}
			});

			findElementsCallTask = Task.Run(async () =>
			{
				while (await findElementsCall.ResponseStream.MoveNext())
				{
					var response = await HandleFindElementsRequest(findElementsCall.ResponseStream.Current);
					await findElementsCall.RequestStream.WriteAsync(response);
				}
			});
		}

		async Task<ElementsResponse> HandleElementsRequest(ElementsRequest request)
		{
			// Get the elements from the running app host
			var elements = await Application.GetElements(request.Platform);

			var response = new ElementsResponse();
			response.RequestId = request.RequestId;
			response.Elements.AddRange(elements);

			return response;
		}

		async Task<ElementsResponse> HandleFindElementsRequest(FindElementsRequest request)
		{
			// Get the elements from the running app host
			var elements = await Application.FindElements(request.Platform, e =>
			{
				var value =
					request.PropertyName.ToLowerInvariant() switch
					{
						"id" => e.Id,
						"automationid" => e.AutomationId,
						"text" => e.Text,
						"type" => e.Type,
						"fulltype" => e.FullType,
						_ => string.Empty
					} ?? string.Empty;

				return request.IsExpression
					? Regex.IsMatch(value, request.Pattern)
					: request.Pattern.Equals(value);
			});

			var response = new ElementsResponse();
			response.RequestId = request.RequestId;
			response.Elements.AddRange(elements);

			return response;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					elementsCall.Dispose();
					findElementsCall.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
