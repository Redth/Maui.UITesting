using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
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
		readonly AsyncDuplexStreamingCall<PropertyResponse, PropertyRequest> getPropertyCall;
		readonly AsyncDuplexStreamingCall<PerformActionResponse, PerformActionRequest> performActionCall;

		readonly Task elementsCallTask;
		readonly Task findElementsCallTask;
		readonly Task getPropertyCallTask;
		readonly Task performActionCallTask;

		private bool disposedValue;

		public GrpcRemoteAppAgent(IApplication application, string address, HttpMessageHandler? httpMessageHandler = null)
		{
			Application = application;

			var grpc = GrpcChannel.ForAddress(address, new GrpcChannelOptions
			{
				//ServiceConfig = new ServiceConfig
				//{
				//	MethodConfigs =
				//	 {
				//		 new MethodConfig
				//		 {
				//			 Names = { MethodName.Default },
				//			 RetryPolicy = new RetryPolicy
				//			 {
				//				 MaxAttempts = 60,
				//				 InitialBackoff = TimeSpan.FromSeconds(1),
				//				 MaxBackoff = TimeSpan.FromSeconds(5),
				//				 BackoffMultiplier = 1.1,
				//				 RetryableStatusCodes = { StatusCode.NotFound, StatusCode.Unavailable }
				//			 }
				//		 }
				//	 }
				//},
				//HttpHandler = new Grpc.Net.Client.Web.GrpcWebHandler(Grpc.Net.Client.Web.GrpcWebMode.GrpcWeb)
				//,
					//httpMessageHandler ?? new HttpClientHandler())
				//HttpHandler = new HttpClientHandler
				//{
				// ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
				//}
			}); ;

			client = new RemoteApp.RemoteAppClient(grpc);

			elementsCall = client.GetElementsRoute();
			findElementsCall = client.FindElementsRoute();
			getPropertyCall = client.GetPropertyRoute();
			performActionCall = client.PerformActionRoute();

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
				try
				{
					while (await findElementsCall.ResponseStream.MoveNext())
					{
						var response = await HandleFindElementsRequest(findElementsCall.ResponseStream.Current);
						await findElementsCall.RequestStream.WriteAsync(response);
					}
				} catch (Exception ex)
				{
					throw ex;
				}
			});

			getPropertyCallTask = Task.Run(async () =>
			{
				while (await getPropertyCall.ResponseStream.MoveNext())
				{
					var response = await HandleGetPropertyRequest(getPropertyCall.ResponseStream.Current);
					await getPropertyCall.RequestStream.WriteAsync(response);
				}
			});

			performActionCallTask = Task.Run(async () =>
			{
				while (await performActionCall.ResponseStream.MoveNext())
				{
					var response = await HandlePerformActionRequest(performActionCall.ResponseStream.Current);
					await performActionCall.RequestStream.WriteAsync(response);
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
				e.Matches(request.PropertyName, request.Pattern, request.IsExpression));

			var response = new ElementsResponse();
			response.RequestId = request.RequestId;
			response.Elements.AddRange(elements);

			return response;
		}

		async Task<PropertyResponse> HandleGetPropertyRequest(PropertyRequest request)
		{
			// Get the elements from the running app host
			var v = await Application.GetProperty(request.Platform, request.ElementId, request.PropertyName);

			var response = new PropertyResponse();
			response.Platform = request.Platform;
			response.RequestId = request.RequestId;
			response.Value = v;

			return response;
		}

		async Task<PerformActionResponse> HandlePerformActionRequest(PerformActionRequest request)
		{
			// Get the elements from the running app host
			var result = await Application.PerformAction(request.Platform, request.Action, request.ElementId, request.Arguments.ToArray());

			var response = new PerformActionResponse();
			response.Platform = request.Platform;
			response.RequestId = request.RequestId;
			response.Status = result.Status;
			response.Result = result.Result;

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
