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
		readonly AsyncDuplexStreamingCall<PerformActionResponse, PerformActionRequest> performActionCall;
        
        readonly Task elementsCallTask;
		readonly Task performActionCallTask;
        
        private bool disposedValue;

		public GrpcRemoteAppAgent(IApplication application, string address)
		{
			Application = application;

			var grpc = GrpcChannel.ForAddress(address);
			client = new RemoteApp.RemoteAppClient(grpc);

			elementsCall = client.GetElementsRoute();
			performActionCall = client.PerformActionRoute();
            
            elementsCallTask = Task.Run(async () =>
			{
				while (await elementsCall.ResponseStream.MoveNext())
				{
					var response = await HandleElementsRequest(elementsCall.ResponseStream.Current);
					await elementsCall.RequestStream.WriteAsync(response);
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

		async Task<PerformActionResponse> HandlePerformActionRequest(PerformActionRequest request)
		{
			var response = new PerformActionResponse();
			response.Platform = request.Platform;
			response.RequestId = request.RequestId;

			try
			{
				// Get the elements from the running app host
				var app = GetApp(request.Platform);

				if (request.Action == Actions.GetProperty)
				{
					var propertyName = request.Arguments.FirstOrDefault();

					if (string.IsNullOrEmpty(propertyName))
						throw new ArgumentNullException("PropertyName");

					var propertyValue = await app.GetProperty(request.ElementId, propertyName);
					response.Status = PerformActionResult.SuccessStatus;
					response.Results.Add(propertyValue);
				}
				else if (request.Action == Actions.Backdoor)
				{
					var fullyQualifiedTypeName = request.Arguments.FirstOrDefault();
					var staticMethodName = request.Arguments.Skip(1).FirstOrDefault();
					var remainingArgs = request.Arguments.Skip(2);

					var result = await app.Backdoor(fullyQualifiedTypeName, staticMethodName, remainingArgs.ToArray());

                    response.Status = PerformActionResult.SuccessStatus;
					if (result?.Any() ?? false)
	                    response.Results.AddRange(result?.ToArray());
                }
				else
				{
					var result = await app.PerformAction(request.Action, request.Arguments.ToArray());
					response.Status = result.Status;
					response.Results.AddRange(response.Results);
				}
			}
			catch (Exception ex)
			{
				response.Status = PerformActionResult.ErrorStatus;
				response.Results.Add(ex.Message);
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
					performActionCall.Dispose();
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
