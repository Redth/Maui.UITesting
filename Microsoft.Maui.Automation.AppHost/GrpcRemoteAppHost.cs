using Grpc.Core;
using Microsoft.Maui.Automation.RemoteGrpc;
using System;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation.Remote
{
    public class GrpcRemoteAppHost : RemoteGrpc.RemoteApp.RemoteAppBase
	{
		public GrpcRemoteAppHost(IApplication application)
		{
			this.Application = application;
		}

		protected readonly IApplication Application;

		public async override Task<ElementsResponse> GetElements(ElementsRequest request, ServerCallContext context)
		{
			try
			{
				var elements = await Application.GetElements(request.Platform, request.ElementId, request.ChildDepth);

				var resp = new ElementsResponse();
				resp.Elements.AddRange(elements);
				return resp;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}