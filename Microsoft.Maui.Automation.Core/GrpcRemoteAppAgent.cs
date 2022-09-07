using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
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

		public GrpcRemoteAppAgent(IApplication application, string address)
		{
			Application = application;

			var grpc = GrpcChannel.ForAddress(address);
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
				}
				catch (Exception ex)
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
			var response = new ElementsResponse();
			response.RequestId = request.RequestId;

			try
			{
				// Get the elements from the running app host
				var elements = await GetApp(request.Platform).GetElements();
				response.Elements.AddRange(elements);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return response;
		}

		async Task<ElementsResponse> HandleFindElementsRequest(FindElementsRequest request)
		{
			var response = new ElementsResponse();
			response.RequestId = request.RequestId;

			try
			{
				// Get the elements from the running app host
				var elements = await GetApp(request.Platform).FindElements(e =>
					e.PropertyMatches(request.PropertyName, request.Pattern, request.IsExpression));
				response.Elements.AddRange(elements);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return response;
		}

		async Task<PropertyResponse> HandleGetPropertyRequest(PropertyRequest request)
		{
			var response = new PropertyResponse();
			response.Platform = request.Platform;
			response.RequestId = request.RequestId;

			try
			{
				// Get the elements from the running app host
				var v = await GetApp(request.Platform).GetProperty(request.ElementId, request.PropertyName);

				response.Value = v;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				response.Value = string.Empty;
			}
			return response;

		}

		async Task<PerformActionResponse> HandlePerformActionRequest(PerformActionRequest request)
		{
			var response = new PerformActionResponse();
			response.Platform = request.Platform;
			response.RequestId = request.RequestId;

			try
			{
				// Get the elements from the running app host
				var result = await GetApp(request.Platform).PerformAction(request.Action, request.ElementId, request.Arguments.ToArray());
				response.Status = result.Status;
				response.Result = result.Result;
			}
			catch (Exception ex)
			{
				response.Status = PerformActionResult.ErrorStatus;
				response.Result = ex.Message;
			}

			return response;
		}

		IApplication GetApp(Platform platform)
		{
			var unsupportedText = $"Platform {platform} is not supported on this app agent";

			if (Application is MultiPlatformApplication multiApp)
			{
				if (!multiApp.PlatformApps.ContainsKey(platform))
					throw new NotSupportedException(unsupportedText);

				return multiApp.PlatformApps[platform];
			}

			if (Application.DefaultPlatform != platform)
				throw new NotSupportedException(unsupportedText);
			return Application;
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
