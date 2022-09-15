using Grpc.Core;

namespace Microsoft.Maui.Automation.Driver;

public static class GrpcExtensions
{
	public static Task RequestStream<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, TRequest request, Action<TResponse>? callback = null)
		=> call.RequestStream<TRequest, TResponse>(new[] { request }, callback);

	public static async Task RequestStream<TRequest, TResponse>(this AsyncDuplexStreamingCall<TRequest, TResponse> call, TRequest[] requests, Action<TResponse>? callback = null)
	{
		foreach (var request in requests)
			await call.RequestStream.WriteAsync(request);

		await call.RequestStream.CompleteAsync();

		await foreach (var response in call.ResponseStream.ReadAllAsync())
		{
			callback?.Invoke(response);
		}
	}

	public static Task<TResponse> SendStream<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, params TRequest[] requests)
		=> call.SendStream<TRequest, TResponse>(TimeSpan.Zero, requests);

	public static async Task<TResponse> SendStream<TRequest, TResponse>(this AsyncClientStreamingCall<TRequest, TResponse> call, TimeSpan delayBetween, params TRequest[] requests)
	{
		var first = true;

		foreach (var r in requests)
		{
			if (!first && delayBetween != TimeSpan.Zero)
			{
				await Task.Delay(delayBetween);
				first = false;
			}

			await call.RequestStream.WriteAsync(r);
		}

		await call.RequestStream.CompleteAsync();

		return await call.ResponseAsync;
	}


	public static async Task ReceiveStream<TResponse>(this AsyncServerStreamingCall<TResponse> call, Action<TResponse> callback)
	{
		while (await call.ResponseStream.MoveNext(CancellationToken.None))
		{
			callback?.Invoke(call.ResponseStream.Current);
		}
	}
}
